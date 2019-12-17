using System;
using System.Runtime.CompilerServices;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class World : GameObject {
        public int DiamondsCollected = 0;
        public int Score = 0;
        public int CurrentDiamondValue;
        public float TimeLeft;
        public Level Level;
        public GameObject[,] Objects;
        public Player Player;
        public bool GotEnoughDiamonds;

        private Vector2 playerPosition = Vector2.negativeInfinity;
        private int timeLeftToUpdate = 250;
        private int timeToUpdate = 250;

        public World(Level level) {
            if (game == null) throw new Exception("GameObjects cannot be created before creating a Game instance.");
            ResetLevel(level);
        }

        public void ResetLevel(Level level) {
            GetChildren().ForEach(child => child.LateDestroy());
            Objects = new GameObject[level.Width, level.Height];
            DiamondsCollected = 0;
            GotEnoughDiamonds = false;
            Level = level;
            CurrentDiamondValue = level.IntialDiamondValue;
            TimeLeft = level.CaveTime;
            for (var i = 0; i < level.Width; i++)
            for (var j = 0; j < level.Height; j++) {
                var position = new Vector2(i, j) * Globals.TILE_SIZE;
                switch (level.Tiles[i, j]) {
                    case 1: {
                        var obj = new SteelWall(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 2: {
                        var obj = new Door(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 3: {
                        var obj = new Brick(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 4: {
                        var obj = new Boulder(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 5: {
                        var obj = new Dirt(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 6: {
                        var obj = new Firefly(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 7: {
                        var obj = new MagicWall(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 8: {
                        var obj = new Amoeba(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 9: {
                        var obj = new Diamond(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 10: {
                        var obj = new Butterfly(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                    case 11: {
                        playerPosition = Vector2.one * position;
                        break;
                    }
                    case 12: {
                        var obj = new ExpandingWall(position);
                        Objects[i, j] = obj;
                        AddChild(obj);
                        break;
                    }
                }
            }

            Player = new Player(playerPosition, this);
            Objects[(int) (playerPosition.x / Globals.TILE_SIZE), (int) (playerPosition.y / Globals.TILE_SIZE)] = Player;
            AddChild(Player);
        }

        public void Update() {
            timeLeftToUpdate -= Time.deltaTimeMs;
            TimeLeft -= Time.deltaTime;
            if (TimeLeft < 0f) TimeLeft = 0f;
            var movement = new Vector2(Input.GetAxisDown("Horizontal"), Input.GetAxisDown("Vertical"));
            if (!movement.Equals(Vector2.zero)) {
                var playerPositionGrid = WorldToGrid(new Vector2(Player.x, Player.y));
                if (Level.Tileset.Tiles[Level[movement + playerPositionGrid]].Passable || (Level[movement + playerPositionGrid] == 2 && (Objects[(int) (movement + playerPositionGrid).x, (int) (movement + playerPositionGrid).y] as Door).IsOpen)) {
                    if (Level[movement + playerPositionGrid] == TileType.Diamond) CollectDiamond();
                    MovePlayer(movement);
                } else if (movement.y == 0 && Level[movement + playerPositionGrid] == TileType.Boulder && Level[movement + movement + playerPositionGrid] == TileType.Empty) {
                    var objLocation = movement + playerPositionGrid;
                    var newLocation = movement + movement + playerPositionGrid;
                    var obj = Objects[(int) objLocation.x, (int) objLocation.y] as Boulder;
                    obj.UpdatedThisFrame = true;
                    obj.Move(movement * Globals.TILE_SIZE);
                    Objects[(int) objLocation.x, (int) objLocation.y] = null;
                    Level[(int) objLocation.x, (int) objLocation.y] = TileType.Empty;
                    Objects[(int) newLocation.x, (int) newLocation.y] = obj;
                    Level[(int) newLocation.x, (int) newLocation.y] = TileType.Boulder;
                    MovePlayer(movement);
                }
            }

            if (timeLeftToUpdate < 0) {
                for (var i = 0; i < Level.Width; i++)
                for (var j = 0; j < Level.Height; j++)
                    if (Level[i, j] == TileType.Diamond && !(Objects[i, j] as Diamond).UpdatedThisFrame) {
                        UpdateDiamond(i, j);
                    } else if (Level[i, j] == TileType.Boulder && !(Objects[i, j] as Boulder).UpdatedThisFrame) {
                        UpdateBoulder(i, j);
                    } else if (Level[i, j] == TileType.Butterfly && !(Objects[i, j] as Butterfly).UpdatedThisFrame) {
                        UpdateButterfly(i, j);
                    } else if (Level[i, j] == TileType.Amoeba && !(Objects[i, j] as Amoeba).UpdatedThisFrame) {
                        UpdateAmoeba(i, j);
                    } else if (GotEnoughDiamonds && Level[i, j] == TileType.Door) {
                        var door = Objects[i, j] as Door;
                        door.IsOpen = true;
                        door.Flash();
                    }

                for (var i = 0; i < Level.Width; i++)
                for (var j = 0; j < Level.Height; j++) {
                    if (Level[i, j] == TileType.Boulder) (Objects[i, j] as Boulder).UpdatedThisFrame = false;
                    if (Level[i, j] == TileType.Diamond) (Objects[i, j] as Diamond).UpdatedThisFrame = false;
                    if (Level[i, j] == TileType.Butterfly) (Objects[i, j] as Butterfly).UpdatedThisFrame = false;
                    if (Level[i, j] == TileType.Amoeba) (Objects[i, j] as Amoeba).UpdatedThisFrame = false;
                }
            }

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
            if (DiamondsCollected >= Level.DiamondsNeeded) {
                GotEnoughDiamonds = true;
                CurrentDiamondValue = Level.ExtraDiamondValue;
            }
        }

        private void MovePlayer(Vector2 movement) {
            if (Level[(int) (Player.x / Globals.TILE_SIZE + movement.x), (int) (Player.y / Globals.TILE_SIZE + movement.y)] == TileType.Door) {
                Debug.LogWarning("YOU WON BYE");
                ResetLevel(new Level("data/Levels/GameLevels/Level2.xml"));
            }

            Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = null;
            Level[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = TileType.Empty;
            Player.Move(movement * Globals.TILE_SIZE);
            if (Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] != null)
                RemoveChild(Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)]);
            Objects[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = Player;
            Level[(int) (Player.x / Globals.TILE_SIZE), (int) (Player.y / Globals.TILE_SIZE)] = TileType.Miner;
        }

        private void UpdateBoulder(int i, int j) {
            // Try to move down
            if (j < Level.Height - 1 && Level[i, j + 1] == 0) {
                var obj = Objects[i, j] as Boulder;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(0, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i, j + 1] = obj;
                Level[i, j + 1] = TileType.Boulder;
            } //try to move left
            else if (j < Level.Height - 1 && i > 0 && Level[i - 1, j + 1] == 0 && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i - 1, j] == 0) {
                var obj = Objects[i, j] as Boulder;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(-Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i - 1, j + 1] = obj;
                Level[i - 1, j + 1] = TileType.Boulder;
            } //try to move right 
            else if (j < Level.Height - 1 && i < Level.Width - 1 && Level[i + 1, j + 1] == 0 && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i + 1, j] == 0) {
                var obj = Objects[i, j] as Boulder;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i + 1, j + 1] = obj;
                Level[i + 1, j + 1] = TileType.Boulder;
            } //try to kill player 
            else if (j < Level.Height - 1 && Level[i, j + 1] == 11 && (Objects[i, j] as Boulder).IsFalling) {
                Debug.LogError("YOU DED");
                (Objects[i, j] as Boulder).IsFalling = false;
            } // not falling anymore 
            else {
                (Objects[i, j] as Boulder).IsFalling = false;
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
            } else {
                (Objects[i, j] as Diamond).IsFalling = false;
            } /*
            TODO: maybe don't kill player ?
            //try to kill player 
            else if (j < Level.Height - 1 && Level[i, j + 1] == 11 && (Objects[i, j] as Diamond).IsFalling) {
                Debug.LogError("YOU DED");
                (Objects[i, j] as Diamond).IsFalling = false;
            }*/ // not falling anymore 
        }

        private void UpdateButterfly(int i, int j) {
            // Try to move down
            if (j < Level.Height - 1 && Level[i, j + 1] == 0) {
                var obj = Objects[i, j] as Butterfly;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(0, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i, j + 1] = obj;
                Level[i, j + 1] = TileType.Butterfly;
            } //try to move left
            else if (j < Level.Height - 1 && i > 0 && Level[i - 1, j + 1] == 0 && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i - 1, j] == 0) {
                var obj = Objects[i, j] as Butterfly;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(-Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i - 1, j + 1] = obj;
                Level[i - 1, j + 1] = TileType.Butterfly;
            } //try to move right 
            else if (j < Level.Height - 1 && i < Level.Width - 1 && Level[i + 1, j + 1] == 0 && Level.Tileset.Tiles[Level[i, j + 1]].Rounded && Level[i + 1, j] == 0) {
                var obj = Objects[i, j] as Butterfly;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                Level[i, j] = TileType.Empty;
                Objects[i + 1, j + 1] = obj;
                Level[i + 1, j + 1] = TileType.Butterfly;
            } //try to kill player 
            else if (j < Level.Height - 1 && Level[i, j + 1] == 11 && (Objects[i, j] as Butterfly).IsFalling) {
                Debug.LogError("YOU DED");
                (Objects[i, j] as Butterfly).IsFalling = false;
            } // not falling anymore 
            else {
                (Objects[i, j] as Butterfly).IsFalling = false;
            }
        }

        private void UpdateAmoeba(int i, int j) {
            //grow up
            if (j > 0 && Level[i, j - 1] == TileType.Empty) {
                Level[i, j - 1] = TileType.Amoeba;
                var obj = new Amoeba(i * Globals.TILE_SIZE, (j-1) * Globals.TILE_SIZE);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i, j - 1] = obj;
            }
            //grow down
            if (j < Level.Height && Level[i, j + 1] == TileType.Empty) {
                Level[i, j + 1] = TileType.Amoeba;
                var obj = new Amoeba(i * Globals.TILE_SIZE, (j+1) * Globals.TILE_SIZE);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i, j + 1] = obj;
            }
            //grow left
            if (i > 0 && Level[i - 1, j] == TileType.Empty) {
                Level[i-1, j] = TileType.Amoeba;
                var obj = new Amoeba((i-1) * Globals.TILE_SIZE, j * Globals.TILE_SIZE);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i-1, j] = obj;
            }
            //grow right
            if (i < Level.Width && Level[i + 1, j] == TileType.Empty) {
                Level[i+1, j] = TileType.Amoeba;
                var obj = new Amoeba((i+1) * Globals.TILE_SIZE, j * Globals.TILE_SIZE);
                obj.UpdatedThisFrame = true;
                AddChild(obj);
                Objects[i+1, j] = obj;
            }
        }
    }
}