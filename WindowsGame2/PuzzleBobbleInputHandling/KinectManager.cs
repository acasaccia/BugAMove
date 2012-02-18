using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
namespace PuzzleBobbleInputHandling
{
    class KinectManager
    {
        private KinectSensor kinectSensor;
        private Skeleton[] skeletons;
        public enum Movement { LEFT, RIGHT, IDLE };
        private static KinectManager instance = null;
        private float minimumMovement = 0;
        private Movement kinectMovement = Movement.IDLE;
        public static KinectManager getInstance() {
            if (instance == null)
                instance = new KinectManager();
            return instance;
        }
        private KinectManager() {
            kinectInit();
        }

        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            SkeletonFrame skelFrame = e.OpenSkeletonFrame();

            if (skeletons == null)
            {
                skeletons = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];
            }   // skelFrame.
            // skelFrame.
            if (skelFrame != null)
            {
                skelFrame.CopySkeletonDataTo(skeletons);
                var rightHand = skeletons[0].Joints[JointType.HandRight].Position;
                var rightElbow = skeletons[0].Joints[JointType.ElbowRight].Position;

                float diff = rightHand.X - rightElbow.X;

                if (Math.Abs(diff) > minimumMovement)
                {
                    if (diff > 0)
                    {
                        this.kinectMovement = Movement.RIGHT;
                    }
                    else {
                        this.kinectMovement = Movement.LEFT;
                    }
                }
                else {
                    this.kinectMovement = Movement.IDLE;
                }
                
                //Console.WriteLine(rightHand.X);
            }
        }
        public bool isMovingLeft() {
            return this.kinectMovement == Movement.LEFT;
        }
        public bool isMovingRight()
        {
            return this.kinectMovement == Movement.RIGHT;
        }
        private void SetSensor(KinectSensor newSensor)
        {
            if (kinectSensor != null)
            {
                kinectSensor.Stop();
            }

            kinectSensor = newSensor;

            if (kinectSensor != null)
            {
               // Debug.Assert(kinectSensor.Status == KinectStatus.Connected, "This should only be called with Connected sensors.");
            //    kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
             //   kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.AllFramesReady += _sensor_AllFramesReady;
                kinectSensor.SkeletonFrameReady += OnSkeletonFrameReady;
                kinectSensor.Start();
            }
        }
        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            bool gotColor = false;
            bool gotDepth = false;


        }
        public void kinectInit()
        {
            KinectSensor.KinectSensors.StatusChanged += (object sender, StatusChangedEventArgs e) =>
            {
                if (e.Sensor == kinectSensor)
                {
                    if (e.Status != KinectStatus.Connected)
                    {
                        SetSensor(null);
                    }
                }
                else if ((kinectSensor == null) && (e.Status == KinectStatus.Connected))
                {
                    SetSensor(e.Sensor);
                }
            };

            foreach (var sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    SetSensor(sensor);
                }
            }
        }
    }
}
