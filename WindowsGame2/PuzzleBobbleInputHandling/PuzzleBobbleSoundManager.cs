using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace PuzzleBobbleInputHandling.Sound
{
    public interface ISoundManagerService { 
    
    }
    public class PuzzleBobbleSoundManager 
    {
        private Game game;
        public enum SoundsEvent { BALL_SHOOT, ARROW_MOVED, BALL_EXPLOSION, WIN}
        private static SoundEffect OnBallShoot, OnArrowMoved,OnWin;
       // private PuzzleBobbleSoundManager() { }
        public static void playSound(SoundsEvent e) { 
            switch (e)
	        {

            case SoundsEvent.BALL_SHOOT: {
                OnBallShoot.Play();
                break;
            }
            case SoundsEvent.ARROW_MOVED: {
               // OnArrowMoved.Play();
                break;
            }
            case SoundsEvent.WIN: {
                OnWin.Play();
                break;
            }
		    default:
                break;
	        }
        }

        public PuzzleBobbleSoundManager(Game game)  {
            this.game = game;

            OnBallShoot = game.Content.Load<SoundEffect>("Sounds/waterdrop24");
            OnArrowMoved = game.Content.Load<SoundEffect>("Sounds/tick");
            OnWin = game.Content.Load<SoundEffect>("Sounds/youwin16");
        }
    }
}
