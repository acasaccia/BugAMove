using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame2
{
    class HUDPuzzleBobble : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private SpriteFont timeFont, statsFont;
        private Color timeColor, statsTitleColor, statsColor;
        private Game1 game;
        Vector2 statsPosition1UP;
        public HUDPuzzleBobble(Game1 game)
            : base(game)
        {
            this.game = game;
            

            this.timeColor = Color.White;
            this.statsTitleColor = Color.White;
            this.statsColor = Color.White;
           
            this.statsPosition1UP =  new Vector2( 0.035f * this.game.graphics.PreferredBackBufferWidth, 0.02f * this.game.graphics.PreferredBackBufferHeight);
        }
        public override void Initialize()
        {
            base.Initialize();
        }


        Texture2D hud, hud2;
        Rectangle rect;
        Rectangle rect2;

        protected override void LoadContent()
        {
            hud = game.Content.Load<Texture2D>("hud");
            hud2 = game.Content.Load<Texture2D>("hud2");
            rect = new Rectangle(10, 20, 175, 100);
            rect2 = new Rectangle(this.game.GraphicsDevice.Viewport.Width - 185, 20, 175, 100);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.timeFont = game.Content.Load<SpriteFont>("BigFont");
            this.statsFont = game.Content.Load<SpriteFont>("HudFont");
            base.LoadContent();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override void Draw(GameTime gameTime)
        {
            //spriteBatch.Draw(hud, Vector2.Zero, Color.White);

            int time = (int)PuzzleBobble.game_state.LevelStatus.ElapsedTime.Value;
            string timeString = String.Format("{0:D2}:{1:D2}", (int)time/60, time % 60);

            int score1UP = PuzzleBobble.game_state.LevelStatus.Score.Value; 
            //int score1UP = PuzzleBobble.game_state.Value.LevelStatus.Score.Value; // LevelStatus.Score.Value;
            Vector2 timeStringPosition = new Vector2(0.85f * this.game.graphics.PreferredBackBufferWidth, 0.02f * this.game.graphics.PreferredBackBufferHeight);
            float delta = this.statsFont.MeasureString("UP").Y;
            spriteBatch.Begin();

            spriteBatch.Draw(hud, rect, Color.White);
            spriteBatch.Draw(hud2, rect2, Color.White);

            spriteBatch.DrawString(this.timeFont, "Time:\n" + timeString, timeStringPosition, this.timeColor, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
            //spriteBatch.DrawString(this.statsFont, "--- 1UP STATS --- ", this.statsPosition1UP, this.statsTitleColor,
            //    0.0f, Vector2.Zero, Vector2.UnitX + Vector2.UnitY, SpriteEffects.None, 0);
            spriteBatch.DrawString(this.timeFont, "Score:\n" + PuzzleBobble.game_state.LevelStatus.Score.Value, 
                statsPosition1UP, this.statsTitleColor,
                0.0f, Vector2.Zero, Vector2.UnitX + Vector2.UnitY, SpriteEffects.None, 0);

            //Dictionary<string, PuzzleBobble.ColorCounter> d = PuzzleBobble.game_state.LevelStatus.AvailableColors.Value;
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //public Casanova.Variable<Microsoft.FSharp.Collections.FSharpMap<string, PuzzleBobble.ColorCounter>> c { get; set; }
    }
}
