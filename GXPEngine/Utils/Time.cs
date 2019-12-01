using System;
using GXPEngine.OpenGL;

namespace GXPEngine {
	/// <summary>
	///     Contains various time related functions.
	/// </summary>
	public class Time {
        private static int previousTime;

        /// <summary>
        ///     Returns the current system time in milliseconds
        /// </summary>
        public static int now => Environment.TickCount;

        /// <summary>
        ///     Returns this time in milliseconds since the program started
        /// </summary>
        /// <value>
        ///     The time.
        /// </value>
        public static int time => (int) (GL.glfwGetTime() * 1000);
        /// <summary>
        ///     Returns the time in milliseconds that has passed since the previous frame
        /// </summary>
        /// <value>
        ///     The delta time.
        /// </value>
        public static int deltaTime { get; private set; }

        internal static void newFrame() {
            deltaTime = time - previousTime;
            previousTime = time;
        }
    }
}