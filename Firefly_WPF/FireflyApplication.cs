using System;
using System.Drawing;
using System.Timers;
using FireflyUtility.Structure;
using ShaderGen;
using System.Diagnostics;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Firefly_WPF;
using FireflyUtility.Math;
using FireflyUtility.Renderable;
using ShaderLib;

namespace Firefly
{
    public delegate void OnFrameComplete(RgbaFloat[] buff);

    public delegate void OnFrameCopyComplete();

    public class FireflyApplication
    {
        public const int Width = 512;
        public const int Height = 512;
        public const uint ViewScale = 1;

        private Color32 _color;
        private int _frameCount;

        public OnFrameComplete OnFrameComplete;
        public OnFrameCopyComplete OnFrameCopyComplete;
        public Thread RenderThread;
        public bool CloseRenderThread;

        public bool waiting = false;

        public void Run()
        {
            _color = new Color32(0, 0, 0);

            Renderer.InitRender(_color, Width, Height, RenderType.GouraudShading);
            Debug.Write("加载场景:Scene1...");
            Renderer.LoadScene("Scene1");
            Debug.WriteLine("完成");

            Debug.Write("编译Shader...");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<string> paths = new List<string>();
            foreach (KeyValuePair<string, Material> item in Renderer.Materials)
                paths.Add($"Shaders/{item.Value.ShaderName}.cs");
            ShaderGenerator.CompleShader(paths);
            stopwatch.Stop();
            Debug.WriteLine($"完成，耗时: {stopwatch.ElapsedMilliseconds} 毫秒");
            
            Renderer.DelegateCollections = ShaderGenerator.DelegateCollections;
            Renderer.ShaderInformation = ShaderGenerator.ShaderInformation;
            Renderer.InitMaterials();
            
            //Thread.CurrentThread.Resume();
            RenderThread = new Thread(RenderFrames);
            RenderThread.Start();
        }

        private void RenderFrames()
        {
            while (true)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                RgbaFloat[] buff = Renderer.Draw();
                Renderer.ClearDepth();
                OnFrameComplete.Invoke(buff);
                if(CloseRenderThread) return;
                while (waiting)
                {
                    Thread.Sleep(1);
                }

                stopwatch.Stop();
                float deltaTime = stopwatch.ElapsedMilliseconds / 1000.0f;

                int speed = 1;
                Camera camera = Renderer.CurrentScene.Camera;
                if (InputManager.IsMouseDown())
                {
                    float scale = 0.03f;
                    Vector2 move = InputManager.GetMouseMove();
                    camera.Rotation = Mathf.Rotate(camera.Rotation, move.X * scale, -move.Y * scale, 0);
                }

                InputManager.ClearMouseMove();

                Matrix4x4 rotationMatrix = Matrixs.GetRotationMatrix(camera.Rotation);
                if (InputManager.IsKeyPressed(Key.W))
                {
                    camera.Position += ShaderMath.Mul(rotationMatrix, new Vector4(0, 0, -speed * deltaTime, 0)).XYZ();
                }
                if (InputManager.IsKeyPressed(Key.A))
                {
                    camera.Position += ShaderMath.Mul(rotationMatrix, new Vector4(speed * deltaTime, 0, 0, 0)).XYZ();
                }
                if (InputManager.IsKeyPressed(Key.S))
                {
                    camera.Position += ShaderMath.Mul(rotationMatrix, new Vector4(0, 0, speed * deltaTime, 0)).XYZ();
                }
                if (InputManager.IsKeyPressed(Key.D))
                {
                    camera.Position += ShaderMath.Mul(rotationMatrix, new Vector4(-speed * deltaTime, 0, 0, 0)).XYZ();
                }
                if (InputManager.IsKeyPressed(Key.Space))
                {
                    camera.Position += ShaderMath.Mul(rotationMatrix, new Vector4(0, speed * deltaTime, 0, 0)).XYZ();
                }
                if (InputManager.IsKeyPressed(Key.LeftShift))
                {
                    camera.Position += ShaderMath.Mul(rotationMatrix, new Vector4(0, -speed * deltaTime, 0, 0)).XYZ();
                }


                waiting = true;
            }
        }

        private void OnCopyComplete()
        {

        }
    }
}
