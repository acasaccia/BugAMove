using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame2
{
    class SquareTextured
    {
        public Texture2D texture;
        //public VertexPositionColorTexture[] vertices = new[]{
        
        //    new VertexPositionColorTexture(Vector3.Left + Vector3.Up , Color.White, new Vector2(1,0)),
        //    new VertexPositionColorTexture(Vector3.Left + Vector3.Down , Color.White , new Vector2(0,0)),
        //    new VertexPositionColorTexture(Vector3.Right + Vector3.Down , Color.White,new Vector2(0,1)),
        //    new VertexPositionColorTexture(Vector3.Right + Vector3.Up , Color.White,new Vector2(1,1) )
            
        //};

        public UInt16[] index = new UInt16[]{
           0,2,1,1,2,3
        };
      public   VertexPositionColorTexture[] vertices = new[]
      {
        new VertexPositionColorTexture(-Vector3.Right - Vector3.Up, Color.Blue, new Vector2(0,0)),
        new VertexPositionColorTexture(Vector3.Right - Vector3.Up,  Color.Red, new Vector2(1,0)),
        new VertexPositionColorTexture(-Vector3.Right + Vector3.Up, Color.Red, new Vector2(0,1)),
        new VertexPositionColorTexture(Vector3.Right + Vector3.Up, Color.Green, new Vector2(1,1)),
      };

      //public   UInt16[] index = new[]
      //  {
      //    (UInt16)0,
      //    (UInt16)2,
      //    (UInt16)1,
      //    (UInt16)1,
      //    (UInt16)2,
      //    (UInt16)3
      //  };

        public SquareTextured(Texture2D texture) {

            this.texture = texture;
        }
    }
}
