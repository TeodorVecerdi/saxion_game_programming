using System;
using System.Collections.Generic;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class Program : GXPEngine.Game {
        public Program() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC,
            pPixelArt: Globals.PIXEL_ART) {
            ShowMouse(true);
            targetFps = 24;
            var level1 = new Level("data/Levels/GameLevels/Level1.xml");
            var world = new World(level1);
            Input.AddAxis("Horizontal", new List<int> {Key.A, Key.LEFT}, new List<int> {Key.D, Key.RIGHT});
            Input.AddAxis("Vertical", new List<int> {Key.W, Key.UP}, new List<int> {Key.S, Key.DOWN});
            AddChild(world);
        }

        public static void Main(string[] args) {
            new Program().Start();
        }
    }
}