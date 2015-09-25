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
using System.Windows.Media.Animation;
using System.Drawing;





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
        int Llaves = 0;
        SoundPlayer backgroundM;
        bool distanceChanged = false;
        bool spinyOrb;
        System.Windows.Controls.Image[] keys = new System.Windows.Controls.Image[3];
        System.Windows.Controls.Image[] barreras = new System.Windows.Controls.Image[3];
        static Random random = new Random();



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
            keys[0] = key1; keys[1] = key2; keys[2] = key3;
            foreach (var key in keys)
            {

                Canvas.SetTop(key, random.Next(50, 350));
                Canvas.SetLeft(key, random.Next(100, 500));
            }

            barreras[0] = Wall1; barreras[1] = Wall2; barreras[2] = Wall3;
            foreach (var barrera in barreras)
            {

                Canvas.SetTop(barrera, random.Next(50, 350));
                Canvas.SetLeft(barrera, random.Next(100, 500));
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 5);
            timer.IsEnabled = true;
            timer.Start();
            timer.Tick += new EventHandler(timer_Tick);

        }
        void timer_Tick(object sender, EventArgs e)
        {
            var controller = WpfAnimatedGif.ImageBehavior.GetAnimationController(Avatar);
            var leftRun = new BitmapImage();
            leftRun.BeginInit();
            leftRun.UriSource = new Uri(@"Images\runningLeft.gif", UriKind.Relative);
            leftRun.EndInit();
            var Run = new BitmapImage();
            Run.BeginInit();
            Run.UriSource = new Uri(@"Images\megaman running.gif", UriKind.Relative);
            Run.EndInit();
            CharPos = int.Parse(Avatar.GetValue(Canvas.LeftProperty).ToString());


            //if(Personaje.po) valores de interaccion
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
            // valor se de Paredes
            int wall1 = int.Parse(Wall1.GetValue(Canvas.TopProperty).ToString());
            int wall2 = int.Parse(Wall2.GetValue(Canvas.TopProperty).ToString());
            int wall3 = int.Parse(Wall3.GetValue(Canvas.TopProperty).ToString());
            int leftWall1 = int.Parse(Wall1.GetValue(Canvas.LeftProperty).ToString());
            int leftWall2 = int.Parse(Wall2.GetValue(Canvas.LeftProperty).ToString());
            int leftWall3 = int.Parse(Wall3.GetValue(Canvas.LeftProperty).ToString());

            int Key1 = int.Parse(key1.GetValue(Canvas.TopProperty).ToString());
            int Key2 = int.Parse(key2.GetValue(Canvas.TopProperty).ToString());
            int Key3 = int.Parse(key3.GetValue(Canvas.TopProperty).ToString());
            int leftKey1 = int.Parse(key1.GetValue(Canvas.LeftProperty).ToString());
            int leftKey2 = int.Parse(key2.GetValue(Canvas.LeftProperty).ToString());
            int leftKey3 = int.Parse(key3.GetValue(Canvas.LeftProperty).ToString());

            System.Drawing.Rectangle AvatarRect = new System.Drawing.Rectangle(PerLeft, PerTop, (int)Avatar.Width, (int)Avatar.Height);
            System.Drawing.Rectangle rWall1 = new System.Drawing.Rectangle(leftWall1, wall1, (int)Wall1.Width, (int)Wall1.ActualHeight);
            System.Drawing.Rectangle rWall2 = new System.Drawing.Rectangle(leftWall2, wall2, (int)Wall2.Width, (int)Wall2.ActualHeight);
            System.Drawing.Rectangle rWall3 = new System.Drawing.Rectangle(leftWall3, wall3, (int)Wall3.Width, (int)Wall3.ActualHeight);

            System.Drawing.Rectangle rKey1 = new System.Drawing.Rectangle(leftKey1, Key1, (int)key1.Width, (int)key1.ActualHeight);
            System.Drawing.Rectangle rKey2 = new System.Drawing.Rectangle(leftKey2, Key2, (int)key2.Width, (int)key2.ActualHeight);
            System.Drawing.Rectangle rKey3 = new System.Drawing.Rectangle(leftKey3, Key3, (int)key3.Width, (int)key3.ActualHeight);


            //xPosition.Content = "avatar x= " + PerLeft + "Area= \n" + AvatarRect;
            //yPosition.Content = "wall1 x= " + rWall1 + "Area= \n" + Wall1.ActualHeight + "intersects?\n";
            //Console.WriteLine(distanceChanged + " " + DistanciaX + "monito: "+ Avatar.GetValue(Canvas.LeftProperty));
            if (AvatarRect.IntersectsWith(rWall1) || AvatarRect.IntersectsWith(rWall2) || AvatarRect.IntersectsWith(rWall3))
            {

                distanceChanged = true;
                DistanciaX *= -1;
                Canvas.SetLeft(Avatar, CharPos - 20);
                //Canvas.SetTop(Avatar, 1000);


            }

            if (AvatarRect.IntersectsWith(rKey1))
            {
                Canvas.SetLeft(key1, 1000);
                Canvas.SetTop(key1, 1000);
                Llaves++;
                yPosition.Content = Llaves;
            }
            else if (AvatarRect.IntersectsWith(rKey2))
            {
                Canvas.SetLeft(key2, 1000);
                Canvas.SetTop(key2, 1000);
                Llaves++;
                yPosition.Content = Llaves;

            }
            else if (AvatarRect.IntersectsWith(rKey3))
            {
                Canvas.SetLeft(key3, 1000);
                Canvas.SetTop(key3, 1000);
                Llaves++;
                yPosition.Content = Llaves;
            }

            if (PerTop < PlatTop && PerTop > PlatTop - Avatar.ActualHeight && PerLeft < PlatLeft + (int)leftPlatform.ActualWidth && PerLeft >= PlatLeft)
            {
                CharPos += DistanciaX;
                CharPosY = PlatTop - (int)Avatar.ActualHeight;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else if (PerTop < PlatTop2 && PerTop > PlatTop2 - Avatar.ActualHeight && PerLeft < PlatLeft2 + (int)rightPlatform.ActualWidth && PerLeft >= PlatLeft2)
            {
                CharPos += DistanciaX;
                CharPosY = PlatTop2 - (int)Avatar.ActualHeight;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else if (PerTop < ground1 && PerTop > ground1 - Avatar.ActualHeight && PerLeft < leftGround1 + (int)Floor1.ActualWidth && PerLeft >= leftGround1)
            {
                CharPos += DistanciaX;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else if (PerTop < door && PerTop > door - Avatar.ActualHeight && PerLeft < leftDoor + (int)Door.ActualWidth && PerLeft >= leftDoor)
            {
                if (Llaves == 3) xPosition.Content = "GANASTE";
                timer.Stop();
                CharPos += DistanciaX;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
            }
            else
            {
                CharPosY += 1;
                Canvas.SetTop(Avatar, CharPosY);
            }


            if (CharPosY > 400)
            {

                CharPos = 30;
                CharPosY = -55;
                DistanciaX = 1;

                distanceChanged = true;
                Canvas.SetLeft(Avatar, CharPos);
                Canvas.SetTop(Avatar, CharPosY);
                //Re dibujar llaves
                Llaves = 0;
                foreach (var key in keys)
                {

                    Canvas.SetTop(key, random.Next(50, 350));
                    Canvas.SetLeft(key, random.Next(100, 500));
                }

            }
            if (distanceChanged)
            {
                distanceChanged = false;
                if (DistanciaX == 1)
                {
                    WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Avatar, Run);
                    WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(Avatar, new RepeatBehavior(0));
                    WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(Avatar, RepeatBehavior.Forever);
                    WpfAnimatedGif.ImageBehavior.SetAutoStart(Avatar, true);
                }
                else
                {
                    WpfAnimatedGif.ImageBehavior.SetAnimatedSource(Avatar, leftRun);
                    WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(Avatar, new RepeatBehavior(0));
                    WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(Avatar, RepeatBehavior.Forever);
                    //WpfAnimatedGif.ImageBehavior.GetAnimationController(Avatar).Play();
                    WpfAnimatedGif.ImageBehavior.SetAutoStart(Avatar, true);
                }
            }
            if (CharPos >= 560 || CharPos == -1)
            {
                CharPos = 25;
                distanceChanged = true;
                //Console.WriteLine("Si entra");
                DistanciaX *= -1;
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

                                Joint myJoint2 = body.Joints[JointType.HandLeft]; //the joint I want to compare
                                Joint myJoint = body.Joints[JointType.HandRight]; //the joint I want to compare

                                // 2D coordinates in pixels
                                System.Windows.Point point = new System.Windows.Point();
                                System.Windows.Point point2 = new System.Windows.Point();


                                // Skeleton-to-Color mapping
                                ColorImagePoint colorPoint = _sensor.CoordinateMapper.MapSkeletonPointToColorPoint(skeletonPoint, ColorImageFormat.RgbResolution640x480Fps30);

                                point.X = colorPoint.X;
                                point.Y = colorPoint.Y;
                                point2.X = colorPoint.X;
                                point2.Y = colorPoint.Y;


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
                                    // yPosition.Content = "y: " + point2.Y;
                                    // xPosition.Content = "x: " + point2.X;

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