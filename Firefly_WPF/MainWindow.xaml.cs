using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Firefly;
using FireflyUtility.Structure;
using InputManager = Firefly.InputManager;
using Timer = System.Timers.Timer;

namespace Firefly_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WriteableBitmap WritableBitmap;
        public FireflyApplication Application;
        public PointBitmap PointBitmap;
        private int frameCount;

        public MainWindow()
        {
            InitializeComponent();
            Application = new FireflyApplication();
            
            Height = FireflyApplication.Height;
            Width = FireflyApplication.Width;
            Img.Source = Bitmap2WritableBitmap.Bitmap2BitmapImage(new Bitmap((int) Width, (int) Height));
            WritableBitmap = new WriteableBitmap((BitmapSource)Img.Source);
            PointBitmap = new PointBitmap(new Bitmap((int)Width, (int)Height));
            Application.OnFrameComplete += OnFrameComplete;
            Application.Run();

            Timer timer1 = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timer1.Elapsed += Timer1_Elapsed;

        }
        public static float Range(float v, float min, float max) => v <= min ? min :
            v >= max ? max : v;

        private void OnFrameComplete(RgbaFloat[] buff)
        {
            Img.Dispatcher.BeginInvoke(new Action(() =>
            {
                PointBitmap.LockBits();
                Parallel.For(0, buff.Length, (int i) =>
                {
                    PointBitmap.SetColor(i * 4, (byte)Range(buff[i].B * 255 + 0.5f, 0, 255));
                    PointBitmap.SetColor(i * 4 + 1, (byte)Range(buff[i].G * 255 + 0.5f, 0, 255));
                    PointBitmap.SetColor(i * 4 + 2, (byte)Range(buff[i].R * 255 + 0.5f, 0, 255));
                    PointBitmap.SetColor(i * 4 + 3, 255);
                    
                    buff[i] = RgbaFloat.Black;
                });

                Application.waiting = false;
                //PointBitmap.LockBits();
                Bitmap2WritableBitmap.BitmapToWritableBitmap(WritableBitmap, PointBitmap.UnlockBits());
                Img.Source = WritableBitmap;
            }));
            frameCount++;
        }

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Title = "FireFly FPS: " + frameCount;
                frameCount = 0;
            }));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.CloseRenderThread = true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            InputManager.KeyDown(e.Key);
            if (e.Key != Key.P) return;
            if (!Directory.Exists("./Saved")) Directory.CreateDirectory("./Saved");
            using FileStream fs = new FileStream($"./Saved/{DateTime.Now.Millisecond}.png", FileMode.Create);
            PointBitmap.Source.Save(fs, ImageFormat.Png);
            Debug.WriteLine($"文件 {fs.Name} 保存成功");
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            InputManager.KeyUp(e.Key);
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InputManager.MouseDown();
            Debug.WriteLine(e.GetPosition(e.MouseDevice.Target));
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InputManager.MouseUp();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            InputManager.MouseMove(e.MouseDevice.GetPosition(e.MouseDevice.Target));
        }
    }
}
