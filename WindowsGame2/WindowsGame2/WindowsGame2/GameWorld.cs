using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame2
{
#if FOO
    class BallGrid {

        public static IList<Color> availableColors = new List<Color> { Color.Red,  Color.Blue, Color.Green, Color.Yellow, Color.Gray};
        public int ROW = 30;
        public int COL = 15;
        public  Vector2 gridSize = new Vector2(800,600);
        public  Vector2 ballSize;// = new Vector2(800 / COL, 600 / ROW);
       // public Vector2 ballScale;
        public Vector2 gridDimension = new Vector2(100, 75);

        public Ball [,]grid;
        private IList<IList<Ball>> g; // TODO: change with grid

        private Point convertCoordToCell(Vector2 coord) {
            
            if (coord.X < 0 || coord.X > 800 || coord.Y < 0 || coord.Y > 600)
                return new Point(-1, -1);

            int x = (int)(coord.X / ballSize.X);
            int y = (int)(coord.Y / ballSize.Y);
            int col;
            int row = (int)(coord.Y / ballSize.Y);
            if (row % 2 == 0)
            {
                col = (int)(coord.X / ballSize.X);
            }
            else {
                col = (int)((coord.X - ballSize.X / 2)/ ballSize.X);
            }
            return new Point(col, row);
            //return new Point(x, y);
            
        }
        public Ball getBallAt(Vector2 pos) {
            Point cell = this.convertCoordToCell(pos);
            return this.grid[cell.Y, cell.X];
        }
        public IList<Color> getAvailableColors() {
            return availableColors;
        }
        public TreeNode<Ball> createNeighboursTree() {
            return null;
        }
        public void removeBallAt(Point cell) {
            int row = cell.X;
            int col = cell.Y;
            this.removeBallAt(row, col);
        }
        public void removeBallAt(int row, int col) {
         //   Console.WriteLine("removing ball row " + row + " col " + col);
            if (this.grid[row, col] != null) {
               
                this.grid[row, col].detachFromAllBalls();
                this.grid[row, col] = null;
            }
        }
        public IList<Point> getSameColorNeighbours(Point cell, IList<Point> list) {
            //if (cell.X < 0 || cell.X > this.gr)
            //if (list == null) {
            //    list = new List<Point>();
            //}
            //Color c = this.grid[cell.Y, cell.X].color;
            return null;
        }
        public void cleanVisitedBalls() {
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++) {
                    if (this.grid[i, j] != null) {
                        this.grid[i, j].visited = false;
                    }
                }
            }
        }
        public void visitSameColorBalls(Ball b, IList<Ball> sameColor, Color col) {
            if (b.visited)
                return;
            else
                b.visited = true;
            if (b.color.Equals(col))
            {
                sameColor.Add(b);
          //      b.color = Color.Black;
            //    Console.WriteLine("visited x " + b.gridPosition.X + " y " + b.gridPosition.Y);
              //  System.Threading.Thread.Sleep(10);
           //     Console.WriteLine("same color count " + sameColor.Count);
            }
            else
                return;
            for (int i = 0; i < b.neighbours.Count; i++) {
                this.visitSameColorBalls(b.neighbours[i], sameColor, col);
                
            }
            //if (gridthis.grid[b.gridPosition.Y, b.gridPosition.X] )
        }
        private void attachNewBallToNeighbours(int row, int col) {

            
            if (row == 0 && col == 0) {
                this.grid[row, col].attachBall(this.grid[row , col + 1]);
                this.grid[row, col].attachBall(this.grid[row + 1, col ]);
            }
            else if (row == 0 && col == COL - 1) {
                this.grid[row, col].attachBall(this.grid[row, col - 1]);
                this.grid[row, col].attachBall(this.grid[row + 1, col - 1]);
            }else if(row == ROW -1){
                //should never be reached..player lost
            }
            else if (row == 0) {
                this.grid[row, col].attachBall(this.grid[row, col + 1]);
                this.grid[row, col].attachBall(this.grid[row, col - 1]);
                this.grid[row, col].attachBall(this.grid[row + 1, col - 1]);
                this.grid[row, col].attachBall(this.grid[row + 1, col]);
            }
            else if (row % 2 == 0 && col != 0)
            {
           //     Console.WriteLine("row " + row + " col " + col);
                this.grid[row, col].attachBall(this.grid[row, col + 1]);
                this.grid[row, col].attachBall(this.grid[row, col - 1]);
                this.grid[row, col].attachBall(this.grid[row - 1, col - 1]);
                this.grid[row, col].attachBall(this.grid[row - 1, col]);
                this.grid[row, col].attachBall(this.grid[row + 1, col - 1]);
                this.grid[row, col].attachBall(this.grid[row + 1, col]);
            }
            else if (row % 2 == 1 && col != 0)
            {
                this.grid[row, col].attachBall(this.grid[row, col + 1]);
                this.grid[row, col].attachBall(this.grid[row, col - 1]);
                this.grid[row, col].attachBall(this.grid[row - 1, col]);
                this.grid[row, col].attachBall(this.grid[row - 1, col + 1]);
                this.grid[row, col].attachBall(this.grid[row + 1, col]);
                this.grid[row, col].attachBall(this.grid[row + 1, col + 1]);
            }
            
        }

        //public IList<Ball> getAdiacentCellBallAtNorth(Point cell) {

        //    IList<Ball> balls = new List<Ball>();

        //    //upper is shifted row
        //    if (cell.Y % 2 == 0)
        //    {

        //    }
        //    else { 
            
        //    }
        //}
        //return true if the given Ball b intersect any other ball if moved in position pos
        public bool intersectBallAt(Ball b, Vector2 pos) {

            Point cellNextPos = this.convertCoordToCell(pos);
            Point cellCurrPos = this.convertCoordToCell(b.position);

            //it's still in the same cell..
            if (cellNextPos.X == cellCurrPos.X && cellNextPos.Y == cellCurrPos.Y) {
                return false;
            }
            //not a boundary cell
            if (cellNextPos.X > 0 && cellNextPos.Y > 0)
            {

                //this.grid[cellNextPos.Y - 1, cellNextPos.X]
                
            }
            return false;
        }
        //pos specify the actual position of the ball
        public void insertBallAt(Ball b, Vector2 pos) {
            
           // Console.WriteLine("Grid: insertBallAt " + cell);
         //   cell = pos;

            if (isFreeAt(pos)) {
        //        Console.WriteLine("Grid: cell is free");
                Point cell = this.convertCoordToCell(pos);
            
                if (cell.Y % 2 == 0)
                {
                    // Console.WriteLine("ROW " );
                    b.size = new Vector2(ballSize.X , ballSize.Y);
                    b.position = new Vector2(ballSize.X * cell.X, ballSize.Y * cell.Y);
                    b.velocity = new Vector2(0, 0);
                    b.acceleration = new Vector2(0, 0);
                    this.grid[(int)cell.Y, (int)cell.X] = b;
                    //this.grid[(int)cell.Y, (int)cell.X] = new Ball(new Vector2(ballSize.X, ballSize.Y), new Vector2(ballSize.X * cell.X, ballSize.Y * cell.Y),
                    //    new Vector2(0,0), new Vector2(0,0), b.color);
                    
                }
                else
                {

                    b.size = new Vector2(ballSize.X, ballSize.Y);
                    b.position = new Vector2(ballSize.X * cell.X + ballSize.X / 2, ballSize.Y * cell.Y);
                    b.velocity = new Vector2(0, 0);
                    b.acceleration = new Vector2(0, 0);
                    this.grid[(int)cell.Y, (int)cell.X] = b;
                  //  this.grid[(int)cell.Y, (int)cell.X] = new Ball(new Vector2(ballSize.X, ballSize.Y), new Vector2(ballSize.X * cell.X + ballSize.X / 2,
                      //  ballSize.Y * cell.Y),
                    //    new Vector2(0, 0), new Vector2(0, 0), b.color);
                    
                }
                this.grid[(int)cell.Y, (int)cell.X].gridPosition = new Point(cell.Y, cell.X);
                this.attachNewBallToNeighbours(cell.Y, cell.X);
          //      this.visitBall(this.grid[(int)cell.Y, (int)cell.X], new List<Ball>(), this.grid[(int)cell.Y, (int)cell.X].color);

            }


        }
        public bool isFreeAt(Vector2 pos) {
            Point cell = this.convertCoordToCell(pos);
            if (cell.X != -1 && cell.Y != -1 && this.grid[(int)cell.Y, (int)cell.X] != null)
                return false;
            return true;
        }

        public BallGrid(int width, int height, int row, int col) {

            g = new List<IList<Ball>>();
            ballSize = new Vector2(this.gridDimension.X / COL, this.gridDimension .Y/ COL);
            int initialRowNum = 5;
            Random rand = new Random();
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL ; j++)
                {
                    if (i >= initialRowNum)
                        break;
                    int nextColorIndex = rand.Next(0, availableColors.Count);
                    if (i % 2 == 0)
                    {
                        // Console.WriteLine("ROW " );
                        this.grid[i, j] = new Ball(new Vector2(ballSize.X, ballSize.Y), new Vector2(ballSize.X * j, ballSize.Y * i), Color.Red);
                        this.grid[i, j].gridPosition = new Point(i, j);
                    }
                    else if (j < COL - 5)
                    {
                        // Console.WriteLine("col j : " + j);
                        this.grid[i, j] = new Ball(new Vector2(ballSize.X, ballSize.Y), new Vector2(ballSize.X * j + ballSize.X / 2, ballSize.Y * i),
                            availableColors[nextColorIndex]);
                        this.grid[i, j].gridPosition = new Point(i, j);
                    }
                    else {
                        continue;
                    }
                }
            }
        
        }
        public BallGrid() {
            this.grid = new Ball[ROW, COL];
         //   ballSize = new Vector2(800 / COL, 800 / COL);
            ballSize = new Vector2(this.gridDimension.X / COL, this.gridDimension.Y / COL);
            int initialRowNum = 5;

            Random rand = new Random();
          //  Console.WriteLine)
            for (int i = 0; i < ROW; i++) {
                for (int j = 0; j < COL - 1; j++) {
                    if (i >= initialRowNum)
                        break;
                    int nextColorIndex = rand.Next(0, availableColors.Count);
                    if (i % 2 == 0) {
                       // Console.WriteLine("ROW " );
                        this.grid[i, j] = new Ball(new Vector2(ballSize.X, ballSize.Y), new Vector2(ballSize.X * j, ballSize.Y * i), availableColors[nextColorIndex]);
                        this.grid[i, j].gridPosition = new Point(i, j);
                    }
                    else if (j < COL - 2)
                    {
                        // Console.WriteLine("wowowowo");
                        this.grid[i, j] = new Ball(new Vector2(ballSize.X, ballSize.Y), new Vector2(ballSize.X * j + ballSize.X / 2, ballSize.Y * i),
                            availableColors[nextColorIndex]);
                        this.grid[i, j].gridPosition = new Point(i, j);
                    }
                    else {
                        continue;
                    }
                    this.attachNewBallToNeighbours(i, j);
                    
                   // Console.WriteLine( "Ball i " + i+ " j " + j + " pos " + grid[i, j].position + " size " + grid[i, j].size);
                }
            }
        }
    }
  [Serializable]
  class Ball
  {
    static public Vector2 G = new Vector2(0.0f, 9.81f) * 100.0f;
    public static int totalBallNumber = 0;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 acceleration;
    public Vector2 size;
    public Color color;
    public int id;
    public bool visited;
    public bool toBeRemoved;
    public Point gridPosition;
    public IList<Ball> neighbours;
    private void init() {

        this.id = totalBallNumber++;
        this.neighbours = new List<Ball>();
        this.visited = false;
        this.toBeRemoved = false;
    }
    public Ball() {
        this.init();
        this.acceleration = G;
    }
    public Ball(Vector2 size, Vector2 pos, Color col) {
        this.init();
        this.color = col;
        this.size = size;
        this.position = pos;
        this.velocity = new Vector2(0.0f, 0.0f);
        this.acceleration = G;
      //  Console.WriteLine("ball acc " + this.acceleration);
    }
    public Ball(Vector2 size, Vector2 pos, Vector2 velocity, Vector2 acc, Color c) {
        this.init();
        this.position = pos;
        this.size = size;
        this.velocity = velocity;
        this.acceleration = acc;
        this.color = c;
    }
    public void attachBall(Ball b) {
        if (b == null)
            return;
        this.neighbours.Add(b);
        b.neighbours.Add(this);
    }
    public void detachBall(Ball b) {
        this.neighbours.Remove(b);
        b.neighbours.Remove(this);
    }
    public void detachFromAllBalls() {
        while (this.neighbours.Count > 0) {
            this.detachBall(neighbours[0]);
        }
    }
    public Ball Derivative()
    {
      //  Console.WriteLine("ball derivative acc " + this.acceleration + " G " + G);
        return new Ball(this.size, this.velocity, this.acceleration, this.acceleration, this.color);
        //return new Ball()
        //{

        //    position = this.velocity,
        //    //  acceleration = this.acceleration,
        //    velocity = G
        //};
    }

    public static Ball operator +(Ball value1, Ball value2)
    {
      return new Ball()
      {
        position = value1.position + value2.position,
        velocity = value1.velocity + value2.velocity
      };
    }

    public static Ball operator *(float k, Ball value)
    {
      return new Ball()
      {
        position = value.position * k,
        velocity = value.velocity * k
      };
    }
      

    
  }

  class RenderingData
  {
    public Texture2D ball;
    
    public float MaxX;
    public float MaxY;
  }
#endif
}
