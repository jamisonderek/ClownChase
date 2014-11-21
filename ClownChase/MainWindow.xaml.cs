using System.Windows;

namespace ClownChase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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
            if (start)
            {
                ShowStatus(Properties.Resources.KinectStarted);                                
            }
            else
            {
                ShowStatus(Properties.Resources.KinectNotStarted);                
            }
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
