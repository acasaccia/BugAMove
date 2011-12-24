using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WindowsGame2;
using WindowsGame2.menu;
using WindowsGame2.Sound;
using PuzzleBobbleInputHandling;


namespace WindowsGame2
{

    public interface IMenuService { }
  /// <summary>
  /// This is a game component that implements IUpdateable.
  /// </summary>
  public class Menu : DrawableGameComponent , IMenuService
  {
      private SpriteFont font;
      private Texture2D menuBackground;
      private Texture2D menuListBackground, scoreBackground;

      private SoundEffect backgroundSong;
    
      private SoundEffectInstance backgroundSongInstace;
      public static bool soundEnabled = true;


      public Game1 game;
      private SpriteBatch spriteBatch;
      private RootMenuItem root;
      private MenuTraverser traverser;
      private IGameLogicService gameLogic;
      private IRendering3DService renderer;
      private MenuSoundManager soundManager;
      private IMenuInputService menuInputController;

    public Menu(Game game)
      : base(game)
    {
        Console.WriteLine("Menu Constructor");
        this.game =(Game1) game;
        //if (Game.Services.GetService(typeof(IMenuService)) != null)
        //    Console.WriteLine("IMenuService already exist...BUG");
        Game.Services.AddService(typeof(IMenuService) ,this);
     //   this.gameLogic = new GameLogic(game);
     //   this.gameLogic.running = true;
     //   Game.Components.Add(this.gameLogic);
       // game.Services.AddService(typeof(GameLogic), this.gameLogic);

        this.font = game.Content.Load<SpriteFont>("SimpleFont");
        this.menuBackground = game.Content.Load<Texture2D>("fractal");
        this.menuListBackground = game.Content.Load<Texture2D>("rootmenu");
        this.scoreBackground = game.Content.Load<Texture2D>("scorelistbg");

        this.root = new RootMenuItem(this.game.graphics.PreferredBackBufferHeight, this.game.graphics.PreferredBackBufferWidth, this.menuBackground);
        VerticalListMenuContainer list = new VerticalListMenuContainer("rootmenu", this.menuListBackground, this.font, Color.White, Color.Yellow);
        VerticalListMenuContainer sublist = new VerticalListMenuContainer("Info", this.menuListBackground, this.font, Color.White, Color.Yellow);

        ScoreBoard board = new ScoreBoard(this, this.scoreBackground, this.font, Color.White, Color.Yellow);
        sublist.addChildComponent(board);
        sublist.addChildComponent((new MenuEntryChoice("Credits", null, this.font, Color.White, Color.Yellow)));
       
        this.root.addChildComponent(list);
        MenuEntryChoice quickPlay = new MenuEntryChoice("Quick Play", null, this.font, Color.White, Color.Yellow);
        quickPlay.actionPerformed += this.StartLevel;
        this.root.getChild(0).addChildComponent(quickPlay);
        MenuEntryChoice loadGame = new MenuEntryChoice("Load Game", null, this.font, Color.White, Color.Yellow);
        loadGame.actionPerformed += this.LoadGame;
        list.addChildComponent(loadGame);

        MenuEntryChoice exitGame = new MenuEntryChoice("Exit Game", null, this.font, Color.White, Color.Yellow);
        exitGame.actionPerformed += this.ExitGame;
        this.root.getChild(0).addChildComponent(exitGame);

        this.root.getChild(0).addChildComponent(sublist);
       
        this.soundManager = new MenuSoundManager(game);
        Game.Components.Add(this.soundManager);
        Game.Services.AddService(typeof(MenuSoundManager), this.soundManager);

        
    }

    private void LoadGame()
    {
        
        this.StartLevel();
        this.gameLogic.OnLoadGame();
    }
    public override void Initialize()
    {
        this.traverser = new MenuTraverser(this.root);
        menuInputController = Game.Services.GetService(typeof(IMenuInputService)) as IMenuInputService;//new MenuInputController(game);
        menuInputController.menuAction += this.traverser.OnMenuAction;//(MenuTraverser.Actions action) => this.traverser.OnMenuAction(action);
        //menuInputController.menuAction += this.traverser.OnMenuActionCachedHandler ;

        menuInputController.start();

        this.gameLogic = Game.Services.GetService(typeof(IGameLogicService)) as IGameLogicService;
       // menuInputController.menuClosed += OnMenuClose;
        this.renderer = Game.Services.GetService(typeof(IRendering3DService)) as IRendering3DService;
        base.Initialize();
        spriteBatch = new SpriteBatch(GraphicsDevice);
    
    }
    protected override void Dispose(bool disposing)
    {
        Console.WriteLine("Menu: Disposed");
        Game.Services.RemoveService(typeof(IMenuService));

        this.menuInputController.menuAction -= this.traverser.OnMenuAction;//(action);//(MenuTraverser.Actions action) => this.traverser.OnMenuAction(action);
       // menuInputController.menuAction -= this.traverser.OnMenuActionCachedHandler;    
        //if (Game.Services.GetService(typeof(IMenuService)) != null)
        //    Console.WriteLine("IMenuService not removed");
        //else
        //    Console.WriteLine("IMenuService REMOVED");
       // Game.Services.
        //TODO: remove the stop call, should be someway implicit
        Game.Services.RemoveService(typeof(MenuSoundManager));
        this.backgroundSongInstace.Stop();
        base.Dispose(disposing);
    }
    protected override void LoadContent()
    {
        


       

      //  menuInputController = new MenuInputController(game);
      //  menuInputController.menuAction += (MenuTraverser.Actions action) => this.traverser.OnMenuAction(action);
      //  menuInputController.menuClosed += OnMenuClose;
        
       // game.Components.Add(menuInputController);
    
     //   game.Services.AddService(typeof(MenuInputController), menuInputController);

        this.backgroundSong = game.Content.Load<SoundEffect>("Sounds/wind_sound");
        this.backgroundSongInstace = this.backgroundSong.CreateInstance();
        this.backgroundSongInstace.IsLooped = true;
        this.backgroundSongInstace.Play();
       // Console.WriteLine("added menuInputController to services");

//       Game.Components.Add(new MenuInputController(game));
    }
    public void ExitGame() {
        Console.WriteLine("Menu: ExitGame");
        Game.Exit();
    }

    public void StartLevel() {
        Console.WriteLine("Menu: StartLevel");
     
        game.Services.RemoveService(typeof(MenuSoundManager));
        game.Services.RemoveService(typeof(MenuInputController));

        this.menuInputController.pause();
      
        InputManager inputManager = Game.Services.GetService(typeof(InputManager)) as InputManager;
        inputManager.start();
        Game.Components.Add(new HUDPuzzleBobble(this.game));
        renderer.start();


      
        Game.Components.Remove(this);
        this.Dispose();
        PuzzleBobble.setup_random_level();
        Casanova.commit_variable_updates();
        gameLogic.start();

    }
    public void OnMenuClose() {
        Console.WriteLine("Menu: OnMenuClosed");
        //while (Game.Components.Count > 0)
        //{
        //    ((GameComponent)Game.Components[0]).Dispose();
        //}

        //Game.Components.Clear();

        //if (this.gameLogic == null)
        //    this.gameLogic = new GameLogic(game);
        //if (this.renderer == null)
        //    this.renderer = new Rendering(game);


        //game.Services.AddService(typeof(ICameraActions), new InputController(this.game));
        // this.game.Services.AddService(typeof(ICameraActions), );
        GameLogicInputController inputController = new GameLogicInputController(this.game);
        InputManager inputManager = new InputManager(game);
        game.Components.Add(inputManager);

        game.Components.Add(inputController);
     //   game.Services.AddService(typeof(ICameraInputService), inputController);
   //     Game.Components.Add(this.gameLogic);
       // Game.Components.Add(this.renderer);

    
    }
    //public override void Update(GameTime gameTime)
    //{

    //    if (Keyboard.GetState().IsKeyDown(Keys.A))
    //    {
    //        while (Game.Components.Count > 0)
    //        {
    //            ((GameComponent)Game.Components[0]).Dispose();
    //        }

    //        Game.Components.Clear();

    //        //game.Services.AddService(typeof(ICameraActions), new InputController(this.game));
    //       // this.game.Services.AddService(typeof(ICameraActions), );
    //       InputController inputController = new InputController(this.game);
    //        game.Components.Add(inputController);
    //        game.Services.AddService(typeof(ICameraActions), inputController);
    //        Game.Components.Add(new GameLogic(Game));
    //        Game.Components.Add(new Rendering(Game));

    //    }

    //    base.Update(gameTime);
    //}

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
