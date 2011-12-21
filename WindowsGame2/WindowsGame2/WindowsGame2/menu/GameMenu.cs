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
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private RootMenuItem root;
        private Texture2D menuListBackground;
        public MenuEntryChoice resume, exit, save;
        private MenuTraverser traverser;
        Game1 game;
        public GameMenu(Game1 game ) : base(game) {
            this.game = game;
            Console.WriteLine("GameMenu");

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
            
        }
        protected override void Dispose(bool disposing)
        {
            IMenuInputService menuInputController = Game.Services.GetService(typeof(IMenuInputService)) as IMenuInputService;
            if (menuInputController != null)
                menuInputController.menuAction -= this.traverser.OnMenuAction;//(MenuTraverser.Actions action) => this.traverser.OnMenuAction(action); ;
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

            IMenuInputService menuInputController = Game.Services.GetService(typeof(IMenuInputService)) as IMenuInputService;

            menuInputController.menuAction += this.traverser.OnMenuAction;//(MenuTraverser.Actions action) => this.traverser.OnMenuAction(action); ;
            menuInputController.start();
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Game.Components.Add(new MenuInputController(this.game));
        }
        
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            this.spriteBatch.Begin();
            root.paintComponent(this.spriteBatch);
            //  this.spriteBatch.DrawString(this.font, "hello world", new Vector2(0, 0), Color.Red, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
            this.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
    
}
