using Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

namespace LearnOpenTK
{
    internal class Game : GameWindow
    {
        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;

        //Кол-во вершин на окружности
        private readonly int _vertexCount = 50;

        private float[] _vertices;
        uint[] _indices;

        private Shader _shader;
        public Game(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        {
        }
        private void InitializeCircle()
        {
            _vertices = new float[(_vertexCount + 1) * 3]; // +1 центральная вершина
            _vertices[0] = 0.0f;
            _vertices[1] = 0.0f;
            _vertices[2] = 0.0f;
            double alpha = 2*Math.PI / _vertexCount;
            uint elementIndex = 0;
            for (int i = 3; i < _vertices.Length;)
            {
                //Единичная окружность
                double currentDeg = alpha * elementIndex;
                _vertices[i] = (float)Math.Cos(currentDeg);
                _vertices[i + 1] = (float)Math.Sin(currentDeg);
                _vertices[i + 2] = 0.0f;
                i += 3;
                elementIndex++;
            }

            _indices = new uint[_vertexCount * 3];
            elementIndex = 0;
            for (uint i = 0; i < _indices.Length - 3;)
            {
                _indices[i] = 0;
                _indices[i + 1] = elementIndex + 1;
                _indices[i + 2] = elementIndex + 2;
                i += 3;
                elementIndex++;
            }
            _indices[_indices.Length - 3] = 0;
            _indices[_indices.Length - 2] = elementIndex + 1;
            _indices[_indices.Length - 1] = 1;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Key.Escape))
            {
                Exit();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeCircle();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

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
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _shader.Use();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
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
