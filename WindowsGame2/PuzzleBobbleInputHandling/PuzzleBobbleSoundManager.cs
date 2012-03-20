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
        public enum SoundsEvent { BALL_SHOOT, BALL_DOCKED, ARROW_MOVED, BALL_EXPLOSION, WIN, ROOF_TICK, ROOF_DOWN}
        private static SoundEffect OnBallShoot, OnArrowMoved, OnWin, OnBallExplosion, OnBallDocked,clockTicking,doorSlam;
   
       // private PuzzleBobbleSoundManager() { }
        public static void playSound(SoundsEvent e) { 
            switch (e)
	        {

            case SoundsEvent.BALL_EXPLOSION:
            {
                OnBallExplosion.Play();
                break;       
            }
            case SoundsEvent.BALL_DOCKED:
            {
                OnBallDocked.Play();
                break;
            }
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
            case SoundsEvent.ROOF_TICK: {
                clockTicking.Play();
                break;
            }
            case SoundsEvent.ROOF_DOWN:
            {
                doorSlam.Play(0.75f, 0.0f, 0.0f);
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
            OnBallExplosion = game.Content.Load<SoundEffect>("Sounds/zing");
            OnBallDocked = game.Content.Load<SoundEffect>("Sounds/glug1");
            doorSlam = game.Content.Load<SoundEffect>("Sounds/doorSlam");
            clockTicking = game.Content.Load<SoundEffect>("Sounds/clockTicking");
        }
    }
}
