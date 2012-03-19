using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame2.menu
{
    public interface IMenuInputService {

        void pause();
        void start();
        event Action<MenuTraverser.Actions> menuAction;
        event Action<MenuTraverser.Actions, Point> cursorAction;
        event Action<MenuTraverser.Actions, Point, float> kinectAction;
        Point currentMouseCoord();
        bool isMouseInputEnabled();
    }
    class MenuInputController  : GameComponent, IMenuInputService
    {
       
        public event Action<MenuTraverser.Actions> menuAction;
        public event Action<MenuTraverser.Actions, Point> cursorAction;
        public event Action<MenuTraverser.Actions, Point, float> kinectAction;
        public event Action menuClosed;
     
        private bool running;
        private KeyboardState prev_kb = new KeyboardState();
        private GamePadState prev_gamepad = new GamePadState();

     
        private bool mouseInputEnabled;
        Point currMouseCoord;
        Point prevMouseCoord;

        private MouseState mouseState;

        public  Point currentMouseCoord() {
            return currMouseCoord;
        }
        public  bool isMouseInputEnabled()
        {
            return mouseInputEnabled;
        }
        public void pause()
        {
            this.running = false;
        }
      
        public void start() {
            this.running = true;
        }
        public MenuInputController(Game game)
            : base(game)
        {
            this.pause();
            //this.cachedHandler
            this.mouseInputEnabled = true;
            Game.Services.AddService(typeof(IMenuInputService), this);
        }
        protected override void Dispose(bool disposing)
        {
            Game.Services.RemoveService(typeof(IMenuInputService));
            base.Dispose(disposing);
        }
        public override void Update(GameTime gameTime)
        {
            if (!this.running)
                return;
           
            //------------- KINECT HANDLER ----------------------------

            if ( PuzzleBobbleInputHandling.KinectManager.getInstance().isTracking() )
            {
                prevMouseCoord = currMouseCoord;

                currMouseCoord = this.convertKinectCoordsToViewport( PuzzleBobbleInputHandling.KinectManager.getInstance().getRightHandPosition() );

                //Console.WriteLine("-----------");
                //Console.WriteLine(rh.X);
                //Console.WriteLine(rh.Y);

                if (kinectAction != null)
                    kinectAction(MenuTraverser.Actions.KINECT_HOVERING, currMouseCoord, (float)gameTime.ElapsedGameTime.TotalSeconds);

            }
            else if (mouseInputEnabled && cursorAction != null)
            {

                //------------- MOUSE HANDLER ----------------------------

                mouseState = Mouse.GetState();

                prevMouseCoord = currMouseCoord;

                currMouseCoord.X = mouseState.X;
                currMouseCoord.Y = mouseState.Y;

                //TODO: check if button is pressed
                if (menuAction != null)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                        menuAction(MenuTraverser.Actions.ACTION_PERFORMED);
                    else if (mouseState.RightButton == ButtonState.Pressed)
                        menuAction(MenuTraverser.Actions.MOVE_BACKWARD);
                }

                //if (prevMouseCoord != currMouseCoord)
                if (cursorAction != null)
                    cursorAction(MenuTraverser.Actions.MOUSE_MOVED, currMouseCoord);

            }


            //------------- KEYBOARD HANDLER ----------------------------
            var state = Keyboard.GetState();
          
            if (menuAction != null) {

                
              //  Console.WriteLine("not null MenuAction");
                if (state.IsKeyUp(Keys.Up) && prev_kb.IsKeyDown(Keys.Up))
                    menuAction(MenuTraverser.Actions.MOVE_UP);
                if (state.IsKeyUp(Keys.Down) && prev_kb.IsKeyDown(Keys.Down))
                    menuAction(MenuTraverser.Actions.MOVE_DOWN);
                if (state.IsKeyUp(Keys.Left) && prev_kb.IsKeyDown(Keys.Left))
                    menuAction(MenuTraverser.Actions.MOVE_BACKWARD);
                if (state.IsKeyUp(Keys.Right) && prev_kb.IsKeyDown(Keys.Right))
                    menuAction(MenuTraverser.Actions.MOVE_FORWARD);
                if (state.IsKeyUp(Keys.Enter) && prev_kb.IsKeyDown(Keys.Enter))
                    menuAction(MenuTraverser.Actions.ACTION_PERFORMED);

                //------------- GAMEPAD HANDLER ----------------------------
          
                var gamePadState = GamePad.GetState(PlayerIndex.One);
                if (gamePadState != null) {
                    if (gamePadState.IsButtonUp(Buttons.DPadDown) && prev_gamepad.IsButtonDown(Buttons.DPadDown))
                        menuAction(MenuTraverser.Actions.MOVE_DOWN);
                    if (gamePadState.IsButtonUp(Buttons.DPadUp) && prev_gamepad.IsButtonDown(Buttons.DPadUp))
                        menuAction(MenuTraverser.Actions.MOVE_UP);
                    if (gamePadState.IsButtonUp(Buttons.DPadLeft) && prev_gamepad.IsButtonDown(Buttons.DPadLeft))
                        menuAction(MenuTraverser.Actions.MOVE_BACKWARD);
                    if (gamePadState.IsButtonUp(Buttons.DPadRight) && prev_gamepad.IsButtonDown(Buttons.DPadRight))
                        menuAction(MenuTraverser.Actions.MOVE_FORWARD);
                    if (gamePadState.IsButtonUp(Buttons.B) && prev_gamepad.IsButtonDown(Buttons.B))
                        menuAction(MenuTraverser.Actions.ACTION_PERFORMED);
                    prev_gamepad = gamePadState;
                }
                
            }
            
            if (menuClosed != null){
                if (state.IsKeyUp(Keys.Escape) && prev_kb.IsKeyDown(Keys.Escape))
                    menuClosed();
            }
            
            prev_kb = state;

            base.Update(gameTime);
        }

        private Point convertKinectCoordsToViewport(Vector2 vector2)
        {
            Point point;
            point.X = (int)((vector2.X + 1.0f) / 2.0f * this.Game.GraphicsDevice.Viewport.Width);
            point.Y = this.Game.GraphicsDevice.Viewport.Height - (int)((vector2.Y + 1.0f) / 2.0f * this.Game.GraphicsDevice.Viewport.Height);
            return point;
        }
        public override void Initialize()
        {
            base.Initialize();
            System.Console.WriteLine("MenuInputController:Initialize");
          //  spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        
    }
}
