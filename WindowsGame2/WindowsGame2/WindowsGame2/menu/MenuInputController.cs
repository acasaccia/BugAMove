using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame2.menu
{
    public interface IMenuInputService {

        void pause();
        void start();
        event Action<MenuTraverser.Actions> menuAction;
    }
    class MenuInputController  : GameComponent, IMenuInputService
    {
       
        public event Action<MenuTraverser.Actions> menuAction;
        public event Action menuClosed;
        //public event EventHandler<EventArgs> actionPerformed;
        private bool running;
        private KeyboardState prev_kb = new KeyboardState();
        private GamePadState prev_gamepad = new GamePadState();
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
         //   Console.WriteLine("MenuInputController: Update");
            var state = Keyboard.GetState();
            //PuzzleBobbleSoundManager m;
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
    }
}
