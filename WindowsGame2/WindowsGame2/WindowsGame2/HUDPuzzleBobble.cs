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
           
            this.statsPosition1UP =  new Vector2( 0.02f * this.game.graphics.PreferredBackBufferWidth, 0.02f * this.game.graphics.PreferredBackBufferHeight);
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            this.timeFont = game.Content.Load<SpriteFont>("BigFont");
            this.statsFont = game.Content.Load<SpriteFont>("SimpleFont");
            base.LoadContent();
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public override void Draw(GameTime gameTime)
        {
            int time = (int)PuzzleBobble.game_state.LevelStatus.ElapsedTime.Value;// LevelStatus.ElapsedTime.Value;
            int score1UP = PuzzleBobble.game_state.LevelStatus.Score.Value; 
            //int score1UP = PuzzleBobble.game_state.Value.LevelStatus.Score.Value; // LevelStatus.Score.Value;
            Vector2 timeStringPosition = new Vector2(this.game.graphics.PreferredBackBufferWidth / 2 - (this.timeFont.MeasureString(time.ToString()).X / 2), 2.0f);
            float delta = this.statsFont.MeasureString("UP").Y;
            spriteBatch.Begin();
            
            spriteBatch.DrawString(this.timeFont, time.ToString(), timeStringPosition, this.timeColor, 0.0f, new Vector2(0, 0), new Vector2(1, 1), SpriteEffects.None, 0);
            spriteBatch.DrawString(this.statsFont, "--- 1UP STATS --- ", this.statsPosition1UP, this.statsTitleColor,
                0.0f, Vector2.Zero, Vector2.UnitX + Vector2.UnitY, SpriteEffects.None, 0);
            spriteBatch.DrawString(this.statsFont, "Score: "  + PuzzleBobble.game_state.LevelStatus.Score.Value, 
                this.statsPosition1UP + new Vector2(0.0f, delta ), this.statsTitleColor,
                0.0f, Vector2.Zero, Vector2.UnitX + Vector2.UnitY, SpriteEffects.None, 0);

            //Dictionary<string, PuzzleBobble.ColorCounter> d = PuzzleBobble.game_state.LevelStatus.AvailableColors.Value;
            
            spriteBatch.End();
        }

        //public Casanova.Variable<Microsoft.FSharp.Collections.FSharpMap<string, PuzzleBobble.ColorCounter>> c { get; set; }
    }
}
