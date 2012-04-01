using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame2.menu
{
    public interface IGameMenuService {
  
    }
    class GameMenu : DrawableGameComponent , IGameMenuService
    {
        private IMenuInputService menuInputController;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private RootMenuItem root;
        private Texture2D menuListBackground;

        private Texture2D[] cursorAnimation;
        public int currentCursorIndex;

        public MenuEntryChoice resume, exit, save;
        private MenuTraverser traverser;
        Game1 game;
        public GameMenu(Game1 game ) : base(game) {
            this.game = game;
            Console.WriteLine("GameMenu");
            this.cursorAnimation = new Texture2D[5];

            this.font = Game.Content.Load<SpriteFont>("SimpleFont");
            //  this.menuBackground = game.Content.Load<Texture2D>("fractal");
            this.menuListBackground = Game.Content.Load<Texture2D>("rootmenu");

            this.root = new RootMenuItem(this.game.graphics.PreferredBackBufferHeight, this.game.graphics.PreferredBackBufferWidth, null);
            VerticalListMenuContainer list = new VerticalListMenuContainer("Game Paused", this.menuListBackground, this.font, Color.White, Color.Yellow);

            this.root.addChildComponent(list);
            resume = new MenuEntryChoice("Resume", null, this.font, Color.White, Color.Yellow);
            exit = new MenuEntryChoice("Back to Main Menu", null, this.font, Color.White, Color.Yellow);
            save = new MenuEntryChoice("Save Game", null, this.font, Color.White, Color.Yellow);


            this.root.getChild(0).addChildComponent(resume);
            this.root.getChild(0).addChildComponent(exit);
            this.root.getChild(0).addChildComponent(save);


            this.traverser = new MenuTraverser(this.root);

            Game.Services.AddService(typeof(IGameMenuService), this);
        }
        protected override void LoadContent()
        {
            this.cursorAnimation[1] = Game.Content.Load<Texture2D>("mouse_cursor_1");
            this.cursorAnimation[2] = Game.Content.Load<Texture2D>("mouse_cursor_2");
            this.cursorAnimation[3] = Game.Content.Load<Texture2D>("mouse_cursor_3");
            this.cursorAnimation[4] = Game.Content.Load<Texture2D>("mouse_cursor_4");
            this.cursorAnimation[0] = Game.Content.Load<Texture2D>("mouse_cursor_32_32");

            this.currentCursorIndex = 0;// this.cursorAnimation[0];
        }
        protected override void Dispose(bool disposing)
        {
            IMenuInputService menuInputController = Game.Services.GetService(typeof(IMenuInputService)) as IMenuInputService;
            if (menuInputController != null)
            {
                menuInputController.menuAction -= this.traverser.OnMenuAction;
                menuInputController.cursorAction -= this.traverser.onCursorAction;
                menuInputController.kinectAction -= this.traverser.onKinectAction;
                menuInputController.pause();
            }
            IGameLogicService gameLogic = Game.Services.GetService(typeof(IGameLogicService)) as IGameLogicService;
            if (gameLogic != null)
            {
                this.exit.actionPerformed -= gameLogic.OnBackToMenu;
                this.resume.actionPerformed -= gameLogic.OnResumeGame;
                this.save.actionPerformed -= gameLogic.OnSaveGame;
                
                
            }
            Game.Services.RemoveService(typeof(IGameMenuService));
            base.Dispose(disposing);
        }
        public override void Initialize()
        {
            IGameLogicService gameLogic = Game.Services.GetService(typeof(IGameLogicService)) as IGameLogicService;
            this.exit.actionPerformed += gameLogic.OnBackToMenu;
            this.resume.actionPerformed += gameLogic.OnResumeGame;
            this.save.actionPerformed += gameLogic.OnSaveGame;
            
            base.Initialize();

            this.menuInputController = Game.Services.GetService(typeof(IMenuInputService)) as IMenuInputService;

            menuInputController.cursorAction += this.traverser.onCursorAction;
            menuInputController.kinectAction += this.traverser.onKinectAction;
            menuInputController.menuAction += this.traverser.OnMenuAction;//(MenuTraverser.Actions action) => this.traverser.OnMenuAction(action); ;
            menuInputController.start();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Game.Components.Add(new MenuInputController(this.game));
        }
        
        public override void Draw(GameTime gameTime)
        {
            int elapsedTime = (int)(this.traverser.hoverTime) % 5;
            this.currentCursorIndex = elapsedTime;
            //GraphicsDevice.Clear(Color.Black);
            this.spriteBatch.Begin();
            root.paintComponent(this.spriteBatch);
            Point currMouseCoord = this.menuInputController.currentMouseCoord();
            this.spriteBatch.Draw(this.cursorAnimation[currentCursorIndex], new Rectangle(currMouseCoord.X, currMouseCoord.Y, this.cursorAnimation[currentCursorIndex].Width, this.cursorAnimation[currentCursorIndex].Height), Color.White);
         //   this.spriteBatch.Draw(this.cursorTexture, new Rectangle(mouseX, mouseY, this.cursorTexture.Width, this.cursorTexture.Height), Color.White);
            this.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
    
}
