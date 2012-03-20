using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


using WindowsGame2;

namespace WindowsGame2.menu
{
    class ScoreBoard : MenuComponentContainer
    {
       // private const string title = "Top Scores";
        private IGameLogicService gameLogic;
        private  Menu menu;
        public ScoreBoard( Menu menu, Texture2D texture, SpriteFont font, Color color, Color focusCol)  {
           // WorldData2D gameWorld = (WorldData2D)Game.Services.GetService(typeof(WorldData2D));
            this.menu = menu;
            this.font = font;
            this.color = color;
            this.focusColor = focusCol;
            this.texture = texture;
            gameLogic = menu.Game.Services.GetService(typeof(IGameLogicService)) as IGameLogicService;
            this.title = "Top Scores";
            
        }
        override public void validate() { }
        //private GameLogic getGameLogic() {
        // //   Console.WriteLine("ScoreBoard: getGameLogic");
            
        //    if (gameLogic == null) {
        //        gameLogic = menu.game.Services.GetService(typeof(GameLogic)) as GameLogic;
        //    }
        //    return this.gameLogic;
        //}
        public override void paintComponent(SpriteBatch spriteBatch)
        {
            //this.getGameLogic();

           // Console.WriteLine("ScoreBoard: paintComponent");
            //base.paintComponent(spriteBatch);

            if (this.gameLogic  == null)
                return;

            IList<ScoreEntry>
               scoreList = gameLogic.getScoreList();


            if (this.expanded)
            {
                if (this.texture != null)
                    spriteBatch.Draw(this.texture, this.expandedBounds, Color.White);//this.getFather().getBounds(), Color.White);
                Point TopLeftMargin = new Point(this.bounds.X + 30, this.bounds.Y + 30);
                int scoreYDelta = 30;
                int count = 0;
                foreach (var score in scoreList)
                {

                    spriteBatch.DrawString(this.font, score.Score + " : " + score.PlayerName, new Vector2(TopLeftMargin.X, TopLeftMargin.Y + count * scoreYDelta),
                        Color.White, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
                    count++;
                }

            }
            else {
             //   Console.WriteLine("ScoreBoard not expanded"); 
                    Color c;
                    if (this.focus)
                    {
               //           Console.WriteLine("has focus setting color " + this.focusColor);
                        c = this.focusColor;
                    }
                    else
                    {
                        c = this.color;
                    }
                    Vector2 titleSize = this.font.MeasureString(this.title);
                    float x = this.bounds.X + (this.bounds.Width / 2.0f) - (titleSize.X / 2);

                    spriteBatch.DrawString(this.font, this.title , new Vector2(x, this.bounds.Y),//new Vector2(this.bounds.X, 20),// this.bounds.Y),
                            c, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
                
            }
        }
    }
}
