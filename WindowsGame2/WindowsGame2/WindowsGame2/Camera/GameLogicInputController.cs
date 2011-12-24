using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame2
{
 //   public delegate void CameraEvent(object sender, CameraEventArg args);
      
   

    public interface IGameLogicInputService
    {
        event Action<Camera.CameraTransformations, float > changedCamera;
        event Action gamePaused;
        event Action gameResumed;

    }
    class GameLogicInputController : GameComponent, IGameLogicInputService
    {
        public event Action<Camera.CameraTransformations, float > changedCamera;
        public event Action gamePaused;
        public event Action gameResumed;
     
        //CONSTRUCTOR
        public GameLogicInputController(Game game) : base(game) {

            Game.Services.AddService(typeof(IGameLogicInputService), this);
        }
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void Dispose(bool disposing)
        {
            Game.Services.RemoveService(typeof(IGameLogicInputService));
            base.Dispose(disposing);
        }
        KeyboardState prev_kb;
        GamePadState prev_game_pad_state;
        public override void Update(GameTime gameTime)
        {
           // Console.WriteLine("InputController : Update");
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         //   Console.WriteLine("dt " + dt);
            //KeyboardState keyboard = Keyboard.GetState();

            //if (keyboard.IsKeyDown(Keys.Space) &&
            //   prev_kb.IsKeyUp(Keys.Space))
            //    if (Shoot != null)
            //        Shoot();

            //prev_kb = keyboard;
           // Camera camera;
            var state = Keyboard.GetState();

            if (this.gamePaused != null) {
                if (state.IsKeyUp(Keys.P) && prev_kb.IsKeyDown(Keys.P)) {
                    this.gamePaused();
                }
            }
            //exist at least one handler listening for camera events..
            if (changedCamera != null) {
                if (state.IsKeyDown(Keys.I))
                    changedCamera(Camera.CameraTransformations.MoveForward, dt);
                if (state.IsKeyDown(Keys.K))
                    changedCamera(Camera.CameraTransformations.MoveBackward, dt);
                if (state.IsKeyDown(Keys.J))
                    changedCamera(Camera.CameraTransformations.MoveLeft, dt);
                if (state.IsKeyDown(Keys.L))
                    changedCamera(Camera.CameraTransformations.MoveRight, dt);
                if (state.IsKeyDown(Keys.Q))
                    changedCamera(Camera.CameraTransformations.MoveUp, dt);
                if (state.IsKeyDown(Keys.A))
                    changedCamera(Camera.CameraTransformations.MoveDown, dt);
                if (state.IsKeyDown(Keys.W))
                    changedCamera(Camera.CameraTransformations.PitchUp, dt);
                if (state.IsKeyDown(Keys.S))
                    changedCamera(Camera.CameraTransformations.PitchDown, dt);
                if (state.IsKeyDown(Keys.E))
                    changedCamera(Camera.CameraTransformations.RollAnticlockwise, dt);
                if (state.IsKeyDown(Keys.D))
                    changedCamera(Camera.CameraTransformations.RollClockwise, dt);
                if (state.IsKeyDown(Keys.R))
                    changedCamera(Camera.CameraTransformations.YawLeft, dt);
                if (state.IsKeyDown(Keys.F))
                    changedCamera(Camera.CameraTransformations.YawRight, dt);
            }
            var gamePadState = GamePad.GetState(PlayerIndex.One);
            if (gamePadState != null){
                if (gamePadState.IsButtonDown(Buttons.Start) && prev_game_pad_state.IsButtonDown(Buttons.Start)) {
                    this.gamePaused();
                }
                if ((gamePadState.IsButtonDown(Buttons.LeftThumbstickUp)))
                    changedCamera(Camera.CameraTransformations.MoveForward, dt);
                if ((gamePadState.IsButtonDown(Buttons.LeftThumbstickDown)))
                    changedCamera(Camera.CameraTransformations.MoveBackward, dt);
                if ((gamePadState.IsButtonDown(Buttons.LeftThumbstickLeft)))
                    changedCamera(Camera.CameraTransformations.MoveLeft, dt);
                if ((gamePadState.IsButtonDown(Buttons.LeftThumbstickRight)))
                    changedCamera(Camera.CameraTransformations.MoveRight, dt);
                if ((gamePadState.IsButtonDown(Buttons.RightShoulder)))
                    changedCamera(Camera.CameraTransformations.RollAnticlockwise, dt);
                if ((gamePadState.IsButtonDown(Buttons.LeftShoulder)))
                    changedCamera(Camera.CameraTransformations.RollClockwise, dt);
                if ((gamePadState.IsButtonDown(Buttons.RightTrigger)))
                    changedCamera(Camera.CameraTransformations.YawRight, dt);
                if ((gamePadState.IsButtonDown(Buttons.LeftTrigger)))
                    changedCamera(Camera.CameraTransformations.YawLeft, dt);
                if ((gamePadState.IsButtonDown(Buttons.RightThumbstickUp)))
                    changedCamera(Camera.CameraTransformations.PitchUp, dt);
                if ((gamePadState.IsButtonDown(Buttons.RightThumbstickDown)))
                    changedCamera(Camera.CameraTransformations.PitchDown, dt);
                prev_game_pad_state = gamePadState;

            }
            prev_kb = state;
            base.Update(gameTime);
        }



    }
}
