using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
//using PuzzleBubble;



namespace WindowsGame2
{
    
    class Floor
    {
        public VertexPositionColorTexture[] vertices = new[]
      {
        new VertexPositionColorTexture(Vector3.Zero, Color.Blue, new Vector2(0,0)),
        new VertexPositionColorTexture(Vector3.Forward + Vector3.Right,  Color.Red, new Vector2(1,1)),
        new VertexPositionColorTexture(Vector3.Forward, Color.Red, new Vector2(0,1)),
        new VertexPositionColorTexture(Vector3.Right , Color.Green, new Vector2(1,0)),
      };

        public UInt16[] index = new UInt16[]{
           0,1,2,0,3,2
        };

        
    }
}
