using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBobbleInputHandling
{
    class HorizontalGesture
    {
        static float accumulator = 0;
        static  float delta = 0.03f;
        static float deltaOriginY = 0.4f;
        static float prevX = 0;
       
        public static KinectManager.Movement getMovementFromPosition(float originX, float originY, float positionX, float positionY)
        {
            //float diffX = positionX - originX;
            float diffX = positionX - prevX;
            accumulator += diffX;
            prevX = positionX;

            if (Math.Abs(originY - positionY) > deltaOriginY) {
                return KinectManager.Movement.IDLE;
            }
         //   System.Console.WriteLine("diff " + diffX + " acc " + accumulator);
            if (accumulator > delta) { 
                //move right
                accumulator = accumulator % delta;
                return KinectManager.Movement.RIGHT;
            }
            else if (accumulator < -delta)
            {
                accumulator = accumulator % delta;
                //accumulator *= -1;
                return KinectManager.Movement.LEFT;
            }
            return KinectManager.Movement.IDLE;
        }
    }
}
