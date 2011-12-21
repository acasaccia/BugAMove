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
              Console.WriteLine("added score for " + score_entry.PlayerName);
              scores.Add(new ScoreEntry(score_entry.PlayerName, score_entry.PlayerScore));
	      }
          return scores;
          
      }

    public bool running;
    private PuzzleBobbleSoundManager soundManager;
    private ICameraInputService cameraInput; //TODO:move this into rendering component
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
        Console.WriteLine("GameLogic OnGamePause");
        if (this.running)
        {
            this.pause();
            gameMenu = new GameMenu(this.game);
            Game.Components.Add(this.gameMenu);
            InputManager input = Game.Services.GetService(typeof(InputManager)) as InputManager;
            input.pause();
            Game.Services.RemoveService(typeof(ICameraInputService));
            Game.Components.Remove((GameComponent)this.cameraInput);
        }
        else
            this.start();
    }

    public void OnBackToMenu() { 
        Console.WriteLine("GameLogic OnBackToMenu");

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
        Console.WriteLine("GameLogic OnResumeGame");
        Game.Components.Remove(this.gameMenu);
        Game.Services.RemoveService(typeof(IGameMenuService));
        this.gameMenu.Dispose();
        InputManager input = Game.Services.GetService(typeof(InputManager)) as InputManager;
        input.start();

        Game.Components.Add((GameComponent)this.cameraInput);
        Game.Services.AddService(typeof(ICameraInputService), this.cameraInput);
       
        this.start();
    }

    public void OnSaveGame() {
        Console.WriteLine("GameLogic OnSaveGame");
        using (var stream = File.Open("game_status.sav", FileMode.Create))
        {
            var binary_formatter = new BinaryFormatter();
          //  Casanova.commit_variable_updates();
            binary_formatter.Serialize(stream, PuzzleBobble.game_state );
            Console.WriteLine("SAVE ESEGUITO at gametime : " + PuzzleBobble.game_state.LevelStatus.ElapsedTime.Value.ToString());
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
                Console.WriteLine("State Loaded at gametime " + state.LevelStatus.ElapsedTime.Value.ToString());
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
        CameraInputController inputController = new CameraInputController(this.game);
        game.Components.Add(inputController);

    }

    protected override void Dispose(bool disposing)
    {
        Game.Components.Remove((CameraInputController)this.cameraInput);
        ((CameraInputController)this.cameraInput).Dispose();
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
        Console.WriteLine("GameLogic: Initialize");
 //       gameWorld = (WorldData2D)Game.Services.GetService(typeof(WorldData2D));
#if FOO
      if (!loaded)
      {
        ball.position = new Vector2(0.0f, 0.0f);
        ball.velocity = new Vector2(10.0f, -200.0f);//-150.0f);
        arrow.position = new Vector2((this.grid.gridDimension.X / 2) - (arrow.size.X / 2),
            this.grid.gridDimension.Y - arrow.size.Y);//new Vector2((gameWorld.width / 2) - (arrow.size.X /2), gameWorld.height - (arrow.size.Y ));
        Vector2 delta = arrow.size - grid.ballSize  ;
        gameWorld.activeBall = this.createNewBall();//new Ball(this.grid.ballSize, this.arrow.position+  delta / 2, new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), Color.Red);
      }

      rendering_data = (RenderingData)Game.Services.GetService(typeof(RenderingData));
#endif
      this.cameraInput = Game.Services.GetService(typeof(ICameraInputService)) as ICameraInputService;
      //if (cameraInput != null)
      //{
      if (this.cameraInput == null) {
          this.cameraInput = new CameraInputController(this.game);
          Game.Components.Add((CameraInputController)this.cameraInput);
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
      using (var stream = File.Open("savegame.sav", FileMode.Create))
      {
        var binary_formatter = new BinaryFormatter();
       // binary_formatter.Serialize(stream, ball);
      }
    }
#if FOO
    private Ball createNewBall() {
        Vector2 delta = arrow.size - grid.ballSize;
        Random rand = new Random();
        IList<Color> colors = this.grid.getAvailableColors();
        return new Ball(this.grid.ballSize, this.arrow.position + delta / 2, new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), colors[rand.Next(0,colors.Count)]);
    }

    private void fireActiveBall() {
        gameWorld.activeBall.velocity.X = (float)Math.Sin(-MathHelper.ToRadians(arrow.angle)) * -60;
        gameWorld.activeBall.velocity.Y = (float)Math.Cos(-MathHelper.ToRadians(arrow.angle)) * -60;//-500.0f;
    }

    private void moveActiveBall(float dt) {
        //if (gameWorld.activeBall.velocity.X == 0 && gameWorld.activeBall.velocity.Y == 0)
        //    return;
        //var k1 = gameWorld.activeBall.Derivative();
        //var k2 = (0.5f * gameWorld.activeBall + 0.5f * dt * k1).Derivative();
        //var k3 = (0.75f * gameWorld.activeBall + 0.75f * dt * k1).Derivative();

        //Ball ballNew;
        //ballNew = gameWorld.activeBall + (2.0f / 9.0f) * dt * k1 +
        //        (1.0f / 3.0f) * dt * k2 +
        //        (4.0f / 9.0f) * dt * k3;
        //gameWorld.activeBall.position = ballNew.position;
        //gameWorld.activeBall.velocity = ballNew.velocity;
        Vector2 newPos = gameWorld.activeBall.position + gameWorld.activeBall.velocity * dt;
        //gameWorld.activeBall.position = newPos;
       // gameWorld.activeBall.position = newPos;
        if (!this.grid.isFreeAt(newPos))
        {
            this.grid.insertBallAt(gameWorld.activeBall, gameWorld.activeBall.position);
            IList<Ball> visited = new List<Ball>();
        //    this.grid.visitSameColorBalls(gameWorld.activeBall, visited, gameWorld.activeBall.color);
            this.grid.cleanVisitedBalls();
            if (visited.Count > 3) {
                for (int i = 0; i < visited.Count; i++) {
                //    Console.WriteLine("i " + i);
                  //  this.grid.removeBallAt(visited[i].gridPosition.X, visited[i].gridPosition.Y);
                    this.grid.removeBallAt(visited[i].gridPosition);
                }
            }
            Vector2 delta = arrow.size - grid.ballSize;

            gameWorld.activeBall = this.createNewBall();
                //new Ball(this.grid.ballSize, new Vector2 (this.arrow.position.X + delta.X / 2, this.arrow.position.Y + delta.Y /2),
                //new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), Color.White);
            return;
        }
        else {
       //     Console.WriteLine("freepos");
            gameWorld.activeBall.position = newPos;
        }
        if (gameWorld.activeBall.position.X + gameWorld.activeBall.size.X> grid.gridDimension.X || // gameWorld.width ||
            gameWorld.activeBall.position.X < 0) {

                gameWorld.activeBall.velocity.X *= -1;
        }
    
    }
#endif
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
                        System.Threading.Thread.Sleep(5000);
                        this.OnBackToMenu();
                        break;
                    }
                case PuzzleBobble.GameStatus.Win:
                    {
                       
                        //System.Threading.Thread.Sleep(5000);
                        //PuzzleBobble.update_state(dt);
                        //PuzzleBobble.update_script();

                        //Casanova.commit_variable_updates();
                        System.Threading.Thread.Sleep(5000);
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
