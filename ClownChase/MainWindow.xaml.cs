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

            ShowStatus(Properties.Resources.KinectFound);
        }

        private void ShowStatus(string message)
        {
            StatusBarText.Text = message;
        }
    }
}
