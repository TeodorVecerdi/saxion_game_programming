using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Game.Utils;
using GXPEngine;
using SysDebug = System.Diagnostics.Debug;

namespace Game {
    public class World : GameObject {
        public Level Level;
        private const float timeToMovePlayer = 166.6667F;
        private const int timeToUpdate = 250;
        private bool isAmoebaFinished;
        private bool loadedSounds;
        private float timeLeftToMovePlayer = 125F;
        private int playerLives = 3;
        private int timeLeftToUpdate = 250;
        private Camera camera;
        private GameObject[,] objects;
        private Player player;
        private Sound boulderStart, boulderEnd, enoughDiamonds, moveDirt, moveEmpty, pickupDiamond;

        public int CurrentDiamondValue { get; private set; }
        public int DiamondsCollected { get; private set; }
        public int Score { get; private set; }
        public float TimeLeft { get; private set; }
        public bool GotEnoughDiamonds { get; private set; }

        public World(Level level) {
            if (game == null) throw new Exception("GameObjects cannot be created before creating a Game instance.");
            LoadSounds();
            ResetLevel(level);
        }

        private void LoadSounds() {
            if (loadedSounds) {
                Debug.LogWarning("Tried to load sounds after already loading them once");
                return;
            }

            boulderStart = new Sound("data/sounds/boulder_start.wav");
            boulderEnd = new Sound("data/sounds/boulder_end.wav");
            enoughDiamonds = new Sound("data/sounds/enough_diamonds.wav");
            moveDirt = new Sound("data/sounds/move_dirt.wav");
            moveEmpty = new Sound("data/sounds/move_empty.wav");
            pickupDiamond = new Sound("data/sounds/pickup_diamond.wav");
            loadedSounds = true;
        }

        /// <summary>
        ///     Resets the level and then loads new level from <paramref name="level" />. If <paramref name="resetAll" /> is
        ///     <value>true</value>
        ///     ,
        ///     then the level is reset as if the game is just starting for the first time (i.e. reset score, reset player lives
        ///     etc)
        /// </summary>
        /// <param name="level">The level to load in</param>
        /// <param name="resetAll">Should reset all variables as if the game just started</param>
        private void ResetLevel(Level level, bool resetAll = false) {
            // Clear variables & prepare for level load into world
            BitmapCache.ClearCache();
            GetChildren().ForEach(child => child.LateDestroy());
            objects = new GameObject[level.Width, level.Height];
            DiamondsCollected = 0;
            GotEnoughDiamonds = false;
            isAmoebaFinished = false;
            if (resetAll) {
                playerLives = 3;
                Score = 0;
            }
            // Load new level into world
            Level = level;
            CurrentDiamondValue = level.IntialDiamondValue;
            TimeLeft = level.CaveTime;
            var playerPosition = Vector2.negativeInfinity;
            for (var j = 0; j < level.Height; j++)
            for (var i = 0; i < level.Width; i++) {
                var position = new Vector2(i, j) * Globals.TILE_SIZE;
                var tile = level.Tiles[i, j];
                if (tile != TileType.Miner && tile != TileType.Empty) {
                    var type = Type.GetType(TileType.TileTypeToClassName[level.Tiles[i, j]]) 
                               ?? throw new Exception("Could not find type to create world object.");
                    var obj = (GameObject) Activator.CreateInstance(type, position, level.Color1, level.Color2);
                    objects[i, j] = obj;
                    AddChild(obj);
                } else if (tile == TileType.Miner)
                    playerPosition = position;
            }

            player = new Player(playerPosition, level.Color1, level.Color2);
            camera = new Camera(this);
            camera.SetPosition(UpdateCamera((int) player.x, (int) player.y));
            objects[(int) (playerPosition.x / Globals.TILE_SIZE), (int) (playerPosition.y / Globals.TILE_SIZE)] = player;
            AddChild(player);
            AddChild(camera);
        }

        public void Update() {
            timeLeftToUpdate -= Time.deltaTimeMs;
            timeLeftToMovePlayer -= Time.deltaTimeMs;
            TimeLeft -= Time.deltaTime;

            if (TimeLeft < 0f) {
                KillPlayer();
            }

            // Update player
            var movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            //fix diagonal movement, prioritise horizontal movement
            if (movement.x != 0 && movement.y != 0) movement.y = 0;
            if (timeLeftToMovePlayer < 0f && movement.Equals(Vector2.zero))
                player.SetIdle(true);
            if (timeLeftToMovePlayer < 0f && !movement.Equals(Vector2.zero)) {
                timeLeftToMovePlayer = timeToMovePlayer;
                player.SetIdle(false);
                if (movement.x != 0) player.CurrentDirection = (int) movement.x;
                var playerPositionGrid = WorldToGrid(new Vector2(player.x, player.y));
                // check if the player can move in the desired direction and, if so, move
                if (Level.Tileset.Tiles[Level[movement + playerPositionGrid]].Passable || 
                    Level[movement + playerPositionGrid] == TileType.Door && 
                    (objects[(int) (movement + playerPositionGrid).x, (int) (movement + playerPositionGrid).y] as Door).IsOpen) {
                    if (Level[movement + playerPositionGrid] == TileType.Diamond) {
                        Vector2Int diamondPos = movement + playerPositionGrid;
                        var diamond = objects[diamondPos.x, diamondPos.y];
                        RemoveChild(diamond);
                        Level[movement + playerPositionGrid] = TileType.Empty;
                        CollectDiamond();
                    } else if (Level[movement + playerPositionGrid] == TileType.Dirt 
                               && (Input.GetKey(Key.LEFT_CTRL) || Input.GetKey(Key.RIGHT_CTRL))) {
                        Vector2Int dirtPos = movement + playerPositionGrid;
                        var dirt = objects[dirtPos.x, dirtPos.y];
                        RemoveChild(dirt);
                        Level[movement + playerPositionGrid] = TileType.Empty;
                        moveDirt.Play();
                    }

                    if (!Input.GetKey(Key.LEFT_CTRL) && !Input.GetKey(Key.RIGHT_CTRL))
                        MovePlayer(movement);
                } 
                // if the player cannot move in the desired position, check if there is a boulder there and if you can move the boulder 
                else if (movement.y == 0 && Level[movement + playerPositionGrid] == TileType.Boulder &&
                         Level[movement + movement + playerPositionGrid] == TileType.Empty) {
                    boulderEnd.Play();
                    var objLocation = movement + playerPositionGrid;
                    var newLocation = movement + movement + playerPositionGrid;
                    var obj = objects[(int) objLocation.x, (int) objLocation.y] as Boulder;
                    obj.UpdatedThisFrame = true;
                    obj.Move(movement * Globals.TILE_SIZE);
                    objects[(int) objLocation.x, (int) objLocation.y] = null;
                    Level[(int) objLocation.x, (int) objLocation.y] = TileType.Empty;
                    objects[(int) newLocation.x, (int) newLocation.y] = obj;
                    Level[(int) newLocation.x, (int) newLocation.y] = TileType.Boulder;
                    if (!Input.GetKey(Key.LEFT_CTRL) && !Input.GetKey(Key.RIGHT_CTRL))
                        MovePlayer(movement);
                }
            }

            // Update world
            if (timeLeftToUpdate < 0) {
                var amoebaExpandedThisUpdate = false;
                for (var j = 0; j < Level.Height; j++)
                for (var i = 0; i < Level.Width; i++)
                    if (Level[i, j] == TileType.Diamond && !(objects[i, j] as Diamond).UpdatedThisFrame)
                        UpdateDiamond(i, j);
                    else if (Level[i, j] == TileType.Boulder && !(objects[i, j] as Boulder).UpdatedThisFrame)
                        UpdateBoulder(i, j);
                    else if (Level[i, j] == TileType.Butterfly && !(objects[i, j] as Butterfly).UpdatedThisFrame)
                        UpdateButterfly(i, j);
                    else if (Level[i, j] == TileType.Amoeba && !(objects[i, j] as Amoeba).UpdatedThisFrame)
                        amoebaExpandedThisUpdate |= UpdateAmoeba(i, j);
                    else if (Level[i, j] == TileType.Firefly && !(objects[i, j] as Firefly).UpdatedThisFrame)
                        UpdateFirefly(i, j);
                    else if (GotEnoughDiamonds && Level[i, j] == TileType.Door) {
                        var door = objects[i, j] as Door;
                        door.IsOpen = true;
                        door.Flash();
                    }

                //transform amoeba if time expired
                if (!isAmoebaFinished && Level.CaveTime - TimeLeft >= Level.AmoebaSlowGrowthTime) {
                    for (var j = 0; j < Level.Height; j++)
                    for (var i = 0; i < Level.Width; i++)
                        if (Level[i, j] == TileType.Amoeba) {
                            RemoveChild(objects[i, j]);
                            objects[i, j] = null;
                            if (amoebaExpandedThisUpdate) {
                                objects[i, j] = new Boulder(i * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                                Level[i, j] = TileType.Boulder;
                            } else {
                                objects[i, j] = new Diamond(i * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                                Level[i, j] = TileType.Diamond;
                            }

                            AddChild(objects[i, j]);
                        }

                    isAmoebaFinished = true;
                }

                for (var j = 0; j < Level.Height; j++)
                for (var i = 0; i < Level.Width; i++)
                    if (Level[i, j] == TileType.Boulder) (objects[i, j] as Boulder).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Diamond) (objects[i, j] as Diamond).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Butterfly) (objects[i, j] as Butterfly).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Amoeba) (objects[i, j] as Amoeba).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Firefly) (objects[i, j] as Firefly).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.DiamondSpawner) (objects[i, j] as DiamondSpawner).UpdatedThisFrame = false;
            }

            camera.SetPosition(UpdateCamera((int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)));
            if (timeLeftToUpdate < 0) timeLeftToUpdate += timeToUpdate;
        }

        public Vector2 WorldToGrid(Vector2 world) {
            return world / Globals.TILE_SIZE;
        }

        public Vector2 GridToWorld(Vector2 grid) {
            return grid * Globals.TILE_SIZE;
        }

        private void CollectDiamond() {
            DiamondsCollected++;
            Score += CurrentDiamondValue;
            pickupDiamond.Play();
            if (DiamondsCollected >= Level.DiamondsNeeded && !GotEnoughDiamonds) {
                enoughDiamonds.Play();
                GotEnoughDiamonds = true;
                CurrentDiamondValue = Level.ExtraDiamondValue;
            }
        }

        private void MovePlayer(Vector2 movement) {
            var tile = Level[(int) (player.x / Globals.TILE_SIZE + movement.x), (int) (player.y / Globals.TILE_SIZE + movement.y)];

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (tile == TileType.Door) {
                Score += (int) TimeLeft;
                ResetLevel(new Level(Level.NextLevelPath));
            } else if (tile == TileType.Dirt) moveDirt.Play();
            else if (tile == TileType.Empty) moveEmpty.Play();

            objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = null;
            Level[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = TileType.Empty;
            player.Move(movement * Globals.TILE_SIZE);
            if (objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] != null)
                RemoveChild(objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)]);
            objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = player;
            Level[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = TileType.Miner;
        }

        private void KillPlayer() {
            playerLives--;
            if (playerLives <= 0)
                ResetLevel(new Level("data/Levels/GameLevels/Level1.xml"), true);
            else ResetLevel(new Level(Level.LevelPath));
        }

        /// <summary>
        ///     Calculates the correct camera position in the world according to player position
        /// </summary>
        /// <param name="playerX">x position of the player; in world coordinates</param>
        /// <param name="playerY">y position of the player; in world coordinates</param>
        /// <returns></returns>
        private Vector2 UpdateCamera(float playerX, float playerY) {
            // "Inspired" from C implementation of Boulder Dash by Ahmed Semih Özmekik
            // https://github.com/drh0use1/Boulder-Dash-C-Implementation
            var cameraPosition = Vector2.zero;
            const float xRadius = 1.3789f; // MAGIC NUMBERS, JUST DON'T TOUCH THEM
            const float yRadius = 1.46f;
            if (playerX > Level.Width / xRadius) playerX = Level.Width / xRadius + 0.5f;
            if (playerY > Level.Height / yRadius) playerY = Level.Height / yRadius + 0.5f;
            cameraPosition.x = -(Globals.WIDTH / 2f) + (playerX * Globals.TILE_SIZE + Globals.TILE_SIZE / 2f);
            cameraPosition.y = -(Globals.HEIGHT / 2f) + (playerY * Globals.TILE_SIZE + Globals.TILE_SIZE / 2f);
            if (cameraPosition.x < 0)
                cameraPosition.x = 0;
            if (cameraPosition.y < 0)
                cameraPosition.y = 0;
            return cameraPosition;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Already check object using custom Assert method from Game.Debug class")]
        private void UpdateBoulder(int i, int j) {
            // Checks
            Debug.Assert(i >= 0 && i < Level.Width && j >= 0 && j < Level.Height, "Boulder index is out of range.");
            Debug.Assert(objects[i, j] != null, "Object at index is null.");
            Debug.Assert(objects[i, j] is Boulder, "Object at index is not boulder.");
            Debug.Assert(Level[i, j] == TileType.Boulder, "Object at index is not boulder.");
            var obj = objects[i, j] as Boulder;

            // Try to move down
            if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Empty) {
                // var obj = objects[i, j] as Boulder;
                if (!obj.IsFalling) boulderStart.Play();
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(0, Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                objects[i, j + 1] = obj;
                Level[i, j + 1] = TileType.Boulder;
            } //try to move left
            else if (j < Level.Height - 1 && i > 0 && Level[i - 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i - 1, j] == TileType.Empty) {
                if (!obj.IsFalling) boulderStart.Play();
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(-Globals.TILE_SIZE, Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                objects[i - 1, j + 1] = obj;
                Level[i - 1, j + 1] = TileType.Boulder;
            } //try to move right 
            else if (j < Level.Height - 1 && i < Level.Width - 1 && Level[i + 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i + 1, j] == TileType.Empty) {
                if (!obj.IsFalling) boulderStart.Play();
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(Globals.TILE_SIZE, Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                objects[i + 1, j + 1] = obj;
                Level[i + 1, j + 1] = TileType.Boulder;
            } //try to kill player 
            else if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Miner && obj.IsFalling) {
                obj.IsFalling = false;
                KillPlayer();
            } //turn butterfly into diamonds 
            else if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Butterfly && obj.IsFalling) {
                var validSpotsForDiamondSpawning = new List<(int, int)> {ValueTuple.Create(i, j), ValueTuple.Create(i, j + 1)};
                if (i > 2) {
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i - 1, j));
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i - 1, j + 1));
                    if (j < Level.Height - 2)
                        validSpotsForDiamondSpawning.Add(ValueTuple.Create(i - 1, j + 2));
                }

                if (i < Level.Width - 2) {
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i + 1, j));
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i + 1, j + 1));
                    if (j < Level.Height - 2)
                        validSpotsForDiamondSpawning.Add(ValueTuple.Create(i + 1, j + 2));
                }

                if (j < Level.Height - 2)
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i, j + 2));

                foreach (var diamondSpawningSpot in validSpotsForDiamondSpawning) {
                    var obj1 = objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2];
                    if (obj1 != null && Level[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] != TileType.Miner) {
                        RemoveChild(obj1);
                        objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = null;
                    }

                    if (Level[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] != TileType.Miner) {
                        Level[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = TileType.Diamond;
                        objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = new Diamond(diamondSpawningSpot.Item1 * Globals.TILE_SIZE, diamondSpawningSpot.Item2 * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                        AddChild(objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2]);
                    } else
                        CollectDiamond();
                }
            } else {
                // not falling anymore
                // var obj = objects[i, j] as Boulder;
                if (obj.IsFalling)
                    boulderEnd.Play();
                obj.IsFalling = false;
            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "Already check object using custom Assert method from Game.Debug class")]
        private void UpdateDiamond(int i, int j) {
            // Checks
            Debug.Assert(i >= 0 && i < Level.Width && j >= 0 && j < Level.Height, "Diamond index is out of range.");
            Debug.Assert(objects[i, j] != null, "Object at index is null.");
            Debug.Assert(objects[i, j] is Diamond, "Object at index is not diamond.");
            Debug.Assert(Level[i, j] == TileType.Diamond, "Object at index is not diamond.");
            var obj = objects[i, j] as Diamond;

            // Try to move down
            if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Empty) {
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(0, Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                objects[i, j + 1] = obj;
                Level[i, j + 1] = TileType.Diamond;
            } // get diamond if falls on player
            else if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Miner && obj.IsFalling) {
                CollectDiamond();
                RemoveChild(obj);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
            } //try to move left
            else if (j < Level.Height - 1 && i > 0 && Level[i - 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i - 1, j] == TileType.Empty) {
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(-Globals.TILE_SIZE, Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                objects[i - 1, j + 1] = obj;
                Level[i - 1, j + 1] = TileType.Diamond;
            } //try to move right 
            else if (j < Level.Height - 1 && i < Level.Width - 1 && Level[i + 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i + 1, j] == TileType.Empty) {
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(Globals.TILE_SIZE, Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                objects[i + 1, j + 1] = obj;
                Level[i + 1, j + 1] = TileType.Diamond;
            } else
                obj.IsFalling = false;
        }

        private void UpdateButterfly(int i, int j) {
            // Wall following algorithm loosely based on maze solving algorithm
            // from https://pdfs.semanticscholar.org/1466/06c92916071ed6f6ae98fb27229660570bd3.pdf 

            // Checks
            Debug.Assert(i >= 0 && i < Level.Width && j >= 0 && j < Level.Height, "Butterfly index is out of range.");
            Debug.Assert(objects[i, j] != null, "Object at index is null.");
            Debug.Assert(objects[i, j] is Butterfly, "Object at index is not Butterfly.");
            Debug.Assert(Level[i, j] == TileType.Butterfly, "Object at index is not Butterfly.");

            var butterfly = (Butterfly) objects[i, j];
            var left = Misc.FollowWall.Vec2IntRotate90(butterfly.Direction);
            var back = Misc.FollowWall.Vec2IntRotate180(butterfly.Direction);
            var right = Misc.FollowWall.Vec2IntRotate270(butterfly.Direction);

            // move forward
            if (Level[i + butterfly.Direction.x, j + butterfly.Direction.y] == TileType.Empty || Level[i + butterfly.Direction.x, j + butterfly.Direction.y] == TileType.Miner) {
                butterfly.UpdatedThisFrame = true;
                butterfly.Move(butterfly.Direction.x * Globals.TILE_SIZE, butterfly.Direction.y * Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                if (Level[i + butterfly.Direction.x, j + butterfly.Direction.y] == TileType.Miner)
                    KillPlayer();

                objects[i + butterfly.Direction.x, j + butterfly.Direction.y] = butterfly;
                Level[i + butterfly.Direction.x, j + butterfly.Direction.y] = TileType.Butterfly;
                i += butterfly.Direction.x;
                j += butterfly.Direction.y;
            }

            // left  opening?
            if (Level[i + left.x, j + left.y] == TileType.Empty || Level[i + left.x, j + left.y] == TileType.Miner) // turn left
                butterfly.Direction.Set(left);

            // front wall?
            else if (Level[i + butterfly.Direction.x, j + butterfly.Direction.y] != TileType.Miner && Level[i + butterfly.Direction.x, j + butterfly.Direction.y] != TileType.Empty) {
                // right opening?
                if (Level[i + right.x, j + right.y] == TileType.Empty || Level[i + right.x, j + right.y] == TileType.Miner) // turn right
                    butterfly.Direction.Set(right);
                else // turn around
                    butterfly.Direction.Set(back);
            }
        }

        private bool UpdateAmoeba(int i, int j) {
            // Checks
            Debug.Assert(i >= 0 && i < Level.Width && j >= 0 && j < Level.Height, "Amoeba index is out of range.");
            Debug.Assert(objects[i, j] != null, "Object at index is null.");
            Debug.Assert(objects[i, j] is Amoeba, "Object at index is not Amoeba.");
            Debug.Assert(Level[i, j] == TileType.Amoeba, "Object at index is not Amoeba.");
            var expanded = false;

            //grow up
            if (j > 0 && Level[i, j - 1] == TileType.Empty) {
                Level[i, j - 1] = TileType.Amoeba;
                var obj = new Amoeba(i * Globals.TILE_SIZE, (j - 1) * Globals.TILE_SIZE, Level.Color1, Level.Color2) {UpdatedThisFrame = true};
                AddChild(obj);
                objects[i, j - 1] = obj;
                expanded = true;
            }

            //grow down
            if (j < Level.Height && Level[i, j + 1] == TileType.Empty) {
                Level[i, j + 1] = TileType.Amoeba;
                var obj = new Amoeba(i * Globals.TILE_SIZE, (j + 1) * Globals.TILE_SIZE, Level.Color1, Level.Color2) {UpdatedThisFrame = true};
                AddChild(obj);
                objects[i, j + 1] = obj;
                expanded = true;
            }

            //grow left
            if (i > 0 && Level[i - 1, j] == TileType.Empty) {
                Level[i - 1, j] = TileType.Amoeba;
                var obj = new Amoeba((i - 1) * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2) {UpdatedThisFrame = true};
                AddChild(obj);
                objects[i - 1, j] = obj;
                expanded = true;
            }

            //grow right
            if (i < Level.Width && Level[i + 1, j] == TileType.Empty) {
                Level[i + 1, j] = TileType.Amoeba;
                var obj = new Amoeba((i + 1) * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2) {UpdatedThisFrame = true};
                AddChild(obj);
                objects[i + 1, j] = obj;
                expanded = true;
            }

            return expanded;
        }

        private void UpdateFirefly(int i, int j) {
            // Wall following algorithm loosely based on maze solving algorithm
            // from https://pdfs.semanticscholar.org/1466/06c92916071ed6f6ae98fb27229660570bd3.pdf 

            // Checks
            Debug.Assert(i >= 0 && i < Level.Width && j >= 0 && j < Level.Height, "Firefly index is out of range.");
            Debug.Assert(objects[i, j] != null, "Object at index is null.");
            Debug.Assert(objects[i, j] is Firefly, "Object at index is not Firefly.");
            Debug.Assert(Level[i, j] == TileType.Firefly, "Object at index is not Firefly.");

            var firefly = (Firefly) objects[i, j];
            var left = Misc.FollowWall.Vec2IntRotate90(firefly.Direction);
            var back = Misc.FollowWall.Vec2IntRotate180(firefly.Direction);
            var right = Misc.FollowWall.Vec2IntRotate270(firefly.Direction);

            // move forward
            if (Level[i + firefly.Direction.x, j + firefly.Direction.y] == TileType.Empty || Level[i + firefly.Direction.x, j + firefly.Direction.y] == TileType.Miner) {
                firefly.UpdatedThisFrame = true;
                firefly.Move(firefly.Direction.x * Globals.TILE_SIZE, firefly.Direction.y * Globals.TILE_SIZE);
                objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                if (Level[i + firefly.Direction.x, j + firefly.Direction.y] == TileType.Miner)
                    KillPlayer();

                objects[i + firefly.Direction.x, j + firefly.Direction.y] = firefly;
                Level[i + firefly.Direction.x, j + firefly.Direction.y] = TileType.Firefly;
                i += firefly.Direction.x;
                j += firefly.Direction.y;
            }

            // left  opening?
            if (Level[i + left.x, j + left.y] == TileType.Empty || Level[i + left.x, j + left.y] == TileType.Miner) // turn left
                firefly.Direction.Set(left);

            // front wall?
            else if (Level[i + firefly.Direction.x, j + firefly.Direction.y] != TileType.Miner && Level[i + firefly.Direction.x, j + firefly.Direction.y] != TileType.Empty) {
                // right opening?
                if (Level[i + right.x, j + right.y] == TileType.Empty || Level[i + right.x, j + right.y] == TileType.Miner) // turn right
                    firefly.Direction.Set(right);
                else // turn around
                    firefly.Direction.Set(back);
            }
        }
    }
}