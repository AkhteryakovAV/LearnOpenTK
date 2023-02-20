﻿using Common;
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
        private readonly float[] _vertices =
        {
             0.5f,  0.5f, 0.0f,  // top right
             0.5f, -0.5f, 0.0f,  // bottom right
            -0.5f, -0.5f, 0.0f,  // bottom left
            -0.5f,  0.5f, 0.0f   // top left
        };
        uint[] _indices = {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
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

            //Создал объек массива вершин и сохранил дескриптор
            _vertexArrayObject = GL.GenVertexArray();
            //Привязка VAO 
            // С этого момента мы должны привязать / настроить соответствующие VBO(ы) и указатели атрибутов,
            // а затем отменить привязку VAO для последующего использования. Как только мы хотим нарисовать объект,
            // мы просто привязываем VAO к предпочтительным настройкам перед рисованием объекта, и все. 
            GL.BindVertexArray(_vertexArrayObject);

            //Связывание атрибутов вершин
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _elementBufferObject= GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

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
