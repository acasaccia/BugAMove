using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace WindowsGame2.Sound
{
    
    class MenuSoundManager : Microsoft.Xna.Framework.GameComponent
    {
        private static SoundEffect OnNavigateMenuUpDown, OnError;
        private Game1 game;
        public MenuSoundManager(Game game)
            : base(game)
        {
            this.game = (Game1)game;
        }
        public override void Initialize() {
            OnNavigateMenuUpDown = game.Content.Load<SoundEffect>("Sounds/water_dribble_07");
            OnError = game.Content.Load<SoundEffect>("Sounds/beep1000");
        }

        public static void playMoveUp() {
            OnNavigateMenuUpDown.Play();
        }
        public static void playMoveDown()
        {
            OnNavigateMenuUpDown.Play();
        }
        public static void playMoveForward()
        {
            //OnNavigateMenuUpDown.Play();
        }
        public static void playMoveBack()
        {
            //this.OnNavigateMenuUpDown.Play();
        }
        public static void playError()
        {
            OnError.Play();
            //this.OnNavigateMenuUpDown.Play();
        }
       


        protected override void Dispose(bool disposing) { }
    }
}
