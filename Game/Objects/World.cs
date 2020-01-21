using System;
using System.Collections.Generic;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class World : GameObject {
        public int CurrentDiamondValue;
        public int DiamondsCollected;
        public int Score;
        public float TimeLeft;
        public bool GotEnoughDiamonds;
        public bool Paused;
        public Level Level;
        public Camera Camera;
        public GameObject[,] Objects;
        public Player Player;

        private Sound boulderStart, boulderEnd, enoughDiamonds, moveDirt, moveEmpty, pickupDiamond;
        private readonly int timeToUpdate = 250;
        private readonly float timeToMovePlayer = 166.6667F;
        private int timeLeftToUpdate = 250;
        private float timeLeftToMovePlayer = 125F;
        private bool isAmoebaFinished;
        private bool loadedSounds;
        private Vector2 playerPosition = Vector2.negativeInfinity;

        public World(Level level) {
            if (game == null) throw new Exception("GameObjects cannot be created before creating a Game instance.");
            if (!loadedSounds)
                LoadSounds();
            ResetLevel(level);
        }

        private void LoadSounds() {
            boulderStart = new Sound("data/sounds/boulder_start.wav");
            boulderEnd = new Sound("data/sounds/boulder_end.wav");
            enoughDiamonds = new Sound("data/sounds/enough_diamonds.wav");
            moveDirt = new Sound("data/sounds/move_dirt.wav");
            moveEmpty = new Sound("data/sounds/move_empty.wav");
            pickupDiamond = new Sound("data/sounds/pickup_diamond.wav");
            loadedSounds = true;
        }

        public void ResetLevel(Level level) {
            BitmapCache.ClearCache();
            GetChildren().ForEach(child => child.LateDestroy());
            Objects = new GameObject[level.Width, level.Height];
            DiamondsCollected = 0;
            GotEnoughDiamonds = false;
            isAmoebaFinished = false;
            Level = level;
            CurrentDiamondValue = level.IntialDiamondValue;
            TimeLeft = level.CaveTime;
            for (var j = 0; j < level.Height; j++) 
            for (var i = 0; i < level.Width; i++) {
                var position = new Vector2(i, j) * Globals.TILE_SIZE;
                int tile = level.Tiles[i, j];
                if (tile != TileType.Miner && tile != TileType.Empty) {
                    var obj = (GameObject) Activator.CreateInstance(Type.GetType(TileType.TileTypeToClassName[level.Tiles[i, j]]), position, level.Color1, level.Color2);
                    Objects[i, j] = obj;
                    AddChild(obj);
                } else if (tile == TileType.Miner) {
                    playerPosition = Vector2.one * position;
                }
            }
            Player = new Player(playerPosition, level.Color1, level.Color2);
            Camera = new Camera(this);
            Camera.SetPosition(UpdateCamera((int) Player.x, (int) Player.y));
            Objects[(int) (playerPosition.x / Globals.TILE_SIZE), (int) (playerPosition.y / Globals.TILE_SIZE)] = Player;
            AddChild(Player);
            AddChild(Camera);
        }

        public void Update() {
            if (Input.GetKeyDown(Key.SPACE))
                Paused = !Paused;

            if (!Paused) {
                timeLeftToUpdate -= Time.deltaTimeMs;
                timeLeftToMovePlayer -= Time.deltaTimeMs;
                TimeLeft -= Time.deltaTime;
            }

            if (TimeLeft < 0f)
                TimeLeft = 0f;

            var movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (timeLeftToMovePlayer < 0f && movement.Equals(Vector2.zero))
                Player.SetIdle(true);
            if (timeLeftToMovePlayer < 0f && !movement.Equals(Vector2.zero)) {
                Player.SetIdle(false);
                if (movement.x == -1)
                    Player.CurrentDirection = -1;
                else if (movement.x == 1)
                    Player.CurrentDirection = 1;
                var playerPositionGrid = WorldToGrid(new Vector2(Player.x, Player.y));
                if (Level.Tileset.Tiles[Level[movement + playerPositionGrid]].Passable || Level[movement + playerPositionGrid] == TileType.Door && (Objects[(int) (movement + playerPositionGrid).x, (int) (movement + playerPositionGrid).y] as Door).IsOpen) {
                    if (Level[movement + playerPositionGrid] == TileType.Diamond) {
                        Vector2Int diamondPos = movement + playerPositionGrid;
                        var diamond = Objects[diamondPos.x, diamondPos.y];
                        RemoveChild(diamond);
                        Level[movement + playerPositionGrid] = TileType.Empty;
                        CollectDiamond();
                    } else if (Level[movement + playerPositionGrid] == TileType.Dirt && (Input.GetKey(Key.LEFT_CTRL) || Input.GetKey(Key.RIGHT_CTRL))) {
                        Vector2Int dirtPos = movement + playerPositionGrid;
                        var dirt = Objects[dirtPos.x, dirtPos.y];
                        RemoveChild(dirt);
                        Level[movement + playerPositionGrid] = TileType.Empty;
                        moveDirt.Play();
                    }
                    if (!Input.GetKey(Key.LEFT_CTRL) && !Input.GetKey(Key.RIGHT_CTRL))
                        MovePlayer(movement);
                } else if (movement.y == 0 && Level[movement + playerPositionGrid] == TileType.Boulder && Level[movement + movement + playerPositionGrid] == TileType.Empty) {
                    boulderEnd.Play();
                    var objLocation = movement + playerPositionGrid;
                    var newLocation = movement + movement + playerPositionGrid;
                    var obj = Objects[(int) objLocation.x, (int) objLocation.y] as Boulder;
                    obj.UpdatedThisFrame = true;
                    obj.Move(movement * Globals.TILE_SIZE);
                    Objects[(int) objLocation.x, (int) objLocation.y] = null;
                    Level[(int) objLocation.x, (int) objLocation.y] = TileType.Empty;
                    Objects[(int) newLocation.x, (int) newLocation.y] = obj;
                    Level[(int) newLocation.x, (int) newLocation.y] = TileType.Boulder;
                    if (!Input.GetKey(Key.LEFT_CTRL) && !Input.GetKey(Key.RIGHT_CTRL))
                        MovePlayer(movement);
                }
            }

            if (timeLeftToUpdate < 0) {
                var amoebaExpandedThisUpdate = false;
                for (var j = 0; j < Level.Height; j++)
                for (var i = 0; i < Level.Width; i++)
                    if (Level[i, j] == TileType.Diamond && !(Objects[i, j] as Diamond).UpdatedThisFrame)
                        UpdateDiamond(i, j);
                    else if (Level[i, j] == TileType.Boulder && !(Objects[i, j] as Boulder).UpdatedThisFrame)
                        UpdateBoulder(i, j);
                    else if (Level[i, j] == TileType.Butterfly && !(Objects[i, j] as Butterfly).UpdatedThisFrame)
                        UpdateButterfly(i, j);
                    else if (Level[i, j] == TileType.Amoeba && !(Objects[i, j] as Amoeba).UpdatedThisFrame)
                        amoebaExpandedThisUpdate |= UpdateAmoeba(i, j);
                    else if (Level[i, j] == TileType.Firefly && !(Objects[i, j] as Firefly).UpdatedThisFrame)
                        UpdateFirefly(i, j);
                    else if (Level[i, j] == TileType.DiamondSpawner && !(Objects[i, j] as DiamondSpawner).UpdatedThisFrame) {
                        UpdateDiamondSpawner(i, j);
                    } else if (GotEnoughDiamonds && Level[i, j] == TileType.Door) {
                        var door = Objects[i, j] as Door;
                        door.IsOpen = true;
                        door.Flash();
                    }

                if (!isAmoebaFinished && Level.CaveTime - TimeLeft >= Level.AmoebaSlowGrowthTime) {
                    //transform amoeba
                    for (var j = 0; j < Level.Height; j++)
                    for (var i = 0; i < Level.Width; i++)
                        if (Level[i, j] == TileType.Amoeba) {
                            RemoveChild(Objects[i, j]);
                            Objects[i, j] = null;
                            if (amoebaExpandedThisUpdate) {
                                Objects[i, j] = new Boulder(i * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                                Level[i, j] = TileType.Boulder;
                            } else {
                                Objects[i, j] = new Diamond(i * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                                Level[i, j] = TileType.Diamond;
                            }

                            AddChild(Objects[i, j]);
                        }

                    isAmoebaFinished = true;
                }

                for (var j = 0; j < Level.Height; j++) 
                for (var i = 0; i < Level.Width; i++) 
                    if (Level[i, j] == TileType.Boulder) (Objects[i, j] as Boulder).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Diamond) (Objects[i, j] as Diamond).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Butterfly) (Objects[i, j] as Butterfly).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Amoeba) (Objects[i, j] as Amoeba).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.Firefly) (Objects[i, j] as Firefly).UpdatedThisFrame = false;
                    else if (Level[i, j] == TileType.DiamondSpawner) (Objects[i, j] as DiamondSpawner).UpdatedThisFrame = false;
                
            }

            Camera.SetPosition(UpdateCamera((int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)));
            if (timeLeftToUpdate < 0) timeLeftToUpdate += timeToUpdate;
            if (timeLeftToMovePlayer < 0) timeLeftToMovePlayer += timeToMovePlayer;
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
            var tile = Level[(int) (Player.x / Globals.TILE_SIZE + movement.x), (int) (Player.y / Globals.TILE_SIZE + movement.y)];
            if (tile == TileType.Door) {
                Score += (int) TimeLeft;
                ResetLevel(new Level(Level.NextLevelPath));
            } else if (tile == TileType.Dirt) moveDirt.Play();
            else if (tile == TileType.Empty) moveEmpty.Play();

            Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = null;
            Level[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = TileType.Empty;
            Player.Move(movement * Globals.TILE_SIZE);
            if (Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] != null)
                RemoveChild(Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)]);
            Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = Player;
            Level[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = TileType.Miner;
        }

        private void KillPlayer(int i, int j) {
            Debug.LogError($"YOU DED at [{i}, {j}]");
        }

        private Vector2 UpdateCamera(float x, float y) {
            // "Inspired" from C implementation of Boulder Dash by Ahmed Semih Özmekik
            // https://github.com/drh0use1/Boulder-Dash-C-Implementation
            Vector2 cameraPosition = Vector2.zero;
            float x_radius = 1.3789f; // MAGIC NUMBERS, JUST DON'T TOUCH THEM
            float y_radius = 1.46f;
            if (x > Level.Width / x_radius) x = (Level.Width / x_radius + 0.5f);
            if (y > Level.Height / y_radius) y = (Level.Height / y_radius + 0.5f);
            cameraPosition.x = -(Globals.WIDTH / 2f) + (x * Globals.TILE_SIZE + Globals.TILE_SIZE / 2f);
            cameraPosition.y = -(Globals.HEIGHT / 2f) + (y * Globals.TILE_SIZE + Globals.TILE_SIZE / 2f);
            if (cameraPosition.x < 0)
                cameraPosition.x = 0;
            if (cameraPosition.y < 0)
                cameraPosition.y = 0;
            return cameraPosition;
        }

        private void UpdateDiamondSpawner(int i, int j) {
            //TODO: Implement diamond spawner instead of directly spawning diamonds when a butterfly explodes
        }

        private void UpdateBoulder(int i, int j) {
            // Try to move down
            if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Empty) {
                var obj = Objects[i, j] as Boulder;
                System.Diagnostics.Debug.Assert(obj != null, $"Boulder at [{i}, {j}] should never be null");
                if (!obj.IsFalling) boulderStart.Play();
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(0, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i, j + 1] = obj;
                Level[i, j + 1] = TileType.Boulder;
            } //try to move left
            else if (j < Level.Height - 1 && i > 0 && Level[i - 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i - 1, j] == TileType.Empty) {
                var obj = Objects[i, j] as Boulder;
                System.Diagnostics.Debug.Assert(obj != null, $"Boulder at [{i}, {j}] should never be null");
                if (!obj.IsFalling) boulderStart.Play();
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(-Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i - 1, j + 1] = obj;
                Level[i - 1, j + 1] = TileType.Boulder;
            } //try to move right 
            else if (j < Level.Height - 1 && i < Level.Width - 1 && Level[i + 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i + 1, j] == TileType.Empty) {
                var obj = Objects[i, j] as Boulder;
                System.Diagnostics.Debug.Assert(obj != null, $"Boulder at [{i}, {j}] should never be null");
                if (!obj.IsFalling) boulderStart.Play();
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i + 1, j + 1] = obj;
                Level[i + 1, j + 1] = TileType.Boulder;
            } //try to kill player 
            else if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Miner && (Objects[i, j] as Boulder).IsFalling) {
                KillPlayer(i, j + 1);
                (Objects[i, j] as Boulder).IsFalling = false;
            } //turn butterfly into diamonds 
            else if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Butterfly && (Objects[i, j] as Boulder).IsFalling) {
                List<ValueTuple<int, int>> validSpotsForDiamondSpawning = new List<(int, int)> {ValueTuple.Create(i, j), ValueTuple.Create(i, j + 1)};
                if (i > 2) {
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i - 1, j));
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i - 1, j + 1));
                    if (j < Level.Height - 2) {
                        validSpotsForDiamondSpawning.Add(ValueTuple.Create(i - 1, j + 2));
                    }
                }

                if (i < Level.Width - 2) {
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i + 1, j));
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i + 1, j + 1));
                    if (j < Level.Height - 2) {
                        validSpotsForDiamondSpawning.Add(ValueTuple.Create(i + 1, j + 2));
                    }
                }

                if (j < Level.Height - 2) {
                    validSpotsForDiamondSpawning.Add(ValueTuple.Create(i, j + 2));
                }

                foreach (var diamondSpawningSpot in validSpotsForDiamondSpawning) {
                    var obj = Objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2];
                    if (obj != null && Level[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] != TileType.Miner) {
                        RemoveChild(obj);
                        Objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = null;
                    }

                    //TODO: MAYBE USE THE DIAMOND SPAWNER INSTEAD OF STRAIGHT UP DIAMONDS
                    // Level[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = TileType.DiamondSpawner;
                    // Objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = new DiamondSpawner(diamondSpawningSpot.Item1 * Globals.TILE_SIZE, diamondSpawningSpot.Item2 * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                    if (Level[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] != TileType.Miner) {
                        Level[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = TileType.Diamond;
                        Objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2] = new Diamond(diamondSpawningSpot.Item1 * Globals.TILE_SIZE, diamondSpawningSpot.Item2 * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                        AddChild(Objects[diamondSpawningSpot.Item1, diamondSpawningSpot.Item2]);
                    } else {
                        CollectDiamond();
                    }
                }
            } else {
                // not falling anymore
                var obj = Objects[i, j] as Boulder;
                System.Diagnostics.Debug.Assert(obj != null, $"Boulder at [{i}, {j}] should never be null");
                if (obj.IsFalling)
                    boulderEnd.Play();
                obj.IsFalling = false;
            }
        }

        private void UpdateDiamond(int i, int j) {
            // Try to move down
            if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Empty) {
                var obj = Objects[i, j] as Diamond;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(0, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i, j + 1] = obj;
                Level[i, j + 1] = TileType.Diamond;
            } // get diamond if falls on player
            else if (j < Level.Height - 1 && Level[i, j + 1] == TileType.Miner && (Objects[i, j] as Diamond).IsFalling) {
                CollectDiamond();
                var obj = Objects[i, j] as Diamond;
                RemoveChild(obj);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
            } //try to move left
            else if (j < Level.Height - 1 && i > 0 && Level[i - 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i - 1, j] == TileType.Empty) {
                var obj = Objects[i, j] as Diamond;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(-Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i - 1, j + 1] = obj;
                Level[i - 1, j + 1] = TileType.Diamond;
            } //try to move right 
            else if (j < Level.Height - 1 && i < Level.Width - 1 && Level[i + 1, j + 1] == TileType.Empty && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i + 1, j] == TileType.Empty) {
                var obj = Objects[i, j] as Diamond;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i + 1, j + 1] = obj;
                Level[i + 1, j + 1] = TileType.Diamond;
            } else
                (Objects[i, j] as Diamond).IsFalling = false;
        }

        private void UpdateButterfly(int i, int j) {
            // Wall following algorithm loosely based on maze solving algorithm
            // from https://pdfs.semanticscholar.org/1466/06c92916071ed6f6ae98fb27229660570bd3.pdf 
            var butterfly = (Butterfly) Objects[i, j];
            var left = Misc.FollowWall.Vec2IntRotate90(butterfly.Direction);
            var back = Misc.FollowWall.Vec2IntRotate180(butterfly.Direction);
            var right = Misc.FollowWall.Vec2IntRotate270(butterfly.Direction);

            // move forward
            if (Level[i + butterfly.Direction.x, j + butterfly.Direction.y] == TileType.Empty || Level[i + butterfly.Direction.x, j + butterfly.Direction.y] == TileType.Miner) {
                butterfly.UpdatedThisFrame = true;
                butterfly.Move(butterfly.Direction.x * Globals.TILE_SIZE, butterfly.Direction.y * Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                if (Level[i + butterfly.Direction.x, j + butterfly.Direction.y] == TileType.Miner)
                    KillPlayer(i + butterfly.Direction.x, j + butterfly.Direction.y);

                Objects[i + butterfly.Direction.x, j + butterfly.Direction.y] = butterfly;
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
            var expanded = false;

            //grow up
            if (j > 0 && Level[i, j - 1] == TileType.Empty) {
                Level[i, j - 1] = TileType.Amoeba;
                var obj = new Amoeba(i * Globals.TILE_SIZE, (j - 1) * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i, j - 1] = obj;
                expanded = true;
            }

            //grow down
            if (j < Level.Height && Level[i, j + 1] == TileType.Empty) {
                Level[i, j + 1] = TileType.Amoeba;
                var obj = new Amoeba(i * Globals.TILE_SIZE, (j + 1) * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i, j + 1] = obj;
                expanded = true;
            }

            //grow left
            if (i > 0 && Level[i - 1, j] == TileType.Empty) {
                Level[i - 1, j] = TileType.Amoeba;
                var obj = new Amoeba((i - 1) * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i - 1, j] = obj;
                expanded = true;
            }

            //grow right
            if (i < Level.Width && Level[i + 1, j] == TileType.Empty) {
                Level[i + 1, j] = TileType.Amoeba;
                var obj = new Amoeba((i + 1) * Globals.TILE_SIZE, j * Globals.TILE_SIZE, Level.Color1, Level.Color2);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i + 1, j] = obj;
                expanded = true;
            }

            return expanded;
        }

        private void UpdateFirefly(int i, int j) {
            // Wall following algorithm loosely based on maze solving algorithm
            // from https://pdfs.semanticscholar.org/1466/06c92916071ed6f6ae98fb27229660570bd3.pdf 
            var firefly = (Firefly) Objects[i, j];
            var left = Misc.FollowWall.Vec2IntRotate90(firefly.Direction);
            var back = Misc.FollowWall.Vec2IntRotate180(firefly.Direction);
            var right = Misc.FollowWall.Vec2IntRotate270(firefly.Direction);

            // move forward
            if (Level[i + firefly.Direction.x, j + firefly.Direction.y] == TileType.Empty || Level[i + firefly.Direction.x, j + firefly.Direction.y] == TileType.Miner) {
                firefly.UpdatedThisFrame = true;
                firefly.Move(firefly.Direction.x * Globals.TILE_SIZE, firefly.Direction.y * Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                if (Level[i + firefly.Direction.x, j + firefly.Direction.y] == TileType.Miner)
                    KillPlayer(i + firefly.Direction.x, j + firefly.Direction.y);

                Objects[i + firefly.Direction.x, j + firefly.Direction.y] = firefly;
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