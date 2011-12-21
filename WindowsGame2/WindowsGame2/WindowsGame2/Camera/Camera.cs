using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame2
{
    public class Camera
    {

       
        public enum CameraTransformations
        {
            YawLeft,
            YawRight,
            PitchUp,
            PitchDown,
            RollClockwise,
            RollAnticlockwise,
            MoveForward,
            MoveBackward,
            MoveUp,
            MoveDown,
            MoveRight,
            MoveLeft

        };

        private Vector3 rotations = Vector3.Zero;
        private Vector3 position = Vector3.Zero;
        private Vector3 defaultPosition = Vector3.Zero;
        private Vector3 look = Vector3.Forward;
        private Vector3 up = Vector3.Up;
        private Vector3 right = Vector3.Right;
        public Matrix viewMatrix;

        public void setAbsolutePosition(Vector3 pos)
        {
            this.position = pos;
        }
        public void resetCamera() { 
            this.rotations = Vector3.Zero;
            this.position = this.defaultPosition;
            this.look = Vector3.Forward;

            this.updateViewMatrix();
         //   this.up = Vector3.Up;
    
        }
        //public void roll() {
        //   // look.Normalize();
            
        //    Matrix rollMatrix = Matrix.CreateFromAxisAngle(look, rotations.Z);
            
        //    up = Vector3.Transform(up, rollMatrix);
        // //   right = Vector3.Transform(right, rollMatrix);

        //  //  Console.WriteLine("roll " + rotations.Z + " look " + look + " up " + up + " right " + right);

            
        //}
        //public void pitch() {
  
        //    Matrix pitchMatrix = Matrix.CreateFromAxisAngle(right, rotations.X);
        //    look = Vector3.Transform(look, pitchMatrix);
        //    up = Vector3.Transform(up, pitchMatrix); 
        
        //}
        //public void yaw() {
        // //   Console.WriteLine("yaw");
        ////    up.Normalize();
        //    Matrix yawMatrix = Matrix.CreateFromAxisAngle(up, rotations.Y);

        //    look = Vector3.Transform(look, yawMatrix);
        //    right = Vector3.Transform(right, yawMatrix);

            
        //}
        public void moveLeft(float delta) {
            this.position -= this.right * delta;
        }
        public void moveRight(float delta) {
            this.position += this.right * delta;
        }
        public void moveUp(float delta) {
            this.position += this.up * delta;
        }
        public void moveDown(float delta) {
            this.position -= this.up * delta;
        }
        public void moveForward(float delta) {
            this.position += this.look * delta;
        }
        public void moveBack(float delta) {
            this.position -= this.look * delta;
        }

        private void rotateX(float angle)
        {
            this.rotations.X += angle;

            // keep the value in the range 0-360 (0 - 2 PI radians)
            if (this.rotations.X > Math.PI * 2)
                this.rotations.X -= MathHelper.Pi * 2;
            else if (this.rotations.X < 0)
                this.rotations.X += MathHelper.Pi * 2;
        }
        private void rotateY(float angle)
        {
            this.rotations.Y += angle;

            if (this.rotations.Y > Math.PI * 2)
                this.rotations.Y -= MathHelper.Pi * 2;
            else if (this.rotations.Y < 0)
                this.rotations.Y += MathHelper.Pi * 2;
        }
        private void rotateZ(float angle)
        {
            this.rotations.Z += angle;

            // keep the value in the range 0-360 (0 - 2 PI radians)
            if (this.rotations.Z > Math.PI * 2)
                this.rotations.Z -= MathHelper.Pi * 2;
            else if (this.rotations.Z < 0)
                this.rotations.Z += MathHelper.Pi * 2;
        }

        public void transform(CameraTransformations t, float dt) {

          //  Console.WriteLine("Camera.transform()");
            float val = 0.02f;//MathHelper.ToRadians(1) * 0.001f;// MathHelper.ToRadians(0.2 * dt);

            switch (t)
            {
                case CameraTransformations.YawLeft:
                    this.rotate(CameraTransformations.YawLeft, val);// rotateY(val);
                    break;
                case CameraTransformations.YawRight:
                    this.rotate(CameraTransformations.YawRight, val);
                    break;
                case CameraTransformations.PitchUp:
                    this.rotate(CameraTransformations.PitchUp, val);// rotateY(val);
                    break;
                case CameraTransformations.PitchDown:
                    this.rotate(CameraTransformations.PitchDown, val);// rotateY(val);
                    break;
                case CameraTransformations.RollClockwise:
                    this.rotate(CameraTransformations.RollClockwise, val);// rotateY(val);
                    break;
                case CameraTransformations.RollAnticlockwise:
                    this.rotate(CameraTransformations.RollAnticlockwise, val);// rotateY(val);
                    break;
                case CameraTransformations.MoveForward:
                    this.moveForward(val);
                    break;
                case CameraTransformations.MoveBackward:
                    this.moveBack(val);
                    break;
                case CameraTransformations.MoveLeft:
                    this.moveLeft (val);
                    break;
                case CameraTransformations.MoveRight:
                    this.moveRight(val);
                    break;
                case CameraTransformations.MoveUp:
                    this.moveUp(val);
                    break;

                case CameraTransformations.MoveDown:
                    this.moveDown(val);
                    break;


            }
            updateViewMatrix();
        
        }
        public void rotate(CameraTransformations rot, float angle)
        {
            switch (rot)
            {
                case CameraTransformations.YawLeft:
                    rotateY(angle);
                    break;
                case CameraTransformations.YawRight:
                    rotateY(-angle);
                    break;
                case CameraTransformations.PitchUp:
                    rotateX(angle);
                    break;
                case CameraTransformations.PitchDown:
                    rotateX(-angle);
                    break;
                case CameraTransformations.RollClockwise:
                    rotateZ(angle);
                    break;
                case CameraTransformations.RollAnticlockwise:
                    rotateZ(-angle);
                    break;

                
            }
            updateViewMatrix();
        }
        public void updateViewMatrix() {

       //     Console.WriteLine("position " + this.position);
            Matrix rot = Matrix.CreateFromYawPitchRoll(this.rotations.Y, this.rotations.X, this.rotations.Z);
            this.look = Vector3.Transform(Vector3.Forward, rot);
            this.up = Vector3.Transform(Vector3.Up, rot);
            this.right = Vector3.Cross(this.look, this.up);
            Vector3 target = this.position + this.look ;//this.position  + this.look;
            this.viewMatrix = Matrix.CreateLookAt(this.position, target, this.up);
           // Console.WriteLine("angle view " + this.rotations);
        }
        public Camera() {
            
            this.viewMatrix = Matrix.CreateLookAt(this.position + this.look, this.look, this.up  );

        }
        public Camera(Vector3 absolutePosition) {
            this.defaultPosition = absolutePosition;
            this.setAbsolutePosition(absolutePosition);
          //  this.updateViewMatrix();
        }
    }
}
