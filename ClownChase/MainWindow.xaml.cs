﻿using System;
using System.Windows;

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

            _sensor.FrameReady += FrameReady;
        }

        private int _frame;
        private DateTime _lastFrame = DateTime.MinValue;
        private void FrameReady(object sender, FrameReadyEventArgs e)
        {
            string message = String.Format("Frame {0}", ++_frame);
            if (_lastFrame != DateTime.MinValue)
            {
                var diff = DateTime.Now - _lastFrame;
                message += String.Format("   {0:00.00} frames/sec", 1000/diff.TotalMilliseconds);
            }
            _lastFrame = DateTime.Now;
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
