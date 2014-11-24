using System;
using System.Windows;
using System.Windows.Input;
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
        private IFrameProcessor _greenScreen;
        private IKinectSensor _sensor;

        public MainWindow()
        {
            _kinect = new Kinect();
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _sensor = _kinect.GetSensor();
            
            var mask = new ConnectedToNearestMask();
            _greenScreen = new GreenScreenFrameProcessor(PersonColor, mask);

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

        public void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _sensor.Stop();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {

            }
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

        private DateTime _lastFrame = DateTime.MinValue;
        private int _frame;
        private void FrameReady(object sender, FrameReadyEventArgs e)
        {
            var message = _greenScreen.ProcessFrame(e);
            
            message = String.Format("Frame {0} {1}", ++_frame, message);
            if (_lastFrame != DateTime.MinValue)
            {
                var diff = DateTime.Now - _lastFrame;
                message += String.Format(" {0:00.00} frames/sec", 1000 / diff.TotalMilliseconds);
            }
            _lastFrame = DateTime.Now;
            ShowStatus(message);
        }

        private void ShowStatus(string message)
        {
            StatusBarText.Text = message;
        }
    }
}
