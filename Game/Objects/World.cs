using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class World : GameObject {
        private Level level;
        private int timeToUpdate = 250;
        private int timeLeftToUpdate = 250;
        public int diamondsCollected = 0;

        public Sprite[,] Objects;
        public Vector2 playerPosition = Vector2.negativeInfinity;
        public Player player;

        public World(Level level, string texturePath) : this(level, Texture2D.GetInstance(texturePath, true)) { }

        public World(Level level, Texture2D texture) {
            if (game == null) throw new Exception("GameObjects cannot be created before creating a Game instance.");
            Objects = new Sprite[level.Width, level.Height];
            this.level = level;
            for (int i = 0; i < level.Width; i++) {
                for (int j = 0; j < level.Height; j++) {
                    Vector2 position = new Vector2(i, j) * Globals.TILE_SIZE;
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

//                            var obj = new Player(position);
//                            Objects[i,j] = (obj);
//                            AddChild(obj);
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
            }

            player = new Player(playerPosition, this);
            Objects[(int) (playerPosition.x / Globals.TILE_SIZE), (int) (playerPosition.y / Globals.TILE_SIZE)] = player;
            AddChild(player);
        }


        public void Update() {
            timeLeftToUpdate -= Time.deltaTimeMs;

            var movement = new Vector2(Input.GetAxisDown("Horizontal"), Input.GetAxisDown("Vertical"));
            if (!movement.Equals(Vector2.zero)) {
                Vector2 playerPositionGrid = WorldToGrid(new Vector2(player.x, player.y));
                if (Level.Tileset.Tiles[level[movement + playerPositionGrid]].Passable || (level[movement+playerPositionGrid] == 2 && (Objects[(int)(movement+playerPositionGrid).x, (int)(movement+playerPositionGrid).y] as Door).IsOpen)) {
                    if (level[movement + playerPositionGrid] == TileType.Diamond) diamondsCollected++;
                    /*if (level[movement + playerPositionGrid] == TileType.Dirt) {
                        Objects[(int) (movement + playerPositionGrid).x, (int) (movement + playerPositionGrid).y] = null;
                        level[movement + playerPositionGrid] = 0;
                    }*/
                    MovePlayer(movement);
                }
            }

            if (timeLeftToUpdate < 0) {
                for (int i = 0; i < level.Width; i++)
                for (int j = 0; j < level.Height; j++) {
                    if (level[i, j] == 4 && !(Objects[i, j] as Boulder).UpdatedThisFrame) {
                        DoBoulderUpdate(i, j);
                    }else if (diamondsCollected >= level.DiamondsNeeded && level[i, j] == 2) {
                        var door = Objects[i, j] as Door;
                        door.SetTexture(door.doorTexture);
                        door.IsOpen = true;
                    }
                }
                for (int i = 0; i < level.Width; i++)
                for (int j = 0; j < level.Height; j++) {
                    if (level[i, j] == 4) {
                        (Objects[i, j] as Boulder).UpdatedThisFrame = false;
                    }
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

        private void MovePlayer(Vector2 movement) {
            Objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = null;
            level[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = 0;
            player.Move(movement * Globals.TILE_SIZE);
            if (Objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] != null)
                RemoveChild(Objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)]);
            Objects[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = player;
            level[(int) (player.x / Globals.TILE_SIZE), (int) (player.y / Globals.TILE_SIZE)] = 11;
        }

        private void DoBoulderUpdate(int i, int j) {
            if (j < level.Height - 1 && level[i, j + 1] == 0) {
                var obj = Objects[i, j] as Boulder;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(0, Globals.TILE_SIZE);
                Objects[i, j] = null;
                level[i, j] = 0;
                Objects[i, j + 1] = obj;
                level[i, j + 1] = 4;
            } else if (j < level.Height - 1 && i > 0 && level[i - 1, j + 1] == 0 && level[i, j + 1] == 4 && level[i - 1, j] == 0) {
                var obj = Objects[i, j] as Boulder;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(-Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                level[i, j] = 0;
                Objects[i - 1, j + 1] = obj;
                level[i - 1, j + 1] = 4;
            } else if (j < level.Height - 1 && i < level.Width - 1 && level[i + 1, j + 1] == 0 && level[i, j + 1] == 4 && level[i + 1, j] == 0) {
                var obj = Objects[i, j] as Boulder;
                obj.UpdatedThisFrame = true;
                obj.IsFalling = true;
                obj.Move(Globals.TILE_SIZE, Globals.TILE_SIZE);
                Objects[i, j] = null;
                level[i, j] = 0;
                Objects[i + 1, j + 1] = obj;
                level[i + 1, j + 1] = 4;
            } else if (j < level.Height - 1 && level[i, j + 1] == 11 && (Objects[i, j] as Boulder).IsFalling) {
                Debug.LogError("YOU DED");
                (Objects[i, j] as Boulder).IsFalling = false;
            } else {
                (Objects[i, j] as Boulder).IsFalling = false;
            }
        }
    }
}