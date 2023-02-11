using Common;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenTK
{
    internal class Game : GameWindow
    {
        private int _vertexBufferObject;
        private readonly float[] _vertices =
        {
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.0f,  0.5f, 0.0f
        };
        private Shader _shader;
        public Game(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title)
        {
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
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            //Создал бубер и сохранил дескриптор
            _vertexBufferObject = GL.GenBuffer();

            //Связал буффер.
            //С этого момента любые вызовы буфера, которые мы делаем (для BufferTarget.ArrayBuffer цели),
            //будут использоваться для настройки текущего связанного буфера, который является VertexBufferObject. 
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            //Копирую данные в текущий связанный буфер
            GL.BufferData<float>(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Code goes here.

            Context.SwapBuffers();

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            _shader.Dispose();
        }
    }
}
