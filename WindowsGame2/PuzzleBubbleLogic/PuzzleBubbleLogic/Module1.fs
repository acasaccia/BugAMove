// Ulteriori informazioni su F# all'indirizzo http://fsharp.net

namespace Puzzle

module Module1 = 
    open System
    open System.Collections.Generic
    open Microsoft.Xna.Framework
    open Microsoft.Xna.Framework.Graphics
   // open Vec    
  //  let v = Quaternion.Identity

    
    

    [<Measure>]
    type m

    [<Measure>]
    type kg

    [<Measure>]
    type s

    [<Measure>]
    type v = m/s

    [<Measure>]
    type a = m/s^2

//type Game2D() as this =
//    inherit Game()

//vector stuff
    type Vector3<[<Measure>] 'a> =
        {
        X : float<'a>
        Y : float<'a>
        Z : float<'a>
        }


        static member Zero : Vector3<'a> =
            {   X = 0.0<_>;Y = 0.0<_>;Z = 0.0<_>; }

        static member (+)
            (v1:Vector3<'a>,v2:Vector3<'a>):Vector3<'a> =
            { X = v1.X+v2.X; Y = v1.Y+v2.Y ; Z = v1.Z+v2.Z }
        static member (+)
            (v:Vector3<'a>,k:float<'a>):Vector3<'a> =
            { X = v.X+k; Y = v.Y+k ; Z = v.Z + k}
        static member (+)
            (k:float<'a>,v:Vector3<'a>):Vector3<'a> = v+k
        static member ( ~- ) (v:Vector3<'a>):Vector3<'a> =
            { X = -v.X; Y = -v.Y ; Z = -v.Z}
        static member (-)
            (v1:Vector3<'a>,v2:Vector3<'a>):Vector3<'a> =
                v1+(-v2)
        static member (-)
            (v:Vector3<'a>,k:float<'a>):Vector3<'a> = v+(-k)
        static member (-)
            (k:float<'a>,v:Vector3<'a>):Vector3<'a> = k+(-v)
        static member (*)
            (v1:Vector3<'a>,v2:Vector3<'b>):Vector3<'a*'b> =
                { X = v1.X*v2.X; Y = v1.Y*v2.Y ; Z = v1.Z * v2.Z}
        static member (*)
            (v:Vector3<'a>,f:float<'b>):Vector3<'a*'b> =
            { X = v.X*f; Y = v.Y*f ; Z = v.Z * f}
        static member (*)
            (f:float<'b>,v:Vector3<'a>):Vector3<'b*'a> =
                v * f
        static member (/)
            (v:Vector3<'a>,f:float<'b>):Vector3<'a/'b> =
                v*(1.0/f)
        member this.Length : float<'a> =
            sqrt((this.X*this.X+this.Y*this.Y+this.Z * this.Z))
        static member Distance(v1:Vector3<'a>,v2:Vector3<'a>) =
            (v1-v2).Length
        static member Normalize(v:Vector3<'a>):Vector3<1> =
            v/v.Length


    type GameEntity = 
        {
            Scale : Vector3<m>
            Position : Vector3<m>
            Velocity : Vector3<m/s>
            Acceleration : Vector3<m/s^2>
        }

    type Grid = 
        {
            entity : GameEntity
    
        }
    type Ball = 
        {
            Position : Vector3<m>
            Radius : float<m>
            Coloro : Color // xna color type
             
        }
    type ClimbingBall =
        {
            Ball : Ball
            Velocity : Vector3<m/s>

    
        }
    type Arrow = 
        {
            Position : Vector3<m>
            Angle : float
        }
    let moveEntity (e:GameEntity, dt:float<s>) = 
        {
            e with Position = e.Position + (e.Velocity * dt)
        }
    let BallColors = [ Color.Black ; Color.White ; Color.Green ; Color.Red ; Color.Blue]

//    let moveClimbingBall (l : ClimbingBall list, dt : float<s>) = 
//        {
//            [
//                for b in l do
//                    let Position = b.Ball.Position += b.Velocity * dt
//                    yield ClimbingBall(b with ClimbingBall.B )
//            ]
//        }
    type PuzzleBobble = 
        {
            name : string
            dimension : Vector3<m>
            dockedBalls : Ball list
            climbingBalls : ClimbingBall list
            fallingBalls : Ball list
            loadedBall : Ball

            arrow : Arrow
        }
    let simulation_step(l : PuzzleBobble) = 
        l.name = "ciao"

//let v = { X= 0.0; Y = 0.0 ; Z = 0.0}
//let anInt = 5
//let aString = "Hello"
//// Perform a simple calculation and bind anIntSquared to the result.
//let anIntSquared = anInt * anInt
//
//let f(x) = x + 10
//System.Console.WriteLine(f(3))
//System.Console.WriteLine(aString)
//System.Console.WriteLine(anIntSquared)


