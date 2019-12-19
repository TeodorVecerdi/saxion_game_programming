using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using GLFW;
using GXPEngine.OpenGL;

namespace GXPEngine.Core {
    internal class WindowSize {
        public static WindowSize instance = new WindowSize();
        public int width, height;
    }

    public class GLContext {
        private const int MAXKEYS = 65535;
        private const int MAXBUTTONS = 255;

        private static readonly bool[] keys = new bool[MAXKEYS + 1];
        private static readonly bool[] keydown = new bool[MAXKEYS + 1];
        private static readonly bool[] keyup = new bool[MAXKEYS + 1];
        private static readonly bool[] buttons = new bool[MAXBUTTONS + 1];
        private static readonly bool[] mousehits = new bool[MAXBUTTONS + 1];
        private static readonly bool[] mouseup = new bool[MAXBUTTONS + 1]; //mouseup kindly donated by LeonB

        public static int mouseX;
        public static int mouseY;
        
        private static GLFW.Window _window;

        private static double _realToLogicWidthRatio;
        private static double _realToLogicHeightRatio;

        private readonly Game _owner;
        private int _frameCount;
        private long _lastFPSTime;
        private long _lastFrameTime;

        private int _targetFrameRate = 60;
        private bool _vsyncEnabled;
        
        private PositionCallback windowPositionCallback;
        private SizeCallback windowSizeCallback, framebufferSizeCallback;
        private FocusCallback windowFocusCallback;
        private WindowCallback closeCallback, windowRefreshCallback;
        private FileDropCallback dropCallback;
        private MouseCallback cursorPositionCallback, scrollCallback;
        private MouseEnterCallback cursorEnterCallback;
        private MouseButtonCallback mouseButtonCallback;
        private CharModsCallback charModsCallback;
        private KeyCallback keyCallback;
        private WindowMaximizedCallback windowMaximizeCallback;
        private WindowContentsScaleCallback windowContentScaleCallback;

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public GLContext(Game owner) {
            _owner = owner;
            currentFps = _targetFrameRate;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Width
        //------------------------------------------------------------------------------------------------------------------------
        public int width => WindowSize.instance.width;

        //------------------------------------------------------------------------------------------------------------------------
        //														Height
        //------------------------------------------------------------------------------------------------------------------------
        public int height => WindowSize.instance.height;

        public int currentFps { get; private set; }

        public int targetFps {
            get => _targetFrameRate;
            set {
                if (value < 1)
                    _targetFrameRate = 1;
                else
                    _targetFrameRate = value;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														setupWindow()
        //------------------------------------------------------------------------------------------------------------------------
        public void CreateWindow(int width, int height, bool fullScreen, bool vSync, int realWidth, int realHeight, string title) {
            // This stores the "logical" width, used by all the game logic:
            WindowSize.instance.width = width;
            WindowSize.instance.height = height;
            _realToLogicWidthRatio = (double) realWidth / width;
            _realToLogicHeightRatio = (double) realHeight / height;
            _vsyncEnabled = vSync;
            
//            _window = new NativeWindow(width, height, title, fullScreen?Glfw.PrimaryMonitor:Monitor.None, GLFW.Window.None);
//            return;
            Glfw.Init();
//            GL.glfwInit();
            Glfw.WindowHint(Hint.Samples, 8);
//            GL.glfwOpenWindowHint(GL.GLFW_FSAA_SAMPLES, 8);
            _window = Glfw.CreateWindow(realWidth, realHeight, title, fullScreen ? Glfw.PrimaryMonitor : Monitor.None, GLFW.Window.None);
            if (Glfw.GetClientApi(_window) != ClientApi.None)
                Glfw.MakeContextCurrent(_window);
//            BindCallbacks();
//            Glfw.SwapInterval(vSync ? 1 : 0);
//            GL.glfwOpenWindow(realWidth, realHeight, 8, 8, 8, 8, 24, 0, fullScreen ? GL.GLFW_FULLSCREEN : GL.GLFW_WINDOWED);
//            GL.glfwSetWindowTitle(title);
//            GL.glfwSwapInterval(vSync);
            Glfw.SetKeyCallback(_window, (ptr, key, code, state, mods) => {
                var press = state == InputState.Press;
                if (press) keydown[(int) key] = true;
                else keyup[(int) key] = true;
                keys[(int) key] = press;
            });
            Glfw.SetMouseButtonCallback(_window, (ptr, button, state, modifiers) => {
                var press = state == InputState.Press;
                if (press) mousehits[(int) button] = true;
                else mouseup[(int) button] = true;
                buttons[(int) button] = press;
            });
            Glfw.SetWindowSizeCallback(_window, (ptr, newWidth, newHeight) => {
//                Glfw.SetWindowSize(window, newWidth, newHeight);
                GL.Viewport(0, 0, newWidth, newHeight);
                GL.Enable(GL.MULTISAMPLE);
                GL.Enable(GL.TEXTURE_2D);
                GL.Enable(GL.BLEND);
                GL.BlendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
                GL.Hint(GL.PERSPECTIVE_CORRECTION, GL.FASTEST);

                //GL.Enable (GL.POLYGON_SMOOTH);
                GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

                // Load the basic projection settings:
                GL.MatrixMode(GL.PROJECTION);
                GL.LoadIdentity();

                // Here's where the conversion from logical width/height to real width/height happens: 
                GL.Ortho(0.0f, newWidth / _realToLogicWidthRatio, newHeight / _realToLogicHeightRatio, 0.0f, 0.0f, 1000.0f);

                lock (WindowSize.instance) {
                    WindowSize.instance.width = (int) (newWidth / _realToLogicWidthRatio);
                    WindowSize.instance.height = (int) (newHeight / _realToLogicHeightRatio);
                }

                if (Game.main != null) Game.main.RenderRange = new Rectangle(0, 0, WindowSize.instance.width, WindowSize.instance.height);
            });
            Glfw.ShowWindow(_window);
            
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ShowCursor()
        //------------------------------------------------------------------------------------------------------------------------
        public void ShowCursor(bool enable) {
            if (enable)
                Glfw.SetInputMode(_window, InputMode.Cursor, (int)CursorMode.Normal);
//                GL.glfwEnable(GL.GLFW_MOUSE_CURSOR);
            else
                Glfw.SetInputMode(_window, InputMode.Cursor, (int)CursorMode.Hidden);
//                GL.glfwDisable(GL.GLFW_MOUSE_CURSOR);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetScissor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetScissor(int x, int y, int width, int height) {
            if (width == WindowSize.instance.width && height == WindowSize.instance.height)
                GL.Disable(GL.SCISSOR_TEST);
            else
                GL.Enable(GL.SCISSOR_TEST);

            GL.Scissor(
                (int) (x * _realToLogicWidthRatio),
                (int) (y * _realToLogicHeightRatio),
                (int) (width * _realToLogicWidthRatio),
                (int) (height * _realToLogicHeightRatio)
            );

            //GL.Scissor(x, y, width, height);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Close()
        //------------------------------------------------------------------------------------------------------------------------
        public void Close() {
            Glfw.SetWindowShouldClose(_window, true);
            Glfw.DestroyWindow(_window);
            Glfw.Terminate();
            Environment.Exit(0);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Run()
        //------------------------------------------------------------------------------------------------------------------------
        public void Run() {
            //Update();
            Glfw.Time = 0.0;
//            GL.glfwSetTime(0.0);
            do {
                
                if (_vsyncEnabled || Time.time - _lastFrameTime > 1000 / _targetFrameRate) {
                    _lastFrameTime = Time.time;

                    //actual fps count tracker
                    _frameCount++;
                    if (Time.time - _lastFPSTime > 1000) {
                        currentFps = (int) (_frameCount / ((Time.time - _lastFPSTime) / 1000.0f));
                        _lastFPSTime = Time.time;
                        _frameCount = 0;
                    }

                    UpdateMouseInput();
//                    Console.WriteLine($"Mouse: [{mouseX}, {mouseY}]");
                    _owner.Step();

                    ResetHitCounters();
                    Display();

                    Time.newFrame();
                    Glfw.PollEvents();
                    Glfw.SwapBuffers(_window);
                    int err = GL.GetError();
                    string description;
                    var errorCode = Glfw.GetError(out description);
                    if (errorCode != ErrorCode.None || err != 0) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"GLFW: ErrCo: {errorCode}, description: {description}");
                        Console.WriteLine($"GL: ErrCo: {err}");
                        Console.ResetColor();
                    }
//                    GL.glfwPollEvents();
                }

//            } while (GL.glfwGetWindowParam(GL.GLFW_ACTIVE) == 1);
//            } while (Glfw.GetWindowAttribute(_window, WindowAttribute.Focused));
            } while (!Glfw.WindowShouldClose(_window));
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														display()
        //------------------------------------------------------------------------------------------------------------------------
        private void Display() {
            GL.Clear(GL.COLOR_BUFFER_BIT);
            GL.MatrixMode(GL.MODELVIEW);
            GL.LoadIdentity();

            _owner.Render(this);
//            Glfw.SwapBuffers(_window);
//            GL.glfwSwapBuffers();
            if (GetKey(Key.Escape)) Close();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														SetColor()
        //------------------------------------------------------------------------------------------------------------------------
        public void SetColor(byte r, byte g, byte b, byte a) {
            GL.Color4ub(r, g, b, a);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PushMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PushMatrix(float[] matrix) {
            GL.PushMatrix();
            GL.MultMatrixf(matrix);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														PopMatrix()
        //------------------------------------------------------------------------------------------------------------------------
        public void PopMatrix() {
            GL.PopMatrix();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														DrawQuad()
        //------------------------------------------------------------------------------------------------------------------------
        public void DrawQuad(float[] vertices, float[] uv) {
            string vertex = @"#version 400\n
in vec3 vp;
void main() {
    gl_Position = vec4(vp, 1.0);
}";
            string fragment = @"#version 400\n
out vec4 frag_color;
void main() {
    frag_color = vec4(1.0, 1.0, 1.0, 1.0);
}";
            /*var vertex_shader = GL.CreateShader(GL.GL_VERTEX_SHADER);
            GL.ShaderSource(vertex_shader, 1, vertex, vertex.Length);
            GL.CompileShader(vertex_shader);
            var fragment_shader = GL.CreateShader(GL.GL_FRAGMENT_SHADER);
            GL.ShaderSource(fragment_shader, 1, fragment, fragment.Length);
            GL.CompileShader(fragment_shader);
            var program = GL.CreateProgram();
            GL.AttachShader(program, fragment_shader);
            GL.AttachShader(program, vertex_shader);
            GL.LinkProgram(program);
            GL.UseProgram(program);*/
            GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.TexCoordPointer(2, GL.FLOAT, 0, uv);
            GL.VertexPointer(2, GL.FLOAT, 0, vertices);
            GL.DrawArrays(GL.QUADS, 0, 4);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);
        }

        /// <summary>
        ///     Draws triangles using 2D coordinates for vertices
        /// </summary>
        public void DrawTriangles2D(float[] vertices, int[] indices, float[] uvs) {
            GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.TexCoordPointer(2, GL.FLOAT, 0, uvs);
            GL.VertexPointer(2, GL.FLOAT, 0, vertices);
            GL.DrawElements(GL.TRIANGLES, indices.Length, GL.UNSIGNED_INT, indices);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);
        }

        /// <summary>
        ///     Draws triangles using 3D coordinates for vertices
        /// </summary>
        public void DrawTriangles(float[] vertices, int[] indices, float[] uvs) {
            GL.EnableClientState(GL.TEXTURE_COORD_ARRAY);
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.TexCoordPointer(2, GL.FLOAT, 0, uvs);
            GL.VertexPointer(3, GL.FLOAT, 0, vertices);
            GL.DrawElements(GL.TRIANGLES, indices.Length, GL.UNSIGNED_INT, indices);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.DisableClientState(GL.TEXTURE_COORD_ARRAY);
        }

        public void DrawMesh(Mesh mesh) {
            var vertices = new List<float>();
            var uvs = new List<float>();
            foreach (var vertex in mesh.Vertices) {
                vertices.Add(vertex.x);
                vertices.Add(vertex.y);
                vertices.Add(vertex.z);
            }

            foreach (var uv in mesh.Uvs) {
                uvs.Add(uv.x);
                uvs.Add(uv.y);
            }

            mesh.Texture.Bind();
            DrawTriangles(vertices.ToArray(), mesh.IndexArray, uvs.ToArray());
            mesh.Texture.Unbind();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKey()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKey(int key) {
            return keys[key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyDown(int key) {
            return keydown[key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetKeyUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetKeyUp(int key) {
            return keyup[key];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButton()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButton(int button) {
            return buttons[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonDown()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonDown(int button) {
            return mousehits[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														GetMouseButtonUp()
        //------------------------------------------------------------------------------------------------------------------------
        public static bool GetMouseButtonUp(int button) {
            return mouseup[button];
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														ResetHitCounters()
        //------------------------------------------------------------------------------------------------------------------------
        public static void ResetHitCounters() {
            Array.Clear(keydown, 0, MAXKEYS);
            Array.Clear(keyup, 0, MAXKEYS);
            Array.Clear(mousehits, 0, MAXBUTTONS);
            Array.Clear(mouseup, 0, MAXBUTTONS);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateMouseInput()
        //------------------------------------------------------------------------------------------------------------------------
        public static void UpdateMouseInput() {
            double mX, mY;
            Glfw.GetCursorPosition(_window, out mX, out mY);
            mouseX = (int) mX;
            mouseY = (int) mY;
//            GL.glfwGetMousePos(out mouseX, out mouseY);
            mouseX = (int) (mouseX / _realToLogicWidthRatio);
            mouseY = (int) (mouseY / _realToLogicHeightRatio);
        }
        
        /*private void BindCallbacks()
        {
            windowPositionCallback = (_, x, y) => OnPositionChanged(x, y);
            windowSizeCallback = (_, w, h) => OnSizeChanged(w, h);
            windowFocusCallback = (_, focusing) => OnFocusChanged(focusing);
            closeCallback = _ => OnClosing();
            dropCallback = (_, count, arrayPtr) => OnFileDrop(count, arrayPtr);
            cursorPositionCallback = (_, x, y) => OnMouseMove(x, y);
            cursorEnterCallback = (_, entering) => OnMouseEnter(entering);
            mouseButtonCallback = (_, button, state, mod) => OnMouseButton(button, state, mod);
            scrollCallback = (_, x, y) => OnMouseScroll(x, y);
            charModsCallback = (_, cp, mods) => OnCharacterInput(cp, mods);
            framebufferSizeCallback = (_, w, h) => OnFramebufferSizeChanged(w, h);
            windowRefreshCallback = _ => Refreshed?.Invoke(this, EventArgs.Empty);
            keyCallback = (_, key, code, state, mods) => OnKey(key, code, state, mods);
            windowMaximizeCallback = (_, maximized) => OnMaximizeChanged(maximized);
            windowContentScaleCallback = (_, x, y) => OnContentScaleChanged(x, y);

            Glfw.SetWindowPositionCallback(_window, windowPositionCallback);
            Glfw.SetWindowSizeCallback(_window, windowSizeCallback);
            Glfw.SetWindowFocusCallback(_window, windowFocusCallback);
            Glfw.SetCloseCallback(_window, closeCallback);
            Glfw.SetDropCallback(_window, dropCallback);
            Glfw.SetCursorPositionCallback(_window, cursorPositionCallback);
            Glfw.SetCursorEnterCallback(_window, cursorEnterCallback);
            Glfw.SetMouseButtonCallback(_window, mouseButtonCallback);
            Glfw.SetScrollCallback(_window, scrollCallback);
            Glfw.SetCharModsCallback(_window, charModsCallback);
            Glfw.SetFramebufferSizeCallback(_window, framebufferSizeCallback);
            Glfw.SetWindowRefreshCallback(_window, windowRefreshCallback);
            Glfw.SetKeyCallback(_window, keyCallback);
            Glfw.SetWindowMaximizeCallback(_window, windowMaximizeCallback);
            Glfw.SetWindowContentScaleCallback(_window, windowContentScaleCallback);
        }*/
        /*

         #region Delegates and Events
         public event EventHandler<ContentScaleEventArgs> ContentScaleChanged;
         protected virtual void OnContentScaleChanged(float xScale, float yScale)
         {
             ContentScaleChanged?.Invoke(this, new ContentScaleEventArgs(xScale, yScale));
         }
         
         /// <summary>
         ///     Occurs when the window is maximized or restored.
         /// </summary>
         public event EventHandler<MaximizeEventArgs> MaximizeChanged;
         protected virtual void OnMaximizeChanged(bool maximized)
         {
             MaximizeChanged?.Invoke(this, new MaximizeEventArgs(maximized));
         }


        /// <summary>
        ///     Occurs when the window receives character input.
        /// </summary>
        public event EventHandler<CharEventArgs> CharacterInput;

        /// <summary>
        ///     Occurs when the window is closed.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        ///     Occurs when the form is closing, and provides subscribers means of canceling the action..
        /// </summary>
        public event CancelEventHandler Closing;

        /// <summary>
        ///     Occurs when the window is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        ///     Occurs when files are dropped onto the window client area with a drag-drop event.
        /// </summary>
        public event EventHandler<FileDropEventArgs> FileDrop;

        /// <summary>
        ///     Occurs when the window gains or loses focus.
        /// </summary>
        public event EventHandler FocusChanged;

        /// <summary>
        ///     Occurs when the size of the internal framebuffer is changed.
        /// </summary>
        public event EventHandler<SizeChangeEventArgs> FramebufferSizeChanged;

        /// <summary>
        ///     Occurs when a key is pressed, released, or repeated.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyAction;

        /// <summary>
        ///     Occurs when a key is pressed.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyPress;

        /// <summary>
        ///     Occurs when a key is released.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyRelease;

        /// <summary>
        ///     Occurs when a key is held long enough to raise a repeat event.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyRepeat;

        /// <summary>
        ///     Occurs when a mouse button is pressed or released.
        /// </summary>
        public event EventHandler<MouseButtonEventArgs> MouseButton;

        /// <summary>
        ///     Occurs when the mouse cursor enters the client area of the window.
        /// </summary>
        public event EventHandler MouseEnter;

        /// <summary>
        ///     Occurs when the mouse cursor leaves the client area of the window.
        /// </summary>
        public event EventHandler MouseLeave;

        /// <summary>
        ///     Occurs when mouse cursor is moved.
        /// </summary>
        public event EventHandler<MouseMoveEventArgs> MouseMoved;

        /// <summary>
        ///     Occurs when mouse is scrolled.
        /// </summary>
        public event EventHandler<MouseMoveEventArgs> MouseScroll;

        /// <summary>
        ///     Occurs when position of the <see cref="NativeWindow" /> is changed.
        /// </summary>
        public event EventHandler PositionChanged;

        /// <summary>
        ///     Occurs when window is refreshed.
        /// </summary>
        public event EventHandler Refreshed;

        /// <summary>
        ///     Occurs when size of the <see cref="NativeWindow" /> is changed.
        /// </summary>
        public event EventHandler<SizeChangeEventArgs> SizeChanged;

        /// <summary>
        ///     Raises the <see cref="CharacterInput" /> event.
        /// </summary>
        /// <param name="codePoint">The Unicode code point.</param>
        /// <param name="mods">The modifier keys present.</param>
        protected virtual void OnCharacterInput(uint codePoint, ModifierKeys mods)
        {
            CharacterInput?.Invoke(this, new CharEventArgs(codePoint, mods));
        }

        /// <summary>
        ///     Raises the <see cref="Closed" /> event.
        /// </summary>
        protected virtual void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises the <see cref="Closing" /> event.
        /// </summary>
        protected virtual void OnClosing()
        {
            var args = new CancelEventArgs();
            Closing?.Invoke(this, args);
            if (args.Cancel)
            {
                Glfw.SetWindowShouldClose(_window, false);
            }
            else
            {
//                base.Close();
                OnClosed();
            }
        }
        
        public static string PtrToStringUTF8(IntPtr ptr)
        {
            var length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
                length++;
            var buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }
        
        private void OnFileDrop(int count, IntPtr pointer)
        {
            var paths = new string[count];
            var offset = 0;
            for (var i = 0; i < count; i++, offset += IntPtr.Size)
            {
                var ptr = new IntPtr(Marshal.ReadInt32(pointer + offset));
                paths[i] = PtrToStringUTF8(ptr);
            }

            OnFileDrop(paths);
        }

        /// <summary>
        ///     Raises the <see cref="FileDrop" /> event.
        /// </summary>
        /// <param name="paths">The filenames of the dropped files.</param>
        protected virtual void OnFileDrop(string[] paths)
        {
            FileDrop?.Invoke(this, new FileDropEventArgs(paths));
        }

        /// <summary>
        ///     Raises the <see cref="FocusChanged" /> event.
        /// </summary>
        /// <param name="focusing"><c>true</c> if window is gaining focus, otherwise <c>false</c>.</param>
        // ReSharper disable once UnusedParameter.Global
        protected virtual void OnFocusChanged(bool focusing)
        {
            FocusChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises the <see cref="FramebufferSizeChanged" /> event.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        protected virtual void OnFramebufferSizeChanged(int width, int height)
        {
            FramebufferSizeChanged?.Invoke(this, new SizeChangeEventArgs(new Size(width, height)));
        }

        /// <summary>
        ///     Raises the appropriate key events.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="scanCode">The scan code.</param>
        /// <param name="state">The state of the key.</param>
        /// <param name="mods">The modifier keys.</param>
        /// <seealso cref="KeyPress" />
        /// <seealso cref="KeyRelease" />
        /// <seealso cref="KeyRepeat" />
        /// <seealso cref="KeyAction" />
        protected virtual void OnKey(Keys key, int scanCode, InputState state, ModifierKeys mods)
        {
            var args = new KeyEventArgs(key, scanCode, state, mods);
            if (state.HasFlag(InputState.Press))
                KeyPress?.Invoke(this, args);
            else if (state.HasFlag(InputState.Release))
                KeyRelease?.Invoke(this, args);
            else
                KeyRepeat?.Invoke(this, args);
            KeyAction?.Invoke(this, args);
        }

        /// <summary>
        ///     Raises the <see cref="MouseButton" /> event.
        /// </summary>
        /// <param name="button">The mouse button.</param>
        /// <param name="state">The state of the mouse button.</param>
        /// <param name="modifiers">The modifier keys.</param>
        protected virtual void OnMouseButton(MouseButton button, InputState state, ModifierKeys modifiers)
        {
            MouseButton?.Invoke(this, new MouseButtonEventArgs(button, state, modifiers));
        }

        /// <summary>
        ///     Raises the <see cref="MouseEnter" /> and <see cref="MouseLeave" /> events.
        /// </summary>
        /// <param name="entering"><c>true</c> if mouse is entering window, otherwise <c>false</c> if it is leaving.</param>
        protected virtual void OnMouseEnter(bool entering)
        {
            if (entering)
                MouseEnter?.Invoke(this, EventArgs.Empty);
            else
                MouseLeave?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises the <see cref="MouseMoved" /> event.
        /// </summary>
        /// <param name="x">The new x-coordinate of the mouse.</param>
        /// <param name="y">The new y-coordinate of the mouse.</param>
        protected virtual void OnMouseMove(double x, double y)
        {
            MouseMoved?.Invoke(this, new MouseMoveEventArgs(x, y));
        }

        /// <summary>
        ///     Raises the <see cref="MouseScroll" /> event.
        /// </summary>
        /// <param name="x">The amount of the scroll on the x-axis.</param>
        /// <param name="y">The amount of the scroll on the y-axis.</param>
        protected virtual void OnMouseScroll(double x, double y)
        {
            MouseScroll?.Invoke(this, new MouseMoveEventArgs(x, y));
        }

        /// <summary>
        ///     Raises the <see cref="PositionChanged" /> event.
        /// </summary>
        /// <param name="x">The new position on the x-axis.</param>
        /// <param name="y">The new position on the y-axis.</param>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        protected virtual void OnPositionChanged(double x, double y)
        {
            PositionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Raises the <see cref="SizeChanged" /> event.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        protected virtual void OnSizeChanged(int width, int height)
        {
            SizeChanged?.Invoke(this, new SizeChangeEventArgs(new Size(width, height)));
        }

        #endregion*/
    }
}