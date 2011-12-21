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
using Microsoft.FSharp.Core;


namespace WindowsGame2
{


    public interface IRendering3DService {
        void pause();
        void start();
    }

  /// <summary>
  /// This is a game component that implements IUpdateable.
  /// </summary>
  public class Rendering : DrawableGameComponent, IRendering3DService
  {
      //RenderingData rendering_data = new RenderingData();
      SpriteBatch spriteBatch;
      //Ball ball;
      //BallGrid grid;
      Game1 game;
      Vector2 viewPortDimension;

      Model ballMesh; //TODO : remove
      private SpriteFont font;
      Model arrow;
      Model box;
      private Matrix projection;
      public  Camera camera;

      private bool running;

      public void pause() {
          this.running = false;
      }
      public void start() 
      {
          this.running = true;
      }
    public Rendering(Game game)
      : base(game)
    {
     // game.Services.AddService(typeof(RenderingData), rendering_data);

      this.game = (Game1)game;
      this.pause();

      Game.Services.AddService(typeof(IRendering3DService), this);
      this.viewPortDimension = new Vector2(this.game.graphics.PreferredBackBufferWidth, this.game.graphics.PreferredBackBufferHeight);

      this.camera = new Camera(new Vector3(1.5f, 2.4f, 5.2f));
       // this.camera.setAbsolutePosition(new Vector3(1.5f, 2.4f, 5.2f ));
        this.camera.updateViewMatrix();
//        {X:1.519999 Y:2.419998 Z:5.259996}

           

      game.Services.AddService(typeof(Camera), this.camera);
      //game.Services.AddService(typeof(WorldData2D), worldData);
    }

    
    public override void Initialize()
    {

        

      //ball = (Ball)Game.Services.GetService(typeof(Ball));
      //grid = (BallGrid)Game.Services.GetService(typeof(BallGrid));

      
      base.Initialize();

      this.projection =
          Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60.0f),
          GraphicsDevice.Viewport.AspectRatio, 0.01f, 10000.0f);
    }


    private void DrawArrow(float rotation) {
        Matrix[] transforms = new Matrix[this.arrow.Bones.Count];
        float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        
        this.arrow.CopyAbsoluteBoneTransformsTo(transforms);
        ModelMesh cube = this.arrow.Meshes[4];
       // Vector3 distance = PuzzleBobble.game_state.ReadyBall.Value.center.toXNAVector - cube.BoundingSphere.Center;
        //Vector3 distance = PuzzleBobble.game_state.ReadyBall.Value.center.toXNAVector;
        Vector3 distance = PuzzleBobble.game_state.ReadyBall.Value.center.toXNAVector;
        distance.Z -= 1.5f;
        //Console.WriteLine("Bounding sphere : " + cube.BoundingSphere.Center + " rad " + cube.BoundingSphere.Radius);
        Matrix rot = Matrix.CreateRotationZ(rotation );
        Matrix t = Matrix.CreateTranslation(distance);
        for (int i = 0 ; i < this.arrow.Meshes.Count ; i++){
            ModelMesh mesh = this.arrow.Meshes[i];
            if (i == 3 )
                continue;

            

            foreach (BasicEffect effect in mesh.Effects)
            {
            
                effect.EnableDefaultLighting();
                effect.CurrentTechnique.Passes[0].Apply();
                effect.View = this.camera.viewMatrix;
                
                if (i != 4)
                    effect.World = transforms[mesh.ParentBone.Index] *  rot *t;//*
                else
                    effect.World = transforms[mesh.ParentBone.Index] *t;//*
                effect.Projection = this.projection;
            }
            mesh.Draw();
        }
    }
    private void DrawMessage(string message) {

        Vector2 messageDimension = this.font.MeasureString(message);
        Vector2 messagePosition = new Vector2(  this.game.graphics.PreferredBackBufferWidth / 2 - messageDimension.X / 2,
                                                this.game.graphics.PreferredBackBufferHeight / 2 - messageDimension.Y /2
                                                );
        spriteBatch.Begin();
        spriteBatch.DrawString(this.font, message,messagePosition , Color.Black, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
        spriteBatch.End();
    }
    private void DrawModel(Model m, float scale, Vector3 pos, Vector3 rotations, Color col)
    {
        Matrix[] transforms = new Matrix[m.Bones.Count];
        float aspectRatio = GraphicsDevice.Viewport.AspectRatio;
        
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        m.CopyAbsoluteBoneTransformsTo(transforms);
        Matrix projection =
            Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60.0f),
            aspectRatio, 0.01f, 10000.0f);
       
       Matrix scaleMatrix = Matrix.CreateScale(scale);
       Matrix translationMatrix = Matrix.CreateTranslation(pos);
       Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll( rotations.Y, rotations.X, rotations.Z);
        foreach (ModelMesh mesh in m.Meshes)
        {
          
            foreach (BasicEffect effect in mesh.Effects)
            {
                //new stuff
              //  mesh.BoundingSphere.
                effect.EmissiveColor = col.ToVector3();
                effect.EnableDefaultLighting();
                effect.CurrentTechnique.Passes[0].Apply();
                effect.View = this.camera.viewMatrix;
                
                effect.World = transforms[mesh.ParentBone.Index] *
                    scaleMatrix * rotationMatrix * translationMatrix;
                effect.Projection = projection;
               // effect.World = 0.0f *
                 //   transforms[mesh.ParentBone.Index] *
                   //Matrix.CreateScale(new Vector3(0.1f, 0.1f, 0.1f));
            }
            mesh.Draw();
        }
    }

    Model pillar;
    //pillarBox;
    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);
      ballMesh = Game.Content.Load<Model>("sphere");
      arrow = Game.Content.Load<Model>("balestra");
      this.font = game.Content.Load<SpriteFont>("BigFont");
      pillar = Game.Content.Load<Model>("Crate1");
      //pillar = Game.Content.Load<Model>("MysteryBlock");
      #if FOO       
      rendering_data.ball = Game.Content.Load<Texture2D>("ball_icon");
      rendering_data.MaxX = GraphicsDevice.Viewport.Width - rendering_data.ball.Width;
      rendering_data.MaxY = GraphicsDevice.Viewport.Height - rendering_data.ball.Height;
        
        //my stuff
      this.worldData.background = Game.Content.Load<Texture2D>("background");
      this.worldData.arrowTexture = Game.Content.Load<Texture2D>("arrow");
      this.worldData.ballStandardTexture = Game.Content.Load<Texture2D>("ball_white");

      
      //this.colored_effect = new BasicEffect(GraphicsDevice)
      //{
      //    VertexColorEnabled = true
      //    Texture = //
      // //   text
      //};

        /*
         loading texture for 2D
         */
      this.worldData.square = new SquareTextured(Game.Content.Load<Texture2D>("fractal"));
    
      //this.basic_effect.View = this.camera.viewMatrix;
      ballMesh = Game.Content.Load<Model>("sphere");
     // houseModel = Game.Content.Load<Model>("maison");

      this.emptyEffect = new BasicEffect(GraphicsDevice);
      this.emptyEffect.TextureEnabled = false;
      this.basic_effect = new BasicEffect(GraphicsDevice);
      //basic_effect.World = Matrix.Identity;
      //basic_effect.View = Matrix.Identity;
      //basic_effect.Projection = Matrix.Identity;


      vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorTexture), this.worldData.square.vertices.Length, BufferUsage.WriteOnly);
      vertexBuffer.SetData(this.worldData.square.vertices); // TODO: put this elsewhere..SetData should be called only once
      indexBuf = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, this.worldData.square.index.Length, BufferUsage.WriteOnly);
      indexBuf.SetData(this.worldData.square.index);

      this.basic_effect.Texture = this.worldData.square.texture;
      this.basic_effect.TextureEnabled = true;

      this.ground = Game.Content.Load<Model>("newspaper");
      this.building = Game.Content.Load<Model>("tower bridge");
#endif
        //CopyToBuffer();
      base.LoadContent();
    }

    protected override void Dispose(bool disposing)
    {
   //   Game.Services.RemoveService(typeof(RenderingData));
      Game.Services.RemoveService(typeof(WorldData2D));
      base.Dispose(disposing);
    }

    public override void Draw(GameTime gameTime)
    {
        if (!this.running)
            return;
        int h = GraphicsDevice.Viewport.Bounds.Height;
        int w = GraphicsDevice.Viewport.Bounds.Width;
        GraphicsDevice.Clear(Color.CornflowerBlue);

        int row = PuzzleBobble.game_state.Grid.Value.GetLength(0);
        int column = PuzzleBobble.game_state.Grid.Value.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // Microsoft.FSharp.Core.OptionModule.ToList<PuzzleBobble.Ball>( PuzzleBobble.game_state.Grid[row,column])
                if (PuzzleBobble.game_state.Grid.Value[i, j] != null)
                {

                    Vector3 pos = PuzzleBobble.game_state.Grid.Value[i, j].Value.center.toXNAVector;
                    float scale = PuzzleBobble.game_state.Grid.Value[i, j].Value.radius;
                    Color col = PuzzleBobble.game_state.Grid.Value[i, j].Value.color;
                    this.DrawModel(this.ballMesh, scale, pos, Vector3.Zero, col);

                    //       Console.WriteLine("ball_docked " + i + " " + j + " pos " + pos);
                }
            }
        }
        foreach (var climbingBall in PuzzleBobble.game_state.ClimbingBalls.Value)
        {
            
            Vector3 pos = climbingBall.center.toXNAVector;
            float scale = climbingBall.radius;
            Color col = climbingBall.color;
            this.DrawModel(this.ballMesh, scale, pos, Vector3.Zero, col);
       
        }
        foreach (var fallingBall in PuzzleBobble.game_state.FallingBalls.Value)
        {

            Vector3 pos = fallingBall.center.toXNAVector;
            float scale = fallingBall.radius;
            Color col = fallingBall.color;
            this.DrawModel(this.ballMesh, scale, pos, Vector3.Zero, col);

        }
       
        PuzzleBobble.Ball readyBall = PuzzleBobble.game_state.ReadyBall.Value;
        
        Vector3 p = readyBall.center.toXNAVector;
        float s = readyBall.radius;
        Color c = readyBall.color;
        this.DrawModel(this.ballMesh, s, p, Vector3.Zero, c);

        PuzzleBobble.Arrow arr = PuzzleBobble.game_state.Arrow;

        this.DrawArrow(arr.angle.Value);

       // Boupillar.Meshes[0].BoundingSphere
        float size = pillar.Meshes[0].BoundingSphere.Radius;
        Vector3 ce = pillar.Meshes[0].BoundingSphere.Center;
        for (int i = 0; i < 8; i++)
        {
            this.DrawModel(pillar, 1.0f, new Vector3(-size + ce.X, +size - ce.Y + (size * 2 * i), -size), Vector3.Zero, c);
            this.DrawModel(pillar, 1.0f, new Vector3(PuzzleBobble.BoxDimension.X +size - ce.X, +size - ce.Y + (size * 2 * i), -size), Vector3.Zero, c);
        }
        if (PuzzleBobble.game_state.LevelStatus.Status.Value == PuzzleBobble.GameStatus.Ready)
        {
            this.DrawMessage("Level Ready : Press Enter to start Game");
        }
        else if (PuzzleBobble.game_state.LevelStatus.Status.Value == PuzzleBobble.GameStatus.Win)
        {
            this.DrawMessage("Level Completed!");
        }
        else if (PuzzleBobble.game_state.LevelStatus.Status.Value == PuzzleBobble.GameStatus.Lost)
        {
            this.DrawMessage("You Lost!");
        }
        base.Draw(gameTime);


 

      
    }



    //public Casanova.Variable<FSharpList> list { get; set; }
  }
}
