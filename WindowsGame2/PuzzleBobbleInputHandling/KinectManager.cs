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
        public enum Movement { LEFT, RIGHT, IDLE };
        private static KinectManager instance = null;
        private float minimumMovement = 0.2f;
        private Movement kinectMovement = Movement.IDLE;
        private bool kinectShoot = false;
        public static KinectManager getInstance() {
            if (instance == null)
                instance = new KinectManager();
            return instance;
        }
        private KinectManager() {
            kinectInit();
        }

        float prevLeftHandY;
        SkeletonFrame skelFrame;
        Skeleton[] skeletons;
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            skelFrame = e.OpenSkeletonFrame();
            skeletons = new Skeleton[kinectSensor.SkeletonStream.FrameSkeletonArrayLength];          
            if (skelFrame != null)
            {
                skelFrame.CopySkeletonDataTo(skeletons);
                foreach (Skeleton skel in skeletons) {
                    if (skel.TrackingState >= SkeletonTrackingState.PositionOnly)
                    {
                        var rightHand = skel.Joints[JointType.HandRight].Position;
                        var centerShoulder = skel.Joints[JointType.ShoulderCenter].Position;
                        var leftHand = skel.Joints[JointType.HandLeft].Position;

                        if (leftHand.Y > centerShoulder.Y && prevLeftHandY < centerShoulder.Y)
                            this.kinectShoot = true;
                        else
                            this.kinectShoot = false;

                        prevLeftHandY = leftHand.Y;

                        float diff = rightHand.X - centerShoulder.X;

                        if (Math.Abs(diff) > minimumMovement)
                        {
                            if (diff > 0)
                            {
                                this.kinectMovement = Movement.RIGHT;
                            }
                            else
                            {
                                this.kinectMovement = Movement.LEFT;
                            }
                        }
                        else
                        {
                            this.kinectMovement = Movement.IDLE;
                        }
#if DEBUG
                        Console.WriteLine(rightHand.X);
#endif
                        skelFrame.Dispose();
                        return;
                    }
                }
                skelFrame.Dispose();
            }
        }
        public bool isMovingLeft()
        {
            return this.kinectMovement == Movement.LEFT;
        }
        public bool isMovingRight()
        {
            return this.kinectMovement == Movement.RIGHT;
        }
        public bool isShooting()
        {
            return this.kinectShoot;
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
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.SkeletonFrameReady += OnSkeletonFrameReady;
                kinectSensor.Start();
            }
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
