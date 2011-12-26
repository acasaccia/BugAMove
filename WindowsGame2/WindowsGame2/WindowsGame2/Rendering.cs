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

      this.camera = new Camera(new Vector3(1.5f, 2.6f, 5.4f));
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
        Matrix[] transforms = new Matrix[this.arrow.Bones.Count];
        float aspectRatio = GraphicsDevice.Viewport.AspectRatio;

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        
        this.arrow.CopyAbsoluteBoneTransformsTo(transforms);
        ModelMesh cube = this.arrow.Meshes[4];
       // Vector3 distance = PuzzleBobble.game_state.ReadyBall.Value.center.toXNAVector - cube.BoundingSphere.Center;
        //Vector3 distance = PuzzleBobble.game_state.ReadyBall.Value.center.toXNAVector;
        Vector3 distance = PuzzleBobble.game_state.ReadyBall.Value.center.toXNAVector;
        distance.Z -= 1.3f;
        //Console.WriteLine("Bounding sphere : " + cube.BoundingSphere.Center + " rad " + cube.BoundingSphere.Radius);
        Matrix rot = Matrix.CreateRotationZ(rotation );
        Matrix t = Matrix.CreateTranslation(distance);
        for (int i = 0 ; i < this.arrow.Meshes.Count ; i++){
            ModelMesh mesh = this.arrow.Meshes[i];
            if (i == 3 || i ==4)
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
    private void DrawModel(Model m, Vector3 scale, Vector3 pos, Vector3 rotations, Nullable<Color> col)
    {
        Matrix[] transforms = new Matrix[m.Bones.Count];
        float aspectRatio = GraphicsDevice.Viewport.AspectRatio;
        
        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        m.CopyAbsoluteBoneTransformsTo(transforms);
        Matrix projection =
            Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60.0f),
            aspectRatio, 0.01f, 10000.0f);
       
       Matrix scaleMatrix = Matrix.CreateScale(scale);
      //  Matrix.
       Matrix translationMatrix = Matrix.CreateTranslation(pos);
       Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll( rotations.Y, rotations.X, rotations.Z);
        foreach (ModelMesh mesh in m.Meshes)
        {
          
            foreach (BasicEffect effect in mesh.Effects)
            {
                //new stuff
              //  mesh.BoundingSphere.
                if (col.HasValue)
                    effect.EmissiveColor = col.Value.ToVector3();
                
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
    BoundingSphere BoxMergedBoundingSphere;
    BoundingBox BoxBoundingBox;
    Model skyDome;
    //pillarBox;
    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);
      ballMesh = Game.Content.Load<Model>("sphere");
      arrow = Game.Content.Load<Model>("balestra");
      this.font = game.Content.Load<SpriteFont>("BigFont");
      Box = Game.Content.Load<Model>("Crate1");
 //     this.world = Game.Content.Load<Model>("RoadSign");
   
      //this.BoxMergedBoundingSphere = new BoundingSphere();
      //foreach (var mesh in Box.Meshes)
      //{
      //    this.BoxMergedBoundingSphere  = BoundingSphere.CreateMerged(this.BoxMergedBoundingSphere, mesh.BoundingSphere );
      //}
      //this.BoxMergedBoundingSphere = this.MergedBoundingSphere(Box);
      this.BoxBoundingBox = this.BoundingBoxFromVertex(Box);
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
        for (int i = 0; i < 4; i++)
        {
            this.DrawModel(Box, Vector3.One, new Vector3(-XSIZE /2.0f  , YSIZE + (YSIZE * 2 * i), 0.0f), Vector3.Zero, null);
            this.DrawModel(Box, Vector3.One, new Vector3(XSIZE / 2.0f + PuzzleBobble.BoxDimension.X, YSIZE + (YSIZE * 2 * i), 0.0f), Vector3.Zero, null);
          
        }
        float roofScaleX = PuzzleBobble.BoxDimension.X / XSIZE;
        this.DrawModel(Box, new Vector3(roofScaleX, 1.0f, 1.0f), new Vector3(XSIZE * roofScaleX / 2.0f, PuzzleBobble.BoxDimension.Y + YSIZE - PuzzleBobble.game_state.GridSteps.Value * PuzzleBobble.BallDiameter, 0.0f), Vector3.Zero, null);

       
        //pretty works 
        //for (int i = 0; i < 8; i++)
        //{
        //    this.DrawModel(Box, 1.0f, new Vector3(-size + ce.X, +size - ce.Y + (size * 2 * i), -size), Vector3.Zero, null);
        //    this.DrawModel(Box, 1.0f, new Vector3(PuzzleBobble.BoxDimension.X + size - ce.X, +size - ce.Y + (size * 2 * i), -size), Vector3.Zero, null);
        //}

        
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
