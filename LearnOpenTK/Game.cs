using Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace LearnOpenTK
{
    internal class Game : GameWindow
    {
        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;
        private readonly float[] _vertices =
        {
            0.5f, -0.5f, 0.5f,
            0.5f, 0.5f, 0.5f,
            -0.5f, 0.5f, 0.5f,
            -0.5f, -0.5f, 0.5f,

            0.5f, -0.5f, -0.5f,
            0.5f, 0.5f, -0.5f,
            -0.5f, 0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
        };
        uint[] _indices = {
            0, 1, 2,
            0, 2, 3,
            0, 1, 4,
            1, 4, 5,
            0, 4, 3,
            3, 4, 7,
            3, 7, 6,
            3, 2, 6,
            4, 5, 6,
            4, 6, 7,
            1, 2, 5,
            2, 5, 6,
        };

        private double _time;

        private Shader _shader;

        //Матрицы просмотра и проекции были удалены,
        //поскольку они нам здесь больше не нужны.
        //Теперь их можно найти в новом классе камер.

        //Нам нужен экземпляр нового класса camera,
        //чтобы он мог управлять кодом матрицы просмотра и проекции.
        //Нам также нужно логическое значение, установленное в true, чтобы определить,
        //была ли мышь перемещена в первый раз или нет.
        //Наконец, мы добавляем последнее положение мыши, чтобы мы могли легко вычислить смещение мыши.
        private Camera _camera;
        private bool _firstMove = true;
        private Vector2 _lastPos;


        public Game(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        { }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if (!Focused) // Check to see if the window is focused
            {
                return;
            }
            KeyboardState input = Keyboard.GetState();
            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Key.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Key.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Key.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Key.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Key.ShiftLeft))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = Mouse.GetState();

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            _vertexBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
            _elementBufferObject = GL.GenBuffer();
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            // 1. Привязываем VAO
            GL.BindVertexArray(_vertexArrayObject);
            // 2. Копируем наши вершины в буфер для OpenGL
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            // 3. Копируем наши индексы в в буфер для OpenGL
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            // 3. Устанавливаем указатели на вершинные атрибуты
            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            _shader.Use();

            // Мы инициализируем камеру так, чтобы она находилась на расстоянии 3 единиц от того места,
            // где находится прямоугольник. Мы также придаем ему правильное соотношение сторон.
            _camera = new Camera(Vector3.UnitZ * 3, Width / (float)Height);

            // Мы делаем курсор мыши невидимым и фиксируемым, чтобы у нас было правильное движение камеры с частотой кадров в секунду.
            CursorVisible = false;
            CursorGrabbed = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _time += 40.0 * e.Time;
            var model = Matrix4.Identity *
                Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time)) *
                Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time * Math.Sin(MathHelper.DegreesToRadians(_time))));

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _shader.Use();

            SwapBuffers();
        }
        // In the mouse wheel function, we manage all the zooming of the camera.
        // This is simply done by changing the FOV of the camera.
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.Mouse.ScrollWheelValue;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
            _camera.AspectRatio = Width / (float)Height;
        }

        protected override void OnUnload(EventArgs e)
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            _shader.Dispose();

            base.OnUnload(e);
        }
    }
}
