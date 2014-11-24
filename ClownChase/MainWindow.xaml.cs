using System;
using System.Media;
using System.Threading;
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
        private ICaptureFrameCollection _captureFrameCollection;
        private IFrameProcessor _greenScreenMask;
        private IFrameProcessor _greenScreenColor;
        private IFrameProcessor _captureFrames;
        private IFrameProcessor _renderFrame;
        private IKinectSensor _sensor;

        public MainWindow()
        {
            _kinect = new Kinect();
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _sensor = _kinect.GetSensor();

            _captureFrameCollection = new CaptureFrameCollection();
            var mask = new ConnectedToNearestMask();
            _greenScreenMask = new GreenScreenMaskFrameProcessor(mask);
            _greenScreenColor = new GreenScreenColorFrameProcessor(PersonColor);
            _captureFrames = new CaptureFrameProcessor(_captureFrameCollection);
            _renderFrame = new RenderFrameProcessor(_captureFrameCollection, ClownColor);

            if (!_sensor.Connected)
            {
                ShowStatus(Properties.Resources.KinectNotFound);
                return;
            }

            _sensor.Initialize();

            var start = _sensor.Start();
            ShowStatus(start?Properties.Resources.KinectStarted:Properties.Resources.KinectNotStarted);

            InitializeImage(PersonColor, _sensor.Boundaries);
            InitializeImage(ClownColor, _sensor.Boundaries);

            _sensor.FrameReady += FrameReady;
        }

        public void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _sensor.Stop();
        }

        private bool _renderClown;
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.B)
            {
                ((CaptureFrameProcessor) _captureFrames).Capture = true;
                SystemSounds.Beep.Play();
                Thread.Sleep(1000);
                SystemSounds.Beep.Play();
                Thread.Sleep(1000);
                SystemSounds.Beep.Play();
                Thread.Sleep(1000);
            }
            if (e.Key == Key.E)
            {
                ((CaptureFrameProcessor)_captureFrames).Capture = false;
            }
            if (e.Key == Key.C)
            {
                _renderClown = true;
            }
            if (e.Key == Key.S)
            {
                var storage = new Storage();
                storage.Save(_captureFrameCollection);
            }
            if (e.Key == Key.L)
            {
                var storage = new Storage();
                storage.Load(_captureFrameCollection);
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
            if ((!e.ColorReceived || !e.DepthReceived))
                return;

            var message = _greenScreenMask.ProcessFrame(e);
            message += _greenScreenColor.ProcessFrame(e);
            message += _captureFrames.ProcessFrame(e);
            if (_renderClown)
            {
                message += _renderFrame.ProcessFrame(e);
            }
            
            message = String.Format("Frame {0} {1}", ++_frame, message);
            if (_lastFrame != DateTime.MinValue)
            {
                var diff = DateTime.Now - _lastFrame;
                message += String.Format(" {0:00.00} f/sec", 1000 / diff.TotalMilliseconds);
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
