using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame2
{
    class Rendere2D
    {
        VertexPositionColor[] vertices = new[]{
        
            new VertexPositionColor(Vector3.Left - Vector3.Up , Color.White),
            new VertexPositionColor(Vector3.Down - Vector3.Up , Color.White),
            new VertexPositionColor(Vector3.Down - Vector3.Up , Color.White),

            new VertexPositionColor(Vector3.Down - Vector3.Up , Color.White),
            new VertexPositionColor(Vector3.Down - Vector3.Up , Color.White),
            new VertexPositionColor(Vector3.Down - Vector3.Up , Color.White),
        };
        
        UInt16[] index = new UInt16 []{
            0,0,0,0
        };

        //UInt16[] indices_data = new []{
        //    0u,
        //    0u,
        //    0u,
        //    0u
        //};
        //duplication of vertex is a problem: wasting memory and decreasing performances while rasterizing -> IndexBuffer
        //a sort of malloc..still empty..need to fill it with data
        //VertexBuffer vertexBuffer = new VertexBuffer(GraphicsDevice){
        //    typeof(VertexPositionColor),
        //    vertices.Length,
        //    BufferUsage.WriteOnly,
        //}
        //vertexBuffer.setData();// load data into vertex buffer


        //just one Vertex Buffer and indexbuffer ..use only one set because expansive

        //texture : vertexpositioncolortexture

        //font.measureString .. string dimension
    }
}
