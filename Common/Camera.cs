using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Camera
    {
        // Эти векторы представляют собой направления, направленные наружу от камеры, чтобы определить, как она вращалась.
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        // Вращение вокруг оси X (в радианах). Тангаж
        private float _pitch;

        // Вращение вокруг оси Y (в радианах). Рыскание
        private float _yaw = -MathHelper.PiOver2; // Без этого вы бы начали поворачиваться на 90 градусов вправо.

        // Поле зрения камеры (в радианах)
        private float _fov = MathHelper.PiOver2;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        // Положение камеры
        public Vector3 Position { get; set; }

        // Это просто соотношение сторон видового экрана (viewport), используемое для матрицы проекции.
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        // Мы преобразуем градусы в радианы, как только свойство настроено для повышения производительности.
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // Мы устанавливаем значение высоты тона между -89 и 89,
                // чтобы предотвратить переворачивание камеры вверх ногами и кучу странных "багов", когда вы используете углы Эйлера для поворота.
                // Если вы хотите узнать больше об этом, вы можете попробовать изучить тему под названием gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // Мы преобразуем градусы в радианы, как только свойство настроено для повышения производительности.
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        // Поле зрения (FOV) - это вертикальный угол обзора камеры.
        // В этом руководстве вы также узнали, как мы можем использовать это для имитации функции масштабирования.
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // Получите матрицу просмотра, используя удивительную функцию LookAt, более подробно описанную в веб-руководствах
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        // Получите матрицу проекции, используя тот же метод, который мы использовали до этого момента
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        // Эта функция собирается обновить вершины направления, используя некоторые математические методы, изученные в веб-руководствах.
        private void UpdateVectors()
        {
            // Во-первых, передняя матрица вычисляется с использованием некоторой базовой тригонометрии.
            _front.X = (float)(Math.Cos(_pitch) * Math.Cos(_yaw));
            _front.Y = (float)Math.Sin(_pitch);
            _front.Z = (float)(Math.Cos(_pitch) * Math.Sin(_yaw));

            // Нам нужно убедиться, что все векторы нормализованы, так как в противном случае мы получили бы несколько обалденных результатов.
            _front = Vector3.Normalize(_front);

            // Вычислите как правый, так и восходящий вектор, используя перекрестное произведение.
            // Обратите внимание, что мы рассчитываем справа от глобального значения;
            // такое поведение может быть не тем, что вам нужно для всех камер, поэтому имейте это в виду,
            // если вам не нужна камера FPS.
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}
