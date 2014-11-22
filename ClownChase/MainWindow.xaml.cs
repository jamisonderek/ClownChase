using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClownChase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IKinect _kinect;

        private IKinectSensor _sensor;

        public MainWindow()
        {
            _kinect = new Kinect();
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _sensor = _kinect.GetSensor();
            if (!_sensor.Connected)
            {
                ShowStatus(Properties.Resources.KinectNotFound);
                return;
            }

            _sensor.Initialize();

            var start = _sensor.Start();
            ShowStatus(start?Properties.Resources.KinectStarted:Properties.Resources.KinectNotStarted);

            InitializeImage(PersonColor, _sensor.Boundaries);

            _sensor.FrameReady += FrameReady;
        }

        private void InitializeImage(System.Windows.Controls.Image image, Boundaries boundaries)
        {
            var depthWidth = boundaries.DepthWidth;
            var depthHeight = boundaries.DepthHeight;
            var colorWidth = boundaries.ColorRect.Width;
            var colorHeight = boundaries.ColorRect.Height;
            var maskImage = new WriteableBitmap(depthWidth, depthHeight, 96, 96, PixelFormats.Bgra32, null);
            image.Source = new WriteableBitmap(colorWidth, colorHeight, 96, 96, PixelFormats.Bgr32, null);
            image.OpacityMask = new ImageBrush { ImageSource = maskImage };
        }

        private int _frame;
        private DateTime _lastFrame = DateTime.MinValue;
        private void FrameReady(object sender, FrameReadyEventArgs e)
        {
            ColorPixelMask mask = null;

            string message = String.Format("Frame {0}", ++_frame);
            if (_lastFrame != DateTime.MinValue)
            {
                var diff = DateTime.Now - _lastFrame;
                message += String.Format("   {0:00.00} frames/sec", 1000/diff.TotalMilliseconds);
            }
            _lastFrame = DateTime.Now;

            if (e.DepthReceived)
            {
                mask = new ColorPixelMask(e.Data.Boundaries);
                var nearestObject = e.Data.PopulateColorPixelMask(mask, e.Mapper, (i, i1) => Math.Abs(i-i1)<400);
                message += String.Format(" @{0}/{1}/{2}[{3}]", nearestObject.MinX, nearestObject.NearestX, nearestObject.MaxX, nearestObject.Depth);
            }

            if (e.ColorReceived)
            {
                var image = PersonColor;

                var sourceBitmap = image.Source as WriteableBitmap;
                Debug.Assert(null != sourceBitmap);

                var maskBitmap = ((ImageBrush)image.OpacityMask).ImageSource as WriteableBitmap;
                Debug.Assert(null != maskBitmap);

                sourceBitmap.WritePixels(e.Data.Boundaries.ColorRect, e.Data.ColorPixels, sourceBitmap.PixelWidth * sizeof(int), 0);
                if (mask != null)
                {
                    maskBitmap.WritePixels(e.Data.Boundaries.DepthRect, mask.Mask, e.Data.Boundaries.DepthRect.Width * ((maskBitmap.Format.BitsPerPixel + 7) / 8), 0);                    
                }
            }

            ShowStatus(message);
        }

        public void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _sensor.Stop();
        }

        private void ShowStatus(string message)
        {
            StatusBarText.Text = message;
        }
    }
}
