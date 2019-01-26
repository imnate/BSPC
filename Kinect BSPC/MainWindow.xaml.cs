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
using System.IO;
using System.Media;
using System.Threading;

namespace Kinectcolortest
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += ColorWindow_Loaded;
            Unloaded += ColorWindow_Unloaded;
        }
        KinectSensor kinect;
       
        void ColorWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (kinect != null)
            {
               
                kinect.ColorStream.Disable();
                kinect.SkeletonStream.Disable();
                kinect.Stop();
                //kinect.ColorFrameReady -= myKinect_ColorFrameReady;
                //kinect.SkeletonFrameReady -= mykinect_SkeletonFrameReady;
                kinect.AllFramesReady -= All_mykinect_SkeletonFrameReady;
            }
        }
       

        byte[] Color_pixelData;
        //byte[] Pic;
        //byte[] Pic2;
        public CoordinateMapper coordinateMapper;
       
        Skeleton[] FrameSkeletons;

        void ColorWindow_Loaded(object sender, RoutedEventArgs e)
        {
            kinect = KinectSensor.KinectSensors[0];
            if (kinect != null)
            {
                /*MediaPlayer mplayer = new MediaPlayer();
                mplayer.Open(new Uri(@"C:\Users\MCU\Desktop\專\WpfApplication1\Resources\右手錯誤.wav", UriKind.Relative));
                mplayer.Play();
                Thread.Sleep(2000);
                mplayer.Open(new Uri(@"C:\Users\MCU\Desktop\Kinectcolortest\Kinectcolortest\Resources\右腳錯誤.wav", UriKind.Relative));
                mplayer.Play();
                Thread.Sleep(2000);
                */// MessageBox.Show("OK");
                ColorImageStream colorStream = kinect.ColorStream;               

                kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                kinect.SkeletonStream.Enable();
                
                Color_pixelData = new byte[640 * 480 * 4];
                //Pic = new byte[100 * 100 * 4];
                //Pic2 = new byte[100 * 100 * 4];

                coordinateMapper = new CoordinateMapper(kinect);
                kinect.AllFramesReady += this.All_mykinect_SkeletonFrameReady;
                kinect.Start();
            }

        }
        //////////////////////////////////////////////////////////////////////////////
        void All_mykinect_SkeletonFrameReady(object sender, AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skframe = e.OpenSkeletonFrame())
            {               
                    using (ColorImageFrame CFrame = e.OpenColorImageFrame())
                    {
                        if (skframe != null && CFrame != null)
                        {
                            int rfx = 50, rfy = 50, lfx = 50, lfy = 50;
                            CFrame.CopyPixelDataTo(Color_pixelData);
                            FrameSkeletons = new Skeleton[skframe.SkeletonArrayLength];
                            skframe.CopySkeletonDataTo(FrameSkeletons);
                            for (int i = 0, j = 0; i < FrameSkeletons.Length; i++, j += 4)
                            {
                                if (FrameSkeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                                {
                                    ColorImagePoint footr = MapToColorImage(FrameSkeletons[i].Joints[JointType.FootRight]);
                                    ColorImagePoint footl = MapToColorImage(FrameSkeletons[i].Joints[JointType.FootLeft]);

                                    rfx = footr.X;
                                    rfy = footr.Y;
                                    lfx = footl.X;
                                    lfy = footl.Y;

                                    Joint jpl = FrameSkeletons[i].Joints[JointType.HandLeft];
                                    Joint jpr = FrameSkeletons[i].Joints[JointType.HandRight];
                                    Joint jlf = FrameSkeletons[i].Joints[JointType.FootLeft];
                                    Joint jrf = FrameSkeletons[i].Joints[JointType.FootRight];
                                    Joint jsl = FrameSkeletons[i].Joints[JointType.ShoulderLeft];
                                    Joint jsr = FrameSkeletons[i].Joints[JointType.ShoulderRight];
                                    Joint jar = FrameSkeletons[i].Joints[JointType.AnkleRight];
                                    Joint jal = FrameSkeletons[i].Joints[JointType.AnkleLeft];

                                    double jtr = Math.Atan2(jrf.Position.Z - jar.Position.Z, jrf.Position.X - jar.Position.X) * 180 / 3.14;
                                    double jtl = Math.Atan2(jlf.Position.Z - jal.Position.Z, jlf.Position.X - jal.Position.X) * 180 / 3.14;

                                    //MediaPlayer mplayer = new MediaPlayer();

                                    ColorImagePoint cpr = MapToColorImage(FrameSkeletons[i].Joints[JointType.HandRight]);
                                    ColorImagePoint cpl = MapToColorImage(FrameSkeletons[i].Joints[JointType.HandLeft]);
                                    ColorImagePoint shl = MapToColorImage(FrameSkeletons[i].Joints[JointType.ShoulderLeft]);
                                    ColorImagePoint shr = MapToColorImage(FrameSkeletons[i].Joints[JointType.ShoulderRight]);
                                    ColorImagePoint rf = MapToColorImage(FrameSkeletons[i].Joints[JointType.FootRight]);
                                    Shoulder.X1 = shl.X;
                                    Shoulder.Y1 = shl.Y;
                                    Shoulder.X2 = shr.X;
                                    Shoulder.Y2 = shr.Y;



                                    if (Math.Abs(jtr - jtl) >= 35 && Math.Abs(jtr - jtl) <= 65)
                                    {
                                        textBlock4.Text = "腳姿正確";
                                        textBlock4.Foreground = new SolidColorBrush(Colors.Black);
                                        PicIn(RightFoot, rf);
                                    }
                                    else
                                    {
                                        textBlock4.Text = "腳姿錯誤";
                                        textBlock4.Foreground = new SolidColorBrush(Colors.Red);
                                        RightFoot.Visibility = Visibility.Collapsed;
                                    }

                                    Joint hip = FrameSkeletons[i].Joints[JointType.HipCenter];
                                    double jls = Math.Atan2(jsl.Position.Z - jsr.Position.Z, jsl.Position.X - jsr.Position.X) * 180 / 3.14;
                                    if (Math.Abs(jtl - jls) >= 35 && Math.Abs(jtl - jls) <= 65)
                                    {
                                        textBlock1.Text = "肩膀正確";
                                        textBlock1.Foreground = new SolidColorBrush(Colors.Black);
                                        Shoulder.Stroke = new SolidColorBrush(Colors.Green);
                                        PicIn(Should, shl);
                                    }
                                    else
                                    {
                                        textBlock1.Text = "肩膀錯誤";
                                        textBlock1.Foreground = new SolidColorBrush(Colors.Red);
                                        Shoulder.Stroke = new SolidColorBrush(Colors.Red);
                                        Should.Visibility = Visibility.Collapsed;
                                    }

                                    
                                    
                                        if (jpr.Position.Z > jlf.Position.Z + 0.65 && jpr.Position.Y < jsr.Position.Y + 0.2 && jpr.Position.Y > jsr.Position.Y - 0.2)
                                        {
                                            textBlock2.Text = "右手正確";
                                            textBlock2.Foreground = new SolidColorBrush(Colors.Black);
                                            PicIn(RightHand, cpr);
                                            

                                        }
                                        
                                        else
                                        {
                                            textBlock2.Text = "右手錯誤";
                                            textBlock2.Foreground = new SolidColorBrush(Colors.Red);
                                            RightHand.Visibility = Visibility.Collapsed;
                                            

                                        }
                                    
                                    
                                    Joint shou = FrameSkeletons[i].Joints[JointType.ShoulderCenter];
                                    Joint spin = FrameSkeletons[i].Joints[JointType.Spine];

                                    if (jpl.Position.X - ((jsl.Position.X + jsr.Position.X) / 2 + 4 * (jlf.Position.X - jal.Position.X)) >= -0.03
                                        && jpl.Position.X - ((jsl.Position.X + jsr.Position.X) / 2 + 4 * (jlf.Position.X - jal.Position.X)) <= 0.03
                                        && jpl.Position.Y < shou.Position.Y && jpl.Position.Y > spin.Position.Y && jsl.Position.Z - jpl.Position.Z >= 0.5)
                                    {
                                        textBlock3.Text = "左手正確";
                                        textBlock3.Foreground = new SolidColorBrush(Colors.Black);
                                        PicIn(LeftHand, cpl);

                                    }
                                    else
                                    {
                                        textBlock3.Text = "左手錯誤";
                                        textBlock3.Foreground = new SolidColorBrush(Colors.Red);
                                        LeftHand.Visibility = Visibility.Collapsed;
                                    }

                                }
                            }
                            /*if(rfx<590&&rfy<430)
                            for (int i = 0, j = ((rfy-50)*640+rfx-50)*4, z = 0; i < 640 * 480; i++, j += 4)
                            
                                int y = i / 640;
                                int x = i % 640;
                                if (y < 100 && x<100 ) 
                                {                                   
                                        Pic[z]=Color_pixelData[j];
                                        Pic[z+1]=Color_pixelData[j + 1];
                                        Pic[z+2] = Color_pixelData[j + 2];
                                        Pic[z+3] = Color_pixelData[j + 3];
                                        z += 4;                                   
                                }
                            }
                            if (lfx < 590 && lfy < 430)
                                for (int i = 0, j = ((lfy - 50) * 640 + lfx - 50) * 4, z = 0; i < 640 * 480; i++, j += 4)
                                {
                                    int y = i / 640;
                                    int x = i % 640;
                                    if (y < 100 && x < 100)
                                    {
                                        Pic2[z] = Color_pixelData[j];
                                        Pic2[z + 1] = Color_pixelData[j + 1];
                                        Pic2[z + 2] = Color_pixelData[j + 2];
                                        Pic2[z + 3] = Color_pixelData[j + 3];
                                        z += 4;
                                    }
                                }
                            textBlock1.Text ="紅"+Pic[20202]+"綠"+Pic[20201]+"藍"+Pic[20200];
                            int fr=0,fl=0;
                            for (int i = 0; i < 100 * 100 * 4; i += 4) 
                            {
                                if(Pic[i+2] >= 120 &&
                                   Pic[i+2] <= 160&&
                                   Pic[i+1] <= 70 &&
                                    Pic[i+1]>=300&&
                                    Pic[i]>=50&&
                                   Pic[i] <= 90 )
                                    fr+=1;
                            }
                            if(fr>=200)
                                 MessageBox.Show("OK");
                            */


                            BitmapSource source = BitmapSource.Create(640, 480, 96, 96,
                            PixelFormats.Bgr32, null, Color_pixelData, 640 * 4);
                            ColorData.Source = source;
                            //image1.Source =  BitmapSource.Create(100, 100, 96, 96,
                            //PixelFormats.Bgr32, null, Pic, 100 * 4);
                            //image2.Source = BitmapSource.Create(100, 100, 96, 96,
                            //PixelFormats.Bgr32, null, Pic2, 100 * 4);

                        }
                        else
                        {
                            RightHand.Visibility = Visibility.Collapsed;
                            LeftHand.Visibility = Visibility.Collapsed;
                            Should.Visibility = Visibility.Collapsed;
                            RightFoot.Visibility = Visibility.Collapsed;
                            Shoulder.Visibility = Visibility.Collapsed;
                            
                        }

                }
            }

        }
        /////////////////////////////////////////////////////////////////////////////////////////////
        ColorImagePoint MapToColorImage(Joint jp)
        {
            //coordinateMapper.MapSkeletonPointToColorPoint(jp.Position, kinect.ColorStream.Format);          
            ColorImagePoint cp = kinect.MapSkeletonPointToColor(jp.Position, kinect.ColorStream.Format);
            return cp;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string pic = NewFileName();
            
            
            SaveToFile(pic);

        }
        ////////////////////////////////////////////////////////////////////////////////////////////
        private int i = 0;
        public string NewFileName()
        {
            i++;

            string mypic = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
           
            string filename = mypic + "\\pic_" + i + ".png";
            return filename;
        }
        public void SaveToFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
            {
                BitmapSource image = (BitmapSource)ColorData.Source;
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fs);
            }
        }
        void PicIn(Image pic,ColorImagePoint cp)
        {
            Canvas.SetLeft(pic, cp.X - pic.Width / 2);
            Canvas.SetTop(pic, cp.Y - pic.Height / 2);
            pic.Visibility = Visibility.Visible;
        }
    }
}
