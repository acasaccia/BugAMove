using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WindowsGame2;
namespace WindowsGame2
{
    class WorldData2D
    {
        public int height;
        public int width;
        public Texture2D background;
        public Texture2D arrowTexture;
        public Texture2D ballStandardTexture;
       // Arrow arrow;
       // public Ball activeBall;
        public SquareTextured square;
        private void init() {
           
        }
        public WorldData2D() {
            this.height = 800;
            this.width = 600;
            this.init();
        }
        public WorldData2D(int w, int h) {
            this.height = h;
            this.width = w;
            this.init();
        }
    }
    class Arrow {
        
        
        public Vector2 position;
        public Vector2 size;
        public float angle;
        public const float maxAngle = 85;
        public const float minAngle = - maxAngle;
        public Arrow() {

            //this.size = new Vector2(64, 64);
            this.size = new Vector2(8f,8f);
            this.angle = 0;
        }
    }
}
