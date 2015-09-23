using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Windows.Threading;

namespace TestJoint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        KinectSensor _sensor;
        Skeleton[] _bodies = new Skeleton[6];
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.KinectSensors.Where(s => s.Status == KinectStatus.Connected).FirstOrDefault();

            if (_sensor != null)
            {
                _sensor.ColorStream.Enable();
                _sensor.DepthStream.Enable();
                _sensor.SkeletonStream.Enable();

                _sensor.AllFramesReady += Sensor_AllFramesReady;

                _sensor.Start();
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Start();
            //timer.Tick += new EventHandler(timer_Tick);
        }
        /*
        void timer_Tick(object sender, EventArgs e)
        {
            Canvas.SetTop(circle, new Random().Next(0, (int)this.Height));
            Canvas.SetLeft(circle, new Random().Next(0, (int)this.Width));
        }
        */
        void Sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
   
            using (var frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    
                        image.Source = frame.ToBitmap();
                    
                }
            }

           
            // Body
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    frame.CopySkeletonDataTo(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            // COORDINATE MAPPING
                            foreach (Joint joint in body.Joints)
                            {
                                // 3D coordinates in meters
                                SkeletonPoint skeletonPoint = joint.Position;

                                Joint myJoint2 = body.Joints[JointType.WristLeft]; //the joint I want to compare
                                Joint myJoint = body.Joints[JointType.WristRight]; //the joint I want to compare

                                // 2D coordinates in pixels
                                Point point = new Point();
                                Point point2 = new Point();


                                // Skeleton-to-Color mapping
                                ColorImagePoint colorPoint = _sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);

                                point.X = colorPoint.X;
                                point.Y = colorPoint.Y;
                                point2.X = colorPoint.X;
                                point2.Y = colorPoint.Y;

                                Rectangle rectangleRightWrist = new Rectangle
                                {
                                    Fill = Brushes.DarkRed,
                                    Width = 100,
                                    Height = 20
                                };

                                Rectangle rectangleLeftWrist = new Rectangle
                                {
                                    Fill = Brushes.Black,
                                    Width = 80,
                                    Height = 20
                                };
                                if (joint == myJoint)
                                {
                                    Canvas.SetLeft(sup, point.X - 10);
                                    Canvas.SetTop(sup, point.Y - 10);
                                     //canvas.Children.Add(rectangleRightWrist);
                     
                                }
                                
                                else if (joint == myJoint2)
                                {
                                    Canvas.SetLeft(Shiryu, point2.X - 30);
                                    Canvas.SetTop(Shiryu, point2.Y -30);
                                    // canvas.Children.Add(Shiryu);
                                    yPosition.Content = "y: " + point2.Y;
                                    xPosition.Content = "x: " + point2.X;

                                }


                                /*
                                if (volumeBool)//es la parte de dibujar
                                {
                                    // Skeleton-to-Color mapping
                                    ColorImagePoint colorPoint = _sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);

                                    point.X = colorPoint.X;
                                    point.Y = colorPoint.Y;


                                    

                                    Ellipse ellipseRightWrist = new Ellipse
                                    {
                                        Fill = Brushes.LemonChiffon,
                                        Width = 40,
                                        Height = 40
                                    };



                                    //Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                                    //Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                                    if (joint == myJoint)
                                    {
                                        Canvas.SetLeft(ellipseRightWrist, point.X - ellipseRightWrist.Width / 2);
                                        Canvas.SetTop(ellipseRightWrist, point.Y - ellipseRightWrist.Height / 2);
                                        canvas.Children.Add(ellipseRightWrist);
                                        scrollBar.Value = (float)point.X / (scrollBar.Width);
                                    }

                                    xCoord.Content = point.X.ToString();
                                    yCoord.Content = point.Y.ToString();

                                    // canvas.Children.Add(ellipse);
                                }*/
                            }
                        }
                    }
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_sensor != null)
            {
                _sensor.Stop();
            }
        }

      



    }


}
