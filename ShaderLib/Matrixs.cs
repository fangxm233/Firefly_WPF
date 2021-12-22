using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ShaderLib
{
    public class Matrixs
    {
        /// 模型*观察*投影矩阵，用于将顶点/方向矢量从模型空间变换到裁切空间
        public static Matrix4x4 MVP;
        /// 模型*观察矩阵，用于将顶点/方向矢量从模型空间变换到观察空间
        public static Matrix4x4 MV;
        /// 观察矩阵，用于将顶点/方向矢量从世界空间变换到观察空间
        public static Matrix4x4 V;
        /// 投影矩阵，用于将顶点/方向矢量从观察空间变换到裁切空间
        public static Matrix4x4 P;
        /// 观察*投影矩阵， 用于将顶点/方向矢量从世界空间变换到裁切空间
        public static Matrix4x4 VP;
        /// MV的转置矩阵
        public static Matrix4x4 T_MV;
        /// MV的逆转置矩阵，用于将法线从模型空间变换到观察空间
        public static Matrix4x4 IT_MV;
        /// 模型矩阵，用于将顶点/法线从模型空间变换到世界空间
        public static Matrix4x4 Entity2World;
        /// Entity2World的逆矩阵，用于将顶点/法线从世界空间变换到模型空间
        public static Matrix4x4 World2Entity;

        public static Matrix4x4 O;

        public static Vector3 CameraPosition, EntityPosition;
        public static Quaternion EntityRotation, CameraRotation;
        public static float FOV, Aspect, Near, Far, Size;
        public static float Width, Height;

        public static void CalculateMatrixs()
        {
            Entity2World = CreateTranslation(EntityPosition) * GetRotationMatrix(EntityRotation);
            Matrix4x4.Invert(Entity2World, out World2Entity);
            Matrix4x4.Invert(GetRotationMatrix(CameraRotation), out Matrix4x4 invertedRotationMatrix);
            V = invertedRotationMatrix * CreateTranslation(-CameraPosition);
            P = PerspectiveFieldOfView(FOV, Aspect, Near, Far);

            //P = Matrix4x4.Transpose(Matrix4x4.CreatePerspectiveFieldOfView(FOV, Aspect, Near, Far));
            O = GetOrthographic(Size, Aspect, Near, Far);
            //O = Matrix4x4.CreateOrthographic(Size * Aspect, Size / Aspect, Near, Far);
            //P = O;
            MV = V * Entity2World;
            T_MV = Matrix4x4.Transpose(MV);
            Matrix4x4.Invert(T_MV, out IT_MV);
            VP = P * V;
            MVP = P * (V * Entity2World);
        }

        public static Matrix4x4 GetRotationMatrix(Quaternion q)
        {
            //var fff = Matrix4x4.Transpose(Matrix4x4.CreateFromQuaternion(q));
            //return GetRotationY(v.Y) * GetRotationX(v.X) * GetRotationZ(v.Z);
            return Matrix4x4.Transpose(Matrix4x4.CreateFromQuaternion(q));
        }
        //=>
        //GetRotationY(v.Y) * GetRotationX(v.X) * GetRotationZ(v.Z);

        private static Matrix4x4 PerspectiveFieldOfView(float fov, float aspect, float near, float far) =>
            new Matrix4x4(1 / (MathF.Tan(fov * 0.5f) * aspect), 0, 0, 0, 0, 1 / MathF.Tan(fov * 0.5f), 
                0, 0, 0, 0, far / (far - near), 1, 0, 0, (near * far) / (near - far), 0);
            //new Matrix4x4(MathF.Atan(fov / 2) / aspect, 0, 0, 0, 0, MathF.Atan(fov / 2), 0, 0,
            //    0, 0, -(far + near) / (far - near), -(2 * near * far) / (far - near), 0, 0, -1, 0);

        private static Matrix4x4 GetOrthographic(float size, float aspect, float near, float far) =>
            new Matrix4x4(1 / (aspect * size), 0, 0, 0, 0, 1 / size, 0, 0,
                0, 0, -2 / (far - near), -(far + near) / (far - near), 0, 0, 0, 1);

        private static Matrix4x4 GetRotationX(float a) =>
            new Matrix4x4(1, 0, 0, 0, 0, MathF.Cos(a), MathF.Sin(a), 0, 0, -MathF.Sin(a), MathF.Cos(a), 0, 0, 0, 0, 1);

        private static Matrix4x4 GetRotationY(float a) =>
            new Matrix4x4(MathF.Cos(a), 0, -MathF.Sin(a), 0, 0, 1, 0, 0, MathF.Sin(a), 0, MathF.Cos(a), 0, 0, 0, 0, 1);

        private static Matrix4x4 GetRotationZ(float a) =>
            new Matrix4x4(MathF.Cos(a), MathF.Sin(a), 0, 0, -MathF.Sin(a), MathF.Cos(a), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        public static Matrix4x4 CreateTranslation(Vector3 position)
        {
            Matrix4x4 result;

            result.M11 = 1.0f;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M41 = 0.0f;
            result.M21 = 0.0f;
            result.M22 = 1.0f;
            result.M23 = 0.0f;
            result.M42 = 0.0f;
            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = 1.0f;
            result.M43 = 0.0f;

            result.M14 = position.X;
            result.M24 = position.Y;
            result.M34 = position.Z;
            result.M44 = 1.0f;

            return result;
        }
    }
}
