using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Kinect;// .Nui;
using System.Diagnostics;
namespace PuzzleBobbleInputHandling
{
    public class PuzzleBobbleGameInputState {
        public bool ArrowMovedLeft = false;
        public bool ArrowMovedRight = false;
        public bool BallShoot = false;
        public bool Continue = false;
        public bool SoundOn = false;
        public bool SoundOff = false;
        
    }
    
    public class InputManager: GameComponent
    {
        public enum InputDevice { KEYBOARD, GAMEPAD };
        private static PuzzleBobbleGameInputState inputState;
        private bool running;
        private InputDevice inputDevice;
   
        
        
      

        
        public InputManager(Game game) : base(game) {
            inputState = new PuzzleBobbleGameInputState();
            Game.Services.AddService(typeof(InputManager),this);

            this.inputDevice = InputDevice.GAMEPAD;
           // kinectInit();
            KinectManager.getInstance();
            //kinectSensor;
            //this.kinectSensor ;//= new KinectSensor();

        }
        public static PuzzleBobbleGameInputState getState()
        {
            return inputState;

        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void Dispose(bool disposing)
        {
            Game.Services.RemoveService(typeof(InputManager));
            base.Dispose(disposing);
        }
        public void start() {
            this.running = true;
        }
        public void pause() {
            this.running = false;
        }
        public override void Update(GameTime gameTime)
        {
            if (!running)
                return;

            

            //if (kinectSensor.IsRunning)
            //{
            //   // Console.WriteLine("WOW kinect is running");
            ////    kinectSensor.SkeletonStream.
            //    if (kinectSensor.SkeletonStream.IsEnabled)
            //    {
            //        Console.WriteLine("skeleton stream is enabled");

            //    }
            //}
            //switch (this.inputDevice) {

            //    case InputDevice.GAMEPAD: {
                    // -----------GAMEPAD HANDLER ---------------------- 
            var state = Keyboard.GetState();
            if (state != null)
            {
                    

                //
                inputState.ArrowMovedLeft = state.IsKeyDown(Keys.Left) || KinectManager.getInstance().isMovingLeft();
                inputState.ArrowMovedRight = state.IsKeyDown(Keys.Right) || KinectManager.getInstance().isMovingRight();
                inputState.BallShoot = state.IsKeyDown(Keys.Space) || KinectManager.getInstance().isShooting();
                inputState.Continue = state.IsKeyDown(Keys.Enter);
                inputState.SoundOn = state.IsKeyDown(Keys.M);
                inputState.SoundOff = state.IsKeyDown(Keys.M);
            }

            var gamePadState = GamePad.GetState(PlayerIndex.One);
            if (gamePadState != null)
            {
                inputState.ArrowMovedLeft |= gamePadState.IsButtonDown(Buttons.DPadLeft);
                inputState.ArrowMovedRight |= gamePadState.IsButtonDown(Buttons.DPadRight);
                inputState.BallShoot |= gamePadState.IsButtonDown(Buttons.A);
                inputState.Continue |= gamePadState.IsButtonDown(Buttons.B);
            }

                //    break;
                //}
                //default:
                //{
                //    // -----------KEYBOARD HANDLER ---------------------- 

           
            //        break;
            //    }
            
            //}
          //  Console.WriteLine("InputManager : Update");
           // inputState = new PuzzleBobbleGameInputState();
            // -----------KEYBOARD HANDLER ----------------------
           // var state = Keyboard.GetState();

      //      inputState.ArrowMovedLeft = state.IsKeyDown(Keys.Left);
      //      inputState.ArrowMovedRight = state.IsKeyDown(Keys.Right);
      //      inputState.BallShoot = state.IsKeyDown(Keys.Space);
      //      inputState.Continue = state.IsKeyDown(Keys.Enter);
      //      inputState.SoundOn = state.IsKeyDown(Keys.M);
      //      inputState.SoundOff = state.IsKeyDown(Keys.M);
      ////      inputState.GamePaused = state.IsKeyDown(Keys.P);

            
            
            
            base.Update(gameTime);
        }
    }
}
