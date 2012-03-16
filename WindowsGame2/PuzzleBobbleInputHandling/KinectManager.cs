using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace PuzzleBobbleInputHandling
{
    public class KinectManager
    {
        private KinectSensor kinectSensor;
        public enum Movement { LEFT, RIGHT, IDLE };
        private static KinectManager instance = null;
        private float minimumMovement = 0.2f;
        private Movement kinectMovement = Movement.IDLE;
        private Vector2 leftHandPosition;
        private Vector2 rightHandPosition;
        private bool kinectShoot = false;
        private bool skeletonTracked = false;
        
        public static KinectManager getInstance() {
            if (instance == null)
                instance = new KinectManager();
            return instance;
        }

        private KinectManager() {
            kinectInit();
        }

        public bool isActive()
        {
            return kinectSensor != null;
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
                        this.skeletonTracked = true;

                        var rightHand = skel.Joints[JointType.HandRight].Position;
                        var centerShoulder = skel.Joints[JointType.ShoulderCenter].Position;
                        var leftHand = skel.Joints[JointType.HandLeft].Position;

                        this.rightHandPosition.X = rightHand.X;
                        this.rightHandPosition.Y = rightHand.Y;
                        this.leftHandPosition.X = leftHand.X;
                        this.leftHandPosition.Y = leftHand.Y;

                        if (leftHand.Y > centerShoulder.Y && prevLeftHandY < centerShoulder.Y)
                            this.kinectShoot = true;
                        else
                            this.kinectShoot = false;

                        prevLeftHandY = leftHand.Y;

                        float diff = rightHand.X - (centerShoulder.X + 0.25f);

                        this.kinectMovement = HorizontalGesture.getMovementFromPosition(centerShoulder.X, centerShoulder.Y, rightHand.X, rightHand.Y);
                        // CircularGesture.getMovementFromPosition(centerShoulder.X , centerShoulder.Y , rightHand.X, rightHand.Y);

                        //if (Math.Abs(diff) > minimumMovement)
                        //{
                        //    if (diff > 0)
                        //    {
                        //        this.kinectMovement = Movement.RIGHT;
                        //    }
                        //    else
                        //    {
                        //        this.kinectMovement = Movement.LEFT;
                        //    }
                        //}
                        //else
                        //{
                        //    this.kinectMovement = Movement.IDLE;
                        //}
#if DEBUG
                        //          Console.WriteLine(rightHand.X);
#endif
                        skelFrame.Dispose();
                        return;
                    }
                    else {
                        this.skeletonTracked = false;
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
        public Vector2 getRightHandPosition()
        {
            return this.rightHandPosition;
        }
        public Vector2 getLeftHandPosition()
        {
            return this.leftHandPosition;
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

        public bool isTracking()
        {
            return this.isActive() && this.skeletonTracked;
        }
    }
}
