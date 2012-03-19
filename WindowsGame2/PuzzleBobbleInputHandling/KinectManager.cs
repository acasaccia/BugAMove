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
        
        float pauseDelta = 0.05f;
        float shootLeftHandDelta = 0.1f;

        private bool kinectPause = false;
        private bool kinectContinue = false;
        private bool kinectGoBack = false;
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
        SkeletonPoint prevLeftHand;
        SkeletonFrame skelFrame;
        Skeleton[] skeletons;

        public bool isPaused() {
            return this.kinectPause;
        }
        private bool shootGestureDetect(SkeletonPoint currLeftHand, SkeletonPoint prevLeftHand, SkeletonPoint leftShoulder)
        {
            //if (leftHand.Y > centerShoulder.Y && prevLeftHandY < centerShoulder.Y)
            //    return true;
            if (currLeftHand.Y - leftShoulder.Y > shootLeftHandDelta &&
                Math.Abs(currLeftHand.X - leftShoulder.X) < shootLeftHandDelta &&
                prevLeftHand.Y < leftShoulder.Y)
                return true;
            return false;
        }
        private bool continueGesture(SkeletonPoint centerShoulder, SkeletonPoint head) 
        {
            if (centerShoulder.Y < head.Y)
                return true;
            return false;
        }
        private bool pauseGesturePrayDetect(SkeletonPoint leftHand, SkeletonPoint rightHand, SkeletonPoint centerShoulder,
            SkeletonPoint leftElbow, SkeletonPoint rightElbow)
        {
            if (Math.Abs(rightHand.Y - leftHand.Y) < pauseDelta &&
                Math.Abs(rightElbow.Y - leftElbow.Y) < pauseDelta &&
                Math.Abs(rightHand.X - leftHand.X) < pauseDelta)
            {
                return true;
            }
            return false;
        }
        private bool goBackGestureDetect(SkeletonPoint head, SkeletonPoint leftHand)
        {
            if (leftHand.Y > head.Y)
                return true;
            return false;
        }
        private bool pauseGestureDetect(SkeletonPoint leftHand, SkeletonPoint rightHand, SkeletonPoint centerShoulder)
        {
            if (Math.Abs(rightHand.X - leftHand.X ) < pauseDelta  &&
                Math.Abs(rightHand.Y - leftHand.Y) < pauseDelta &&
                Math.Abs(rightHand.X - centerShoulder.X) < pauseDelta &&
                Math.Abs(leftHand.X - centerShoulder.X) < pauseDelta &&
                Math.Abs(rightHand.Y - centerShoulder.Y) < pauseDelta &&
                Math.Abs(rightHand.Y - centerShoulder.Y) < pauseDelta)
                    
                return true;
            return false;
            
        }
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

                        SkeletonPoint rightHand = skel.Joints[JointType.HandRight].Position;
                        SkeletonPoint centerShoulder = skel.Joints[JointType.ShoulderCenter].Position;
                        SkeletonPoint leftHand = skel.Joints[JointType.HandLeft].Position;
                        SkeletonPoint leftShoulder = skel.Joints[JointType.ShoulderLeft].Position;
                     // JointType.
                        this.rightHandPosition.X = rightHand.X;
                        this.rightHandPosition.Y = rightHand.Y;
                        this.leftHandPosition.X = leftHand.X;
                        this.leftHandPosition.Y = leftHand.Y;

                        if (leftHand.Y > centerShoulder.Y && prevLeftHandY < centerShoulder.Y)
                            this.kinectShoot = true;
                        else
                            this.kinectShoot = false;
                    //    this.kinectShoot =  shootGestureDetect(leftHand, prevLeftHand, leftShoulder);
                        this.kinectContinue = this.continueGesture(rightHand, skel.Joints[JointType.Head].Position);
                        prevLeftHand = leftHand;
                        prevLeftHandY = leftHand.Y;

                        this.kinectGoBack = this.goBackGestureDetect(skel.Joints[JointType.Head].Position, leftHand);
                        float diff = rightHand.X - (centerShoulder.X + 0.25f);

                        this.kinectMovement = HorizontalGesture.getMovementFromPosition(centerShoulder.X, centerShoulder.Y, rightHand.X, rightHand.Y);

                       // this.kinectPause = this.pauseGestureDetect(leftHand, rightHand, centerShoulder);
                        this.kinectPause = pauseGesturePrayDetect(leftHand, rightHand, centerShoulder,
                            skel.Joints[JointType.ElbowLeft].Position, skel.Joints[JointType.ElbowRight].Position);
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

        public bool goBack ()
        { 
            return this.kinectGoBack;
        }
        public bool isContinue() 
        {
            return this.kinectContinue;
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
