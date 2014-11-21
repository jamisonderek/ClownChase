using System.Windows;
using Microsoft.Kinect;

namespace ClownChase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKinect _kinect;
        private KinectSensor _sensor;

        public MainWindow()
        {
            _kinect = new Kinect();
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _sensor = _kinect.GetSensor();
            if (_sensor == null)
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
