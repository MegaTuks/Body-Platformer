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
using System.Media;


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
        int CharPos = 100;
        int CharPosY = 0;
        int DistanciaX = 1;
        SoundPlayer backgroundM;

        public MainWindow()
        {
            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.KinectSensors.Where(s => s.Status == KinectStatus.Connected).FirstOrDefault();
            backgroundM = new SoundPlayer(Properties.Resources.Megaman_Rockman_X_Intro_Highway_Stage);
            backgroundM.PlayLooping();
            if (_sensor != null)
            {
                _sensor.ColorStream.Enable();
                _sensor.DepthStream.Enable();
                _sensor.SkeletonStream.Enable();

                _sensor.AllFramesReady += Sensor_AllFramesReady;

                _sensor.Start();
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            timer.IsEnabled = true;
            timer.Start();
            timer.Tick += new EventHandler(timer_Tick);
        }
        void timer_Tick(object sender, EventArgs e)
        {


            CharPos = int.Parse(Avatar.GetValue(Canvas.LeftProperty).ToString());
            if (CharPos >= 560 || CharPos == -1)
            {
                
                DistanciaX *= -1;
            }

            //if(Personaje.po)
            int PerTop = int.Parse(Avatar.GetValue(Canvas.TopProperty).ToString());
            int PlatTop = int.Parse(leftPlatform.GetValue(Canvas.TopProperty).ToString());
            int PerLeft = int.Parse(Avatar.GetValue(Canvas.LeftProperty).ToString());
            int PlatLeft = int.Parse(leftPlatform.GetValue(Canvas.LeftProperty).ToString());
            int PlatTop2 = int.Parse(rightPlatform.GetValue(Canvas.TopProperty).ToString());
            int PlatLeft2 = int.Parse(rightPlatform.GetValue(Canvas.LeftProperty).ToString());
            int ground1 = int.Parse(Floor1.GetValue(Canvas.TopProperty).ToString());
            int leftGround1 = int.Parse(Floor1.GetValue(Canvas.LeftProperty).ToString());
            int door = int.Parse(Door.GetValue(Canvas.TopProperty).ToString());
            int leftDoor = int.Parse(Door.GetValue(Canvas.LeftProperty).ToString());


            if (PerTop < PlatTop && PerTop > PlatTop - Avatar.ActualHeight && PerLeft < PlatLeft + (int)leftPlatform.Width && PerLeft >= PlatLeft)
            {
                CharPos += DistanciaX;
                CharPosY = PlatTop - (int)Avatar.ActualHeight;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else if (PerTop < PlatTop2 && PerTop > PlatTop2 - Avatar.ActualHeight && PerLeft < PlatLeft2 + (int)rightPlatform.Width && PerLeft >= PlatLeft2)
            {
                CharPos += DistanciaX;
                CharPosY = PlatTop2 - (int)Avatar.ActualHeight;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else if (PerTop < ground1 && PerTop > ground1 - Avatar.ActualHeight && PerLeft < leftGround1 + (int)Floor1.Width && PerLeft >= leftGround1)
            {
                CharPos += DistanciaX;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else if (PerTop < door && PerTop > door - Avatar.ActualHeight && PerLeft < leftDoor + (int)Door.Width && PerLeft >= leftDoor)
            {
                CharPos += DistanciaX;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else
            {
                CharPosY += 1;
                Canvas.SetTop(Avatar, CharPosY);
            }
            xPosition.Content = PlatTop - (int)leftPlatform.ActualHeight;
            yPosition.Content = PerTop;

            if (CharPosY > 400)
            {
                CharPos = 10;
                CharPosY = 0;
                DistanciaX = 1;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
        }
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
                                    Canvas.SetLeft(rightPlatform, point.X - 10);
                                    Canvas.SetTop(rightPlatform, point.Y - 10);
                                    //canvas.Children.Add(rectangleRightWrist);
                                }

                                else if (joint == myJoint2)
                                {
                                    Canvas.SetLeft(leftPlatform, point2.X - 30);
                                    Canvas.SetTop(leftPlatform, point2.Y - 30);
                                    // canvas.Children.Add(Shiryu);
                                    yPosition.Content = "y: " + point2.Y;
                                    xPosition.Content = "x: " + point2.X;

                                }

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