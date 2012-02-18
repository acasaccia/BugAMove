using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WindowsGame2;
using WindowsGame2.Sound;
using WindowsGame2.menu;
using PuzzleBobbleInputHandling;
using PuzzleBobbleInputHandling.Sound;
//using PuzzleBobbleLogic;

namespace WindowsGame2
{

    public class ScoreEntry
    {
        public string PlayerName;
        public int Score;

        public ScoreEntry(string name, int score)
        {
            this.PlayerName = name;
            this.Score = score;
        }
    }
    public interface IGameLogicService
    {

        void resume();
        void start();
        void pause();

        void OnBackToMenu();
        void OnSaveGame();
        void OnLoadGame();
        void OnResumeGame();
        IList<ScoreEntry> getScoreList();
    }
  public class GameLogic : GameComponent , IGameLogicService
  {
    
      private IList<ScoreEntry> scores;

      public IList<ScoreEntry> getScoreList() {
          IList<ScoreEntry> scores = new List<ScoreEntry>();

          foreach (var score_entry in PuzzleBobble.game_state.LevelStatus.TopScores.Value)
	      {
           //   Console.WriteLine("added score for " + score_entry.PlayerName);
              scores.Add(new ScoreEntry(score_entry.PlayerName, score_entry.PlayerScore));
	      }
          return scores;
          
      }

    public bool running;
    private PuzzleBobbleSoundManager soundManager;
    private IGameLogicInputService cameraInput; //TODO:move this into rendering component
    private Game1 game;
    private GameMenu gameMenu;

    public void resume() { }
    public void start() {
        this.running = true;
    }
    public void pause() {
        this.running = false;
    }

    private void showGameMenu() { 
    
        
    }
    public void OnGamePaused() 
    { 
       // Console.WriteLine("GameLogic OnGamePause");
        if (this.running)
        {
            this.pause();
            gameMenu = new GameMenu(this.game);
            Game.Components.Add(this.gameMenu);
            InputManager input = Game.Services.GetService(typeof(InputManager)) as InputManager;
            input.pause();
            Game.Services.RemoveService(typeof(IGameLogicInputService));
            Game.Components.Remove((GameComponent)this.cameraInput);
        }
        else
            this.start();
    }

    public void OnBackToMenu() { 
     //   Console.WriteLine("GameLogic OnBackToMenu");

     //   Game.Components.Remove(this);
        Game.Components.Remove(this.gameMenu);
       // Game.Services.RemoveService(typeof(IGameMenuService));
  //      cameraInput.changedCamera -= this.OnCameraMoved;//(Camera.CameraTransformations t, float dt) => this.OnCameraMoved(t, dt);
    //    cameraInput.gamePaused -= this.OnGamePaused;
        if (this.gameMenu != null)
            this.gameMenu.Dispose();
        this.gameMenu = null;
        this.Initialize();
    //    this.Dispose();
        Menu menu = new Menu(this.game);
        Game.Components.Add(menu);
       // Game.Services.RemoveService(typeof(IGameLogicService));
        Camera cam = Game.Services.GetService(typeof(Camera)) as Camera;
        cam.resetCamera();
        //Game.Services.AddService(IMenuService)

    }
    public void OnResumeGame() 
    {
   //     Console.WriteLine("GameLogic OnResumeGame");
        Game.Components.Remove(this.gameMenu);
        Game.Services.RemoveService(typeof(IGameMenuService));
        this.gameMenu.Dispose();
        InputManager input = Game.Services.GetService(typeof(InputManager)) as InputManager;
        input.start();

        Game.Components.Add((GameComponent)this.cameraInput);
        Game.Services.AddService(typeof(IGameLogicInputService), this.cameraInput);
       
        this.start();
    }

    public void OnSaveGame() {
     //   Console.WriteLine("GameLogic OnSaveGame");
        using (var stream = File.Open("game_status.sav", FileMode.Create))
        {
            var binary_formatter = new BinaryFormatter();
          //  Casanova.commit_variable_updates();
            binary_formatter.Serialize(stream, PuzzleBobble.game_state );
            //Console.WriteLine("SAVE ESEGUITO at gametime : " + PuzzleBobble.game_state.LevelStatus.ElapsedTime.Value.ToString() + "score len " + 
            //    PuzzleBobble.game_state.LevelStatus.TopScores.Value.Length);
            stream.Close();
        }
        
    }
    public void OnLoadGame() {
        using (var stream = File.Open("game_status.sav", FileMode.Open))
        {
            var binary_formatter = new BinaryFormatter();
            try
            {
                PuzzleBobble.GameState state = binary_formatter.Deserialize(stream) as PuzzleBobble.GameState;
                PuzzleBobble.load_game_state(state);
             //  Casanova.commit_variable_updates();
              //  Console.WriteLine("State Loaded at gametime " + state.LevelStatus.ElapsedTime.Value.ToString());
            }
            catch (ArgumentNullException e) {
                Console.WriteLine(e.Message);  
            }
            stream.Close();
            
        }
    }
    public GameLogic(Game game)
      : base(game)
    {

        this.game = (Game1)game;
        Game.Services.AddService(typeof(IGameLogicService), this);
        bool useFile = false;
        this.running = false;
        this.soundManager = new PuzzleBobbleSoundManager(this.game);
        GameLogicInputController inputController = new GameLogicInputController(this.game);
        game.Components.Add(inputController);

    }

    protected override void Dispose(bool disposing)
    {
        Game.Components.Remove((GameLogicInputController)this.cameraInput);
        ((GameLogicInputController)this.cameraInput).Dispose();
     // Game.Services.RemoveService(typeof(Ball));
     // Game.Services.RemoveService(typeof(Arrow));
        Game.Components.Remove(this);
        Game.Services.RemoveService(typeof(IGameLogicService));
        if (this.gameMenu != null) {
            Game.Components.Remove(this.gameMenu);
            this.gameMenu.Dispose();
            Game.Services.RemoveService(typeof(IGameMenuService));
        }
      base.Dispose(disposing);
      
   //   GameEntity f = new GameEntity(new PuzzleBubble.Vector3<m>(3m,3m,3m) );
    }

    
    public override void Initialize()
    {
      //      Console.WriteLine("GameLogic: Initialize");

      this.cameraInput = Game.Services.GetService(typeof(IGameLogicInputService)) as IGameLogicInputService;
      //if (cameraInput != null)
      //{
      if (this.cameraInput == null) {
          this.cameraInput = new GameLogicInputController(this.game);
          Game.Components.Add((GameLogicInputController)this.cameraInput);
      }
      cameraInput.changedCamera += this.OnCameraMoved;//(Camera.CameraTransformations t, float dt) => this.OnCameraMoved(t, dt);
      cameraInput.gamePaused += this.OnGamePaused;

      //}
      Game.Exiting += new EventHandler<EventArgs>(Game_Exiting);
      PuzzleBobble.setup_random_level();
    //  Casanova.commit_variable_updates();
          //_level();
      base.Initialize();
    }


   
//      public void foo(Camera.CameraTransformations t){}
      public void OnCameraMoved(Camera.CameraTransformations t, float dt)
    {
       // Console.WriteLine("GameLogic.OnCameraMoved");
        Camera cam = Game.Services.GetService(typeof(Camera)) as Camera;
          
        if (cam != null)
        {
       //     Console.WriteLine("not null renderer");
            cam.transform(t, dt);
        }
    }
    public static void Game_Exiting(object sender, EventArgs e)
    {
        //using (var stream = File.Open("savegame.sav", FileMode.Create))
        //{
        //    var binary_formatter = new BinaryFormatter();
        //    binary_formatter.Serialize(stream, ball);
        //}
    }

    public override void Update(GameTime gameTime)
    {
        //if (InputManager.getState().GamePaused)
        //    this.pause();
       // Console.WriteLine("GameLogic: Update");
        if (this.running)
        {
            
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (PuzzleBobble.game_state.LevelStatus.Status.Value)
            {

                case PuzzleBobble.GameStatus.Lost:
                    {
                      
                        //PuzzleBobble.update_state(dt);
                        //PuzzleBobble.update_script();

                        //Casanova.commit_variable_updates();
                        //FIXME: remove thread sleep with an idle cycle
                        System.Threading.Thread.Sleep(5000);
                        this.pause();
                        this.OnBackToMenu();
                        break;
                    }
                case PuzzleBobble.GameStatus.Win:
                    {
                       
                        //System.Threading.Thread.Sleep(5000);
                        PuzzleBobble.update_state(dt);
                        PuzzleBobble.update_script();

                        Casanova.commit_variable_updates();
                        System.Threading.Thread.Sleep(5000);
                        this.pause();
                        this.OnBackToMenu();
                        break;
                    }
                default:
                    {
                      
                        //n    Console.WriteLine("GameLogic: update");
                        PuzzleBobble.update_state(dt);
                        PuzzleBobble.update_script();

                        Casanova.commit_variable_updates();

                        break;
                    }
            }

        //    if (PuzzleBobble.game_state.LevelStatus.GameStatus.Value != PuzzleBobble.GameStatus.Win &&
        //        PuzzleBobble.game_state.LevelStatus.GameStatus.Value != PuzzleBobble.GameStatus.Lost)
        //    {
        //        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        //        //n    Console.WriteLine("GameLogic: update");
        //        PuzzleBobble.update_state(dt);
        //        PuzzleBobble.update_script();

        //        Casanova.commit_variable_updates();

        //    }
        //    else {

        //        System.Threading.Thread.Sleep(5000);
        //        this.OnBackToMenu();
        //    }
            
        }
        base.Update(gameTime);
#if FOO
      if(rendering_data == null)
        rendering_data = (RenderingData)Game.Services.GetService(typeof(RenderingData));

      if (gameWorld == null) {
          gameWorld = (WorldData2D) Game.Services.GetService(typeof(WorldData2D));
      }
        if (Keyboard.GetState().IsKeyDown(Keys.B))
      //if (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed)
      {
        while (Game.Components.Count > 0)
        {
          ((GameComponent)Game.Components[0]).Dispose();
        }

        Game.Components.Clear();

        Game.Components.Add(new Menu(Game));
      }

        //rotate left arrow
        int delta = 2;
        if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
            if (arrow.angle - delta > Arrow.minAngle)
                arrow.angle -= delta;
        }else if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            if (arrow.angle + delta < Arrow.maxAngle)
                arrow.angle += delta;
            //Console.WriteLine("arrow.anngle " + arrow.angle);
        }
        else if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
            this.fireActiveBall();
        }

  //    ball.position = ball.position + ball.velocity * dt;
  //    ball.velocity = ball.velocity + Ball.G * dt;

      var k1 = ball.Derivative();
      var k2 = (0.5f * ball  + 0.5f * dt * k1).Derivative();
      var k3 = (0.75f * ball + 0.75f * dt * k1).Derivative();

      Ball ballNew;
      ballNew = ball + (2.0f / 9.0f) * dt * k1 +
              (1.0f / 3.0f) * dt * k2 +
              (4.0f / 9.0f) * dt * k3;
      ball.position = ballNew.position;
      ball.velocity = ballNew.velocity;
     
      if (ball.position.Y > rendering_data.MaxY)
      {
        ball.position.Y = rendering_data.MaxY;
       
           ball.velocity.Y *= -0.8f;
      }
        
      this.moveActiveBall(dt);
#endif    

      
    }
  }
}
