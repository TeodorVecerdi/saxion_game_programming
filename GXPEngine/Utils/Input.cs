using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GXPEngine.Core;

namespace GXPEngine {
	/// <summary>
	///     The Input class contains functions for reading keys and mouse
	/// </summary>
	public class Input {
		private static Dictionary<string, ValueTuple<List<int>, List<int>>> Axes = new Dictionary<string, ValueTuple<List<int>, List<int>>>();
		/// <summary>
		///     Gets the current mouse x position in pixels.
		/// </summary>
		public static int mouseX => GLContext.mouseX;

		/// <summary>
		///     Gets the current mouse y position in pixels.
		/// </summary>
		public static int mouseY => GLContext.mouseY;

		/// <summary>
		///     Returns 'true' if given key is down, else returns 'false'
		/// </summary>
		/// <param name='key'>
		///     Key number, use Key.KEYNAME or integer value.
		/// </param>
		public static bool GetKey(int key) {
            return GLContext.GetKey(key);
        }

		/// <summary>
		///     Returns 'true' if specified key was pressed down during the current frame
		/// </summary>
		/// <param name='key'>
		///     Key number, use Key.KEYNAME or integer value.
		/// </param>
		public static bool GetKeyDown(int key) {
            return GLContext.GetKeyDown(key);
        }

		/// <summary>
		///     Returns 'true' if specified key was released during the current frame
		/// </summary>
		/// <param name='key'>
		///     Key number, use Key.KEYNAME or integer value.
		/// </param>
		public static bool GetKeyUp(int key) {
            return GLContext.GetKeyUp(key);
        }

		/// <summary>
		///     Returns 'true' if mousebutton is down, else returns 'false'
		/// </summary>
		/// <param name='button'>
		///     Number of button:
		///     0 = left button
		///     1 = right button
		///     2 = middle button
		/// </param>
		public static bool GetMouseButton(int button) {
            return GLContext.GetMouseButton(button);
        }

		/// <summary>
		///     Returns 'true' if specified mousebutton was pressed down during the current frame
		/// </summary>
		/// <param name='button'>
		///     Number of button:
		///     0 = left button
		///     1 = right button
		///     2 = middle button
		/// </param>
		public static bool GetMouseButtonDown(int button) {
            return GLContext.GetMouseButtonDown(button);
        }

		/// <summary>
		///     Returns 'true' if specified mousebutton was released during the current frame
		/// </summary>
		/// <param name='button'>
		///     Number of button:
		///     0 = left button
		///     1 = right button
		///     2 = middle button
		/// </param>
		public static bool GetMouseButtonUp(int button) {
            return GLContext.GetMouseButtonUp(button); /*courtesy of LeonB*/
        }

		public static void AddAxis(string axisName, List<int> negativeKeys, List<int> positiveKeys) {
			Axes.Add(axisName, ValueTuple<List<int>,List<int>>.Create(negativeKeys, positiveKeys));
		}

		public static float GetAxis(string axisName) {
			if (!Axes.ContainsKey(axisName)) {
				Console.Error.WriteLine($"Axis {axisName} does not exist.");
				throw new KeyNotFoundException($"Axis {axisName} does not exist.");
			}
			var value = 0f;
			if (Axes[axisName].Item1.Any(GetKey)) value -= 1f;
			if (Axes[axisName].Item2.Any(GetKey)) value += 1f;
			return value;
		}
		public static float GetAxisDown(string axisName) {
			if (!Axes.ContainsKey(axisName)) {
				Console.Error.WriteLine($"Axis {axisName} does not exist.");
				throw new KeyNotFoundException($"Axis {axisName} does not exist.");
			}
			var value = 0f;
			if (Axes[axisName].Item1.Any(GetKeyDown)) value -= 1f;
			if (Axes[axisName].Item2.Any(GetKeyDown)) value += 1f;
			return value;
		}
    }
}