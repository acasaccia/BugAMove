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
using PuzzleBobbleInputHandling.Sound;


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
      Model gear;
      private Matrix projection;
      public  Camera camera;
      private Vector3 cameraDefaultPosition;
      private IHUDService hud;

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

      // camera posizione iniziale
      this.cameraDefaultPosition = new Vector3(BoxBoundingBox.Max.X + PuzzleBobble.BoxDimension.X / 2.0f, 2.7f, 4.9f);
      this.camera = new Camera(this.cameraDefaultPosition);
      this.camera.updateViewMatrix();
 
      game.Services.AddService(typeof(Camera), this.camera);
      this.hud = game.Services.GetService(typeof(IHUDService)) as IHUDService;
    }

    
    public override void Initialize()
    {

        

      //ball = (Ball)Game.Services.GetService(typeof(Ball));
      //grid = (BallGrid)Game.Services.GetService(typeof(BallGrid));

      
      base.Initialize();

      this.projection =
          Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(58.0f), GraphicsDevice.Viewport.AspectRatio, 0.01f, 10000.0f);
          //Matrix.CreateOrthographic(8.0f, 6.0f, 0.01f, 10000.0f);
    }

    public BoundingBox CalculateBoundingBox(Model m_model)
    {

        // Create variables to hold min and max xyz values for the model. Initialise them to extremes
        Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Matrix[] m_transforms = new Matrix[m_model.Bones.Count];
        foreach (ModelMesh mesh in m_model.Meshes)
        {
            //Create variables to hold min and max xyz values for the mesh. Initialise them to extremes
            Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            // There may be multiple parts in a mesh (different materials etc.) so loop through each
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                byte[] vertexData = new byte[stride * part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1); // fixed 13/4/11

                // Find minimum and maximum xyz values for this mesh part
                // We know the position will always be the first 3 float values of the vertex data
                Vector3 vertPosition = new Vector3();
                for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                {
                    vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                    vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                    vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);

                    // update our running values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }

            // transform by mesh bone transforms
            meshMin = Vector3.Transform(meshMin, m_transforms[mesh.ParentBone.Index]);
            meshMax = Vector3.Transform(meshMax, m_transforms[mesh.ParentBone.Index]);

            // Expand model extents by the ones from this mesh
            modelMin = Vector3.Min(modelMin, meshMin);
            modelMax = Vector3.Max(modelMax, meshMax);
        }


        // Create and return the model bounding box
        return new BoundingBox(modelMin, modelMax);

    }
    private void DrawArrow(float rotation) {
        Vector3 distance = PuzzleBobble.game_state.ReadyBall.Value.center.toXNAVector;
        float smallGearScaleFactor = 0.015f;
        float mediumGearScaleFactor = 0.02f;
        float bigGearScaleFactor = 0.03f;
        float arrowScaleFactor = 0.008f;
        this.DrawModel(this.gear, new Vector3(smallGearScaleFactor), new Vector3(distance.X, distance.Y, distance.Z - 0.05f), new Vector3(0.0f, 0.0f, -rotation * 2.0f), null);
        this.DrawModel(this.gear, new Vector3(bigGearScaleFactor), new Vector3(distance.X + 0.5f, distance.Y - 0.6f, distance.Z + 0.1f), new Vector3(0.0f, 0.0f, -rotation * 0.1f), null);
        this.DrawModel(this.gear, new Vector3(mediumGearScaleFactor), new Vector3(distance.X - 0.5f, distance.Y - 0.4f, distance.Z + 0.05f), new Vector3(0.0f, 0.0f, rotation * 0.3f), null);
        this.DrawModel(this.arrow, new Vector3(arrowScaleFactor), new Vector3(distance.X, distance.Y, distance.Z), new Vector3(0.0f, 0.0f, rotation), null);
    }
    private void DrawMessage(string message) {

        Vector2 messageDimension = this.font.MeasureString(message);
        Vector2 messagePosition = new Vector2(
            this.game.graphics.PreferredBackBufferWidth / 2 - messageDimension.X / 2,
            this.game.graphics.PreferredBackBufferHeight / 2 - messageDimension.Y /2
        );
        spriteBatch.Begin();
        spriteBatch.DrawString(this.font, message,messagePosition , Color.White, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
        spriteBatch.End();
    }
    private void DrawModel(Model m, Vector3 scale, Vector3 pos, Vector3 rotations, Nullable<Color> col)
    {
        Matrix[] transforms = new Matrix[m.Bones.Count];
        float aspectRatio = GraphicsDevice.Viewport.AspectRatio;
        
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        m.CopyAbsoluteBoneTransformsTo(transforms);
       
       Matrix scaleMatrix = Matrix.CreateScale(scale);
      //  Matrix.
       Matrix translationMatrix = Matrix.CreateTranslation(pos);
       Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll( rotations.Y, rotations.X, rotations.Z);
        foreach (ModelMesh mesh in m.Meshes)
        {
            foreach (BasicEffect effect in mesh.Effects)
            {
                if (col.HasValue)
                {
                    effect.DiffuseColor = Color.DarkGray.ToVector3();
                    effect.EmissiveColor = col.Value.ToVector3();
                    effect.SpecularPower = 96.0f;
                }
                
                effect.EnableDefaultLighting();
                effect.CurrentTechnique.Passes[0].Apply();
                effect.View = this.camera.viewMatrix;
                
                effect.World = transforms[mesh.ParentBone.Index] * scaleMatrix * rotationMatrix * translationMatrix;
                effect.Projection = this.projection;
            }
            mesh.Draw();
        }
    }
    public BoundingBox BoundingBoxFromVertex(Model m_model)
    {
        // Create variables to hold min and max xyz values for the model. Initialise them to extremes
        Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Matrix[] m_transforms = new Matrix[m_model.Bones.Count];
        m_model.CopyAbsoluteBoneTransformsTo(m_transforms);
        foreach (ModelMesh mesh in m_model.Meshes)
        {
            //Create variables to hold min and max xyz values for the mesh. Initialise them to extremes
            Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            // There may be multiple parts in a mesh (different materials etc.) so loop through each
            foreach (ModelMeshPart part in mesh.MeshParts)
            {
                // The stride is how big, in bytes, one vertex is in the vertex buffer
                // We have to use this as we do not know the make up of the vertex
                int stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                byte[] vertexData = new byte[stride * part.NumVertices];
                part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1); // fixed 13/4/11

                // Find minimum and maximum xyz values for this mesh part
                // We know the position will always be the first 3 float values of the vertex data
                Vector3 vertPosition = new Vector3();
                for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                {
                    vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                    vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                    vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);

                    // update our running values from this vertex
                    meshMin = Vector3.Min(meshMin, vertPosition);
                    meshMax = Vector3.Max(meshMax, vertPosition);
                }
            }

            // transform by mesh bone transforms
            meshMin = Vector3.Transform(meshMin, m_transforms[mesh.ParentBone.Index]);
            meshMax = Vector3.Transform(meshMax, m_transforms[mesh.ParentBone.Index]);

            // Expand model extents by the ones from this mesh
            modelMin = Vector3.Min(modelMin, meshMin);
            modelMax = Vector3.Max(modelMax, meshMax);
        }


        // Create and return the model bounding box
        BoundingBox b = new BoundingBox(modelMin, modelMax);
        return b;
    }
    protected BoundingSphere MergedBoundingSphere(Model model)
    {
        BoundingSphere mergedSphere = new BoundingSphere();
        BoundingSphere[] boundingSpheres;
        int index = 0;
        int meshCount = model.Meshes.Count;

        boundingSpheres = new BoundingSphere[meshCount];
        foreach (ModelMesh mesh in model.Meshes)
        {
            boundingSpheres[index++] = mesh.BoundingSphere;
        }

        mergedSphere = boundingSpheres[0];
        if ((model.Meshes.Count) > 1)
        {
            index = 1;
            do
            {
                mergedSphere = BoundingSphere.CreateMerged(mergedSphere,
                    boundingSpheres[index]);
                index++;
            } while (index < model.Meshes.Count);
        }
        mergedSphere.Center.Y = 0;
        return mergedSphere;
    }

   
    Model Box;
    //BoundingBox BoxBounds;
    //BoundingSphere BoxMergedBoundingSphere;
    BoundingBox BoxBoundingBox;
    Texture2D background, background2;

  //  SoundEffect clockTicking, doorSlam;

    //Model skyDome;
    //pillarBox;
    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);
      ballMesh = Game.Content.Load<Model>("sphere");
      arrow = Game.Content.Load<Model>("arrow");
      gear = Game.Content.Load<Model>("gear");
      this.font = game.Content.Load<SpriteFont>("BigFont");
      Box = Game.Content.Load<Model>("cube");
      background = Game.Content.Load<Texture2D>("space");
      background2 = Game.Content.Load<Texture2D>("space-dust");

    
      this.BoxBoundingBox = this.BoundingBoxFromVertex(Box);
      base.LoadContent();
    }

    protected override void Dispose(bool disposing)
    {
   //   Game.Services.RemoveService(typeof(RenderingData));
   //   Game.Services.RemoveService(typeof(WorldData2D));
      base.Dispose(disposing);
    }

    int prevStep = 0;
    bool tickingPlaying = false;
    Random rnd = new Random();
    float tremblingOffset = 0.0f;
    public override void Draw(GameTime gameTime)
    {
        if (!this.running)
            return;

        if ((PuzzleBobble.game_state.LevelStatus.ElapsedTime.Value > 0.0f) && (PuzzleBobble.game_state.LevelStatus.ElapsedTime.Value % PuzzleBobble.DefaultRoofStepTimeDelta) > PuzzleBobble.DefaultRoofStepTimeDelta - 5.0f)
        {
            if (!tickingPlaying)
            {
                tickingPlaying = true;
                PuzzleBobbleSoundManager.playSound(PuzzleBobbleSoundManager.SoundsEvent.ROOF_TICK);
             //   this.clockTicking.Play(1.0f, 0.0f, 0.0f);
            }
            tremblingOffset = (float)(rnd.NextDouble() / 75.0f);
        }
        else
        {
            tickingPlaying = false;
            tremblingOffset = 0.0f;
        }

        if (this.prevStep < PuzzleBobble.game_state.GridSteps.Value)
        {
            PuzzleBobbleSoundManager.playSound(PuzzleBobbleSoundManager.SoundsEvent.ROOF_DOWN);
            //this.doorSlam.Play(0.75f, 0.0f, 0.0f);
        }

        this.prevStep = PuzzleBobble.game_state.GridSteps.Value;

        int h = GraphicsDevice.Viewport.Bounds.Height;
        int w = GraphicsDevice.Viewport.Bounds.Width;

        this.DrawBackground(gameTime);

        int row = PuzzleBobble.game_state.Grid.Value.GetLength(0);
        int column = PuzzleBobble.game_state.Grid.Value.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // Microsoft.FSharp.Core.OptionModule.ToList<PuzzleBobble.Ball>( PuzzleBobble.game_state.Grid[row,column])
                if (PuzzleBobble.game_state.Grid.Value[i, j] != null)
                {

                    Vector3 pos = PuzzleBobble.game_state.Grid.Value[i, j].Value.center.toXNAVector + new Vector3(0.0f, tremblingOffset, 0.0f);
                    float scale = PuzzleBobble.game_state.Grid.Value[i, j].Value.radius;
                    Color col = PuzzleBobble.game_state.Grid.Value[i, j].Value.color;
                    this.DrawModel(this.ballMesh, new Vector3 (scale,scale,scale), pos, Vector3.Zero, col);

                    //       Console.WriteLine("ball_docked " + i + " " + j + " pos " + pos);
                }
            }
        }
        foreach (var climbingBall in PuzzleBobble.game_state.ClimbingBalls.Value)
        {
            
            Vector3 pos = climbingBall.center.toXNAVector;
            float scale = climbingBall.radius;
            Color col = climbingBall.color;
            this.DrawModel(this.ballMesh, new Vector3(scale, scale, scale), pos, Vector3.Zero, col);
       
        }
        foreach (var fallingBall in PuzzleBobble.game_state.FallingBalls.Value)
        {

            Vector3 pos = fallingBall.center.toXNAVector;
            float scale = fallingBall.radius;
            Color col = fallingBall.color;
            this.DrawModel(this.ballMesh, new Vector3(scale, scale, scale), pos, Vector3.Zero, col);

        }
       
        PuzzleBobble.Ball readyBall = PuzzleBobble.game_state.ReadyBall.Value;
        
        Vector3 p = readyBall.center.toXNAVector;
        float s = readyBall.radius;
        Color c = readyBall.color;
        this.DrawModel(this.ballMesh, new Vector3(s, s, s), p, Vector3.Zero, c);

        PuzzleBobble.Arrow arr = PuzzleBobble.game_state.Arrow;

        this.DrawArrow(arr.angle.Value);

       
        float XSIZE = BoxBoundingBox.Max.X - BoxBoundingBox.Min.X;
        float YSIZE = BoxBoundingBox.Max.Y - BoxBoundingBox.Min.Y;
        float ZSIZE = BoxBoundingBox.Max.Z - BoxBoundingBox.Min.Z;
        for (int i = -2; i < 3; i++)
        {
            this.DrawModel(Box, new Vector3(1.0f, 1.0f, 0.2f), new Vector3(-XSIZE /2.0f - 0.2f , YSIZE + (YSIZE * i), 0.0f), Vector3.Zero, null);
            this.DrawModel(Box, new Vector3(1.0f, 1.0f, 0.2f), new Vector3(XSIZE / 2.0f + PuzzleBobble.BoxDimension.X + 0.2f, YSIZE + (YSIZE * i), 0.0f), Vector3.Zero, null);
          
        }
        float roofScaleX = PuzzleBobble.BoxDimension.X / ( XSIZE + 0.2f );
        this.DrawModel(Box, new Vector3(roofScaleX, 1.0f, 0.2f), new Vector3(XSIZE * roofScaleX / 2.0f, PuzzleBobble.BoxDimension.Y - PuzzleBobble.game_state.GridSteps.Value * PuzzleBobble.BallDiameter, 0.0f) + new Vector3(0.2f, tremblingOffset, 0.0f), Vector3.Zero, null);
        this.DrawModel(Box, new Vector3(roofScaleX, 1.0f, 0.2f), new Vector3(XSIZE * roofScaleX / 2.0f, PuzzleBobble.BoxDimension.Y + YSIZE - PuzzleBobble.game_state.GridSteps.Value * PuzzleBobble.BallDiameter, 0.0f) + new Vector3(0.2f, tremblingOffset, 0.0f), Vector3.Zero, null);
        this.DrawModel(Box, new Vector3(roofScaleX, 1.0f, 0.2f), new Vector3(XSIZE * roofScaleX / 2.0f + 0.2f, 0.0f - YSIZE, 0.0f), Vector3.Zero, null);

       
        //pretty works 
        //for (int i = 0; i < 8; i++)
        //{
        //    this.DrawModel(Box, 1.0f, new Vector3(-size + ce.X, +size - ce.Y + (size * 2 * i), -size), Vector3.Zero, null);
        //    this.DrawModel(Box, 1.0f, new Vector3(PuzzleBobble.BoxDimension.X + size - ce.X, +size - ce.Y + (size * 2 * i), -size), Vector3.Zero, null);
        //}

        if(this.hud == null)
            this.hud = game.Services.GetService(typeof(IHUDService)) as IHUDService;
        
        if (PuzzleBobble.game_state.LevelStatus.Status.Value == PuzzleBobble.GameStatus.Ready)
        {
            this.hud.printMessage("Press Enter or\nRaise your left hand\nto start");
        }
        else if (PuzzleBobble.game_state.LevelStatus.Status.Value == PuzzleBobble.GameStatus.Win)
        {
            this.hud.printMessage("You\nWin!");
        }
        else if (PuzzleBobble.game_state.LevelStatus.Status.Value == PuzzleBobble.GameStatus.Lost)
        {
            this.hud.printMessage("You\nLost!");
        }
        base.Draw(gameTime);


 

      
    }

    Vector2 cloudsPosition = new Vector2(0.0f);
    private void DrawBackground(GameTime gameTime)
    {
        cloudsPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * 96;
        cloudsPosition.Y = cloudsPosition.Y % this.background2.Height;

        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);

        spriteBatch.Draw(
            this.background,
            Vector2.Zero,
            new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
            Color.White
        );

        spriteBatch.Draw(
            this.background2,
            Vector2.Zero,
            new Rectangle(0, (int)cloudsPosition.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
            Color.White
        );

        spriteBatch.End();
    }



    //public Casanova.Variable<FSharpList> list { get; set; }
  }
}
