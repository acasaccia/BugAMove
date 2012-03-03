using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuzzleBobbleInputHandling
{
   
    class CircularGesture
    {
        private const int NE = 0;
        private const int SE = 1;
        private const int SW = 2;
        private const int NW = 3;
        //public enum Quadrant { NE, SE, SW, NW };
        private static int prev;
        private static int curr;

        private static int getCurrentQuadrant(float originX, float originY, float positionX, float positionY)
        {
            float diffX = positionX - originX;
            float diffY = positionY - originY;

            int ret = 0;
            if (diffX >= 0 && diffY >= 0) {
                ret = NE;
            }else
            if (diffX >= 0 && diffY < 0)
            {
                ret = SE;
            }else
            if (diffX < 0 && diffY >= 0)
            {
                ret = NW;
            }else
            if (diffX < 0 && diffY < 0)
            {
                ret = SW;
            }
            return ret;
        }
        public static KinectManager.Movement getMovementFromPosition(float originX, float originY, float positionX, float positionY)
        {
            prev = curr;

            System.Console.WriteLine("---------------------" );
            System.Console.WriteLine("prev " + curr);
            curr = getCurrentQuadrant(originX, originY, positionX, positionY);
            System.Console.WriteLine("curr " + curr);

            if (curr == 0 && prev == 3) {
                return KinectManager.Movement.RIGHT;
            }else if (curr ==3 && prev ==0){
                return KinectManager.Movement.LEFT;
            }else
            if (curr - prev > 0) {
                return KinectManager.Movement.RIGHT;
            }else
            if (curr - prev < 0)
            {
                return KinectManager.Movement.LEFT;
            }
            return KinectManager.Movement.IDLE;
            
        }
    }
}
