using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Shader
    {
        private bool _disposedValue = false;

        public Shader(string vertexPath, string fragmentPath)
        {
            CreateShaderItem(vertexPath, ShaderType.VertexShader, out int vertexShader);
            CreateShaderItem(fragmentPath, ShaderType.FragmentShader, out int fragmentShader);

            CompileShader(vertexShader);
            CompileShader(fragmentShader);

            AttachShaders(new int[] { vertexShader, fragmentShader });

            //Отдельные вершинные и фрагментные шейдеры теперь бесполезны, поскольку они связаны;
            //скомпилированные данные копируются в шейдерную программу при их связывании. 
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public int Handle { get; set; }

        /// <summary>
        /// Связывает отдельные шейдеры в одну программу, которую можно запускать на графическом процессоре.
        /// </summary>
        /// <param name="shaders"></param>
        /// <exception cref="Exception"></exception>
        private void AttachShaders(int[] shaders)
        {
            Handle = GL.CreateProgram();
            foreach (var shader in shaders)
            {
                GL.AttachShader(Handle, shader);
            }
            GL.LinkProgram(Handle);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                throw new Exception(infoLog);
            }
        }

        /// <summary>
        /// Создание шейдера
        /// </summary>
        /// <param name="path">Путь к файлу шейдера</param>
        /// <param name="shader">Указатель на скомпилированный шейдер</param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        private static void CreateShaderItem(string path, ShaderType shaderType, out int shader)
        {
            if (File.Exists(path))
            {
                string shaderSource = File.ReadAllText(path);
                if (string.IsNullOrEmpty(shaderSource))
                {
                    throw new Exception("Файл шейдера пустой");
                }
                //генерируем шейдеры 
                shader = GL.CreateShader(shaderType);
                //Привязываем исходный код к шейдерам.
                GL.ShaderSource(shader, shaderSource);
            }
            else
            {
                throw new FileNotFoundException(path);
            }
        }

        /// <summary>
        /// Компилируем шейдеры и проверяем их на наличие ошибок.
        /// </summary>
        /// <param name="shader">Указатель на скомпилированный шейдер</param>
        /// <exception cref="Exception">При ошибки компиляции</exception>
        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Не удалось скомпилировать шейдер: {nameof(shader)}.{Environment.NewLine}Лог: {infoLog}");
            }
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                GL.DeleteProgram(Handle);
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
