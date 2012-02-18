module PuzzleBobble
open Casanova
open Coroutines
open Vec
open System 
open Microsoft.FSharp.Math
open Microsoft.CSharp
//open WindowsGame2
open PuzzleBobbleInputHandling


// ----------------------- SOME COSTANTS ------------------------------------k
let ball_scale : Vector3<m> = {X = 1.0f<m> ; Y = 1.0f<m> ; Z = 1.0f<m>}
let ball_default_vel : Vector3<m/s> = Vector3.Zero
let ball_default_pos : Vector3<m> = Vector3.Zero
let ball_default_acc : Vector3<m/s^2> = Vector3.Zero

let FallingBallDefaultWeight = 1.0f<kg>
let BallRadius : float32<m> = 0.2f<m>
let BallDiameter : float32<m> = BallRadius * 2.0f
let ClimbingBallDefaultVelocity : float32<m/s> = 4.0f<m/s>
let G: float32<m/s^2>= 9.81f<m/s^2>
let BallsPerLine  =  9
let LineNumber : int = 2
let MaxLineNumber : int = 13; 
let x  = MaxLineNumber - 1; 
let GridStepDelta : Vector3<m> = {X = 0.0f<m>; Y = BallDiameter; Z= 0.0f<m>}
let BallClusterSize = 3

let BoxDimension : Vector3<m>= { X=  float32 BallsPerLine * BallDiameter; Y= float32 MaxLineNumber * BallDiameter; Z = 3.0f<m>}
let BoxTopMostLeftFrontCorner : Vector3<m> = {X = 0.0f<m>  ; Y = BoxDimension.Y; Z = 0.0f<m>}
let FloorMiddlePoint : Vector3<m> = { X =  BoxDimension.X / 2.0f  ; Y = 0.0f<m>; Z = 0.0f<m>}
let BallMaxDistance = 10.0f<m>
let BallCenterMinY = 2.0f * BallDiameter
let DefaultRoofStepTimeDelta : float32<s>= 20.0f<s>
let ArrowAngleInit () = Microsoft.Xna.Framework.MathHelper.PiOver2

let arrow_rotation_delta = 0.02f
let ArrowMaxAngle: float32 = float32 Math.PI / 2.0f - (0.1f)

let PointsPerBall = 10
// ---------------------------- COLORS ---------------------
let AvailableColors = 
    [
    Microsoft.Xna.Framework.Color.Black;
    Microsoft.Xna.Framework.Color.Red;
    Microsoft.Xna.Framework.Color.Green;
    Microsoft.Xna.Framework.Color.Yellow;
    Microsoft.Xna.Framework.Color.Blue
    ]
//Color wrapper..I need it for using sets of colors
type Color (color: Microsoft.Xna.Framework.Color) =  
    member this.color = color
    override this.Equals(o) =
        match o with
        | :? Color as col -> this.color.PackedValue = col.color.PackedValue
        | _ -> false
    override this.GetHashCode() =
        this.color.GetHashCode()
    interface IComparable with
        member this.CompareTo(o) = 
            match o with
            | :? Color as col -> compare this.color.PackedValue col.color.PackedValue
            | _ -> -1
                        
let GetColorN (x:int) = 
    AvailableColors.Item(x)
type ColorCounter = 
    {     
        Count : int
        Color : Microsoft.Xna.Framework.Color         
    }
    
// -------------------- GAME STATUS ----------------------------------------
type GameStatus = 
    | Playing = 0
    | Win = 1
    | Lost = 2
    | Ready = 3 

//TODO: use GameEvents to share data with renderer
type GameEvents = 
    | BallExplosion 
    | BallDocked

type ScoreEntry = 
    {
        PlayerName : string
        PlayerScore : int
    }

let CompareScores score1 score2  = 
   if score1.PlayerScore < score2.PlayerScore then
        -1
   elif score1.PlayerScore > score2.PlayerScore then
        1
   else
        0 
type LevelStatus = 
    {
        ElapsedTime : Variable<float32<s>>
        Score : Variable<int>
        AvailableColors : Variable<Map<uint32,ColorCounter>>
        TopScores : Variable<List<ScoreEntry>>
        Status : Variable<GameStatus>
    }
type PlayerStatus = 
    {
        number : int
        Name : string
        Points : int
    }

// ----------------------- GAME ENTITIES -----------------------------------------
type GameEntity = 
    {
        Scale : Vector3<m>
        Position : Vector3<m>
        Velocity : Vector3<m/s>
        Acceleration : Vector3<m/s^2>
    }
type Ball =
    {   
       center : Vector3<m> 
       radius : float32<m>
       color : Microsoft.Xna.Framework.Color 
       visited  : bool 
    }
let diameter (b : Ball) : float32<m>=
    b.radius *  2.0f
    
type ClimbingBall =
    {
        center : Vector3<m>
        radius : float32<m>
        velocity : Vector3<m/s>
        color : Microsoft.Xna.Framework.Color 
    }

type FallingBall = 
    {
        center : Vector3<m>
        radius : float32<m>
        velocity : Vector3<m/s>
        color : Microsoft.Xna.Framework.Color 
        mass : float32<kg>
        acceleration : Vector3<m/s^2>
    }
type Arrow = 
    {
        center : Vector3<m>
        angle : Variable<float32>
    }
//let AuxGrid :  mutable bool [,] = 
//     Array2D.init<bool> MaxLineNumber BallsPerLine (fun row col-> ())
//let GridVisitAuxArray : m [,]
type GameState =
    {
        LevelStatus : LevelStatus
        //Grid : Variable<Option<Ball> [,]>
        Grid : Variable<Option<Ball> [,]>
        GridSteps : Variable<int>
        ReadyBall : Variable<Ball>
        ClimbingBalls : Variable<List<ClimbingBall>>
        Arrow : Arrow
        RoofStepTimeDelta : float32<s>
        FallingBalls : Variable<List<FallingBall>>
        SoundOn : Variable<bool>
    }







//returns true if distance between point1 and point2 is higher than MaxDistance
let isDistanceHigher (point1 : Vector3<m> , point2: Vector3<m>, maxDistance: Vector3<m>): bool = 
    let differenceVector = point2 - point1
    if (differenceVector.X < maxDistance.X && differenceVector.Y < maxDistance.Y && differenceVector.Z < maxDistance.Z) then
        false
    else
        true
let AvailableColorList(colorMap: Map<uint32, ColorCounter>) : List<Microsoft.Xna.Framework.Color> = 
    [ for KeyValue(_, col) in colorMap -> col.Color ]
let random_generator = System.Random(5)

let RandomBallFromRemainingColors (colorMap: Map<uint32, ColorCounter>) : Ball = 
    let colorList = AvailableColorList(colorMap)
    let i = random_generator.Next(0,colorList.Length)
    {
        center = { FloorMiddlePoint with Y = BallRadius }
        radius = BallRadius 
        color = colorList.Item(i) //AvailableColors.Item (i)// Microsoft.Xna.Framework.Color.White    
        visited = false //= variable (false)
    }
let new_random_ball (colorMap: Map<uint32, ColorCounter>) : Ball =
    let colorList = AvailableColorList(colorMap)
    let i = random_generator.Next(0,colorList.Length)//AvailableColors.Length )
 //   Console.WriteLine("new ball color index " + i.ToString())
    {
        center = { FloorMiddlePoint with Y = BallRadius }
        radius = BallRadius 
        color = AvailableColors.Item (i)//colorList.Item(i) //AvailableColors.Item (i)// Microsoft.Xna.Framework.Color.White    
        visited = false //= variable (false)
    }
    
let UpdateScore(ballNumber : int , game_state: GameState) =
    game_state.LevelStatus.Score := !game_state.LevelStatus.Score + ballNumber * PointsPerBall
let InitLevelStatus() : LevelStatus =
    {
     ElapsedTime = Variable(fun () -> 0.0f<s>)
     Score = variable(0)
     AvailableColors =  Variable(fun () -> Map.empty)
     TopScores = Variable(fun () -> List.empty) //[ {PlayerName = "marco" ; PlayerScore = 10} ; {PlayerName = "foobar" ; PlayerScore = 100}])
     Status = variable(GameStatus.Ready)
    }

type SomeClass(id : int) =    
    member this.ID = id
    override this.Equals(o) =
        match o with
        | :? SomeClass as sc -> this.ID = sc.ID
        | _ -> false
    override this.GetHashCode() =
        id.GetHashCode()
    interface System.IComparable with
        member this.CompareTo(o) =
            match o with
            | :? SomeClass as sc -> compare this.ID sc.ID
            | _ -> -1


    

//0,0 is at the topleft corner
let GridToCoordinates (grid_step: int , row:int, col:int): Vector3<m> =
    if ((row % 2 ) = 0) then
        { 
        X = BallRadius + BallDiameter * float32 ( col );
        Y = BoxDimension.Y - BallRadius - (float32 (row  ) * BallDiameter) - (float32 grid_step * GridStepDelta.Y);
        Z = 0.0f<m>  
        }
    else
        { 
        X = BallDiameter + BallDiameter * float32 ( col );
        Y = BoxDimension.Y - BallRadius - float32 (row  ) * BallDiameter - (float32 grid_step * GridStepDelta.Y) ;
        Z = 0.0f<m>  
        } 
let CoordinateToGrid (grid_step : int , center : Vector3<m>) : int * int = 
    let row : int = MaxLineNumber - 1 -  int ((center.Y  ) / BallDiameter) -  grid_step //* GridStepDelta.Y
    
    let col : int = 
        if (row % 2 = 0) then
            //int ((center.X - BallRadius) / BallDiameter)
            int (center.X / BallDiameter)
        else 
            int ((center.X - BallRadius) / BallDiameter)            
    row, col



let filled_grid_with_random_ball (): Option<Ball> [,] =
    let random = System.Random()
    Array2D.init<Option<Ball>> MaxLineNumber BallsPerLine (fun row col->
        
       // Console.WriteLine("row " + row.ToString() + " col " + col.ToString())
        if (row % 2 = 0 && row < LineNumber) then
            
             Some(  {
                    center = GridToCoordinates (0, row ,col);
                    radius = BallRadius; 
                    color = AvailableColors.Item (random.Next(0,AvailableColors.Length ))
                    visited = false
                    }
            )
        else if (col < BallsPerLine - 1 && row < LineNumber) then
            Some(   {
                    center = GridToCoordinates (0, row ,col);
                    radius = BallRadius; 
                    color =   AvailableColors.Item (random.Next(0,AvailableColors.Length ))
                    visited = false 
                    }
                )
        else
            None
        )
    
let create_strip_grid (height : int): Variable<Option<Ball> [,]> =
    Variable ( fun () -> Array2D.init<Option<Ball>> MaxLineNumber BallsPerLine (fun row col->  
        if (row % 2 = 0 && row < height) then
      
             Some(  {
                    center = GridToCoordinates (0, row ,col);
                    radius = BallRadius; 
                    color = if (col = BallsPerLine / 2) then 
                                AvailableColors.Item (3)
                            else
                                AvailableColors.Item (0)
                    visited = false
                    }
            )
        else if (col < BallsPerLine - 1 && row < height) then
            Some(   {
                    center = GridToCoordinates (0, row ,col);
                    radius = BallRadius; 
                    color =   if (col = BallsPerLine / 2) then 
                                    AvailableColors.Item (3)
                              else
                                AvailableColors.Item (1)
                    visited = false 
                    }
                )
        else
            None
        )
    )
let CreateEmptyGrid = Variable ( fun () -> Array2D.init<Option<Ball>> MaxLineNumber BallsPerLine (fun x y -> None) )


let EqualsColor (c1 : Microsoft.Xna.Framework.Color, c2:Microsoft.Xna.Framework.Color ) : bool =
    if (c1.R = c2.R && c1.G = c2.G && c1.B = c2.B && c1.A = c2.A) then
        true
    else
        //Console.WriteLine("different colors")
        false

let AvailableColorMap (grid : Option<Ball> [,]) : Map<uint32, ColorCounter> =
    let  map=  ref<Map<uint32, ColorCounter>> Map.empty
    
    grid |> Array2D.iteri(fun row col ball -> match ball with 
                                                    |Some(ball) -> 
                                                        let color : Option<ColorCounter> = map.Value.TryFind(ball.color.PackedValue)
                                                        match color with 
                                                        |Some c -> 
                                                            map.contents <- map.Value.Remove(ball.color.PackedValue)
                                                            map.contents <- map.Value.Add(ball.color.PackedValue, {c with Count = c.Count + 1})
                                                        |None -> map.contents <- map.Value.Add(ball.color.PackedValue, {Count = 1; Color = ball.color})
                                                    |None -> ()
                            )
    
    map.Value

let PrintVisitedBallsIndex (grid : Option<Ball> [,]) =
    grid |> Array2D.iteri(fun row col ball -> match ball with 
                                                |Some(ball) -> 
                                                if ball.visited then Console.WriteLine("ball visited " + row.ToString() + " " +  col.ToString() )
                                                |None -> ())
let GridBallsSetUnvisited (grid : Option<Ball> [,]) =
    grid |> Array2D.iteri(fun row col ball -> match ball with 
                                                |Some(ball) -> 
                                                grid.[row,col] <- Some({ball with visited = false})
                                                |None -> ())

let DeleteUnvisitedGridBalls(grid : Option<Ball> [,]) : List<FallingBall> = 
    let l : ref<List<FallingBall>> = ref<List<FallingBall>> List.empty
    grid |> Array2D.iteri(fun row col ball -> match ball with 
                                                    |Some(ball) -> 
                                                        if (ball.visited = false) then
                                                            grid.[row,col] <- None

                                                            l.contents <- {   center = ball.center; 
                                                                radius = ball.radius ;
                                                                velocity = Vector3.Zero;  
                                                                color = ball.color; 
                                                                mass = FallingBallDefaultWeight;
                                                                acceleration = {X = 0.0f<m/s^2> ; Y =  -G ; Z = 0.0f<m/s^2>}
                                                            } :: l.contents 
                                                    |None -> ())
    l.Value
    

    
let ContainsIndex (r:int, c:int, list:List<int * int>) : bool = List.exists(fun elem ->
                                                                                    let row, col = elem
                                                                               //     Console.WriteLine("contains index " + row.ToString() + " " + col.ToString() + " " + (row = r && col = c).ToString())
                                                                                    (row = r && col = c) ) list

let VisitGridExcluding (grid : Option<Ball> [,], startingRow:int, startingCol:int ,exclusionList: List<int*int>) =
    let rec loop (row, col) =
        if ( row < 0 || col < 0 || row > MaxLineNumber - 1 || col > BallsPerLine - 1) then
            ()
        else
            let ball = grid.[row,col]
            match ball with 
                |Some(ball) ->
                    if (  ball.visited  ) then 
                        ()
                    elif ( ContainsIndex (row,col, exclusionList)) then
                        ()
                    else
                        grid.[row,col] <- Some({ball with visited = true})//ball.visited <- true
                        if (row % 2 = 0) then
                            loop(row  + 1, col )
                            loop(row  - 1, col )
                            loop(row , col + 1)
                            loop(row , col  - 1)
                            loop(row  + 1, col - 1)
                            loop(row  - 1, col  - 1)
                        else 
                            loop(row  + 1, col )
                            loop(row  - 1, col )
                            loop(row , col + 1)
                            loop(row , col  - 1)
                            loop(row  + 1, col + 1)
                            loop(row  - 1, col  + 1)
                |None  ->
                    () 
    loop(startingRow, startingCol)
     

let GetSameColorNeighs(grid : Option<Ball> [,], row : int , col : int, color : Microsoft.Xna.Framework.Color) : List<int * int> =
    let rec loop (row, col) = seq{
                   
        if (row < 0 || col < 0 || row > MaxLineNumber - 1 || col > BallsPerLine - 1) then
            ()
        else
            let ball = grid.[row,col]
            match ball with 
                |Some(ball) -> 
                    if ((ball.visited) = true || not <| EqualsColor (ball.color, color)) then
                        grid.[row,col] <-Some({ball with visited = true})
                        ()
                    else
                        grid.[row,col] <-Some({ball with visited = true})
                        yield row,col
                        if (row % 2 = 0) then
                            yield! loop(row  + 1, col )
                            yield! loop(row  - 1, col )
                            yield! loop(row , col + 1)
                            yield! loop(row , col  - 1)
                            yield! loop(row  + 1, col - 1)
                            yield! loop(row  - 1, col  - 1)
                        else 
                            yield! loop(row  + 1, col )
                            yield! loop(row  - 1, col )
                            yield! loop(row , col + 1)
                            yield! loop(row , col  - 1)
                            yield! loop(row  + 1, col + 1)
                            yield! loop(row  - 1, col  + 1)
                |None  -> () }
    loop(row,col) |> Seq.toList
    
let AvailableColorLis(map : Map<uint32,ColorCounter>) = //: List<Microsoft.Xna.Framework.Color>  = 
     map |> Seq.map (fun _ col -> col.Color) |> Seq.toList

let PlaySound(game_state: GameState, e:Sound.PuzzleBobbleSoundManager.SoundsEvent) = 
    if (!game_state.SoundOn) then
        do Sound.PuzzleBobbleSoundManager.playSound(e);
//dock a ball inside the grid, depending on its position
let DockBall (game_state: GameState, grid: Option<Ball> [,] , ball : Ball) : List<FallingBall> = 
   
    do PlaySound(game_state, Sound.PuzzleBobbleSoundManager.SoundsEvent.BALL_DOCKED)
    let deletedList : ref<List<FallingBall>> = ref<List<FallingBall>> List.empty
    
    let row , col = CoordinateToGrid(!game_state.GridSteps, ball.center)
 //   Console.WriteLine("DockBall at " + row.ToString() + " " + col.ToString())
    let row =   if (row < 0 ) then 
                    0
                else
                    row
    let center = GridToCoordinates(!game_state.GridSteps, row ,col)
    do (grid).[row,col] <-  Some({ball with center = center})
    if center.Y <= BallCenterMinY then
        game_state.LevelStatus.Status:= GameStatus.Lost
    else
        do 
            let l :List<int * int> = GetSameColorNeighs(grid, row, col , ball.color)
            GridBallsSetUnvisited(!game_state.Grid)
            if l.Length >= BallClusterSize then
                for col in 0 .. BallsPerLine - 1 do          
                    VisitGridExcluding (grid, 0, col , l) 
                deletedList.contents <- List.concat [ DeleteUnvisitedGridBalls(grid)  ; !game_state.FallingBalls]
                GridBallsSetUnvisited(!game_state.Grid)
                Sound.PuzzleBobbleSoundManager.playSound(Sound.PuzzleBobbleSoundManager.SoundsEvent.BALL_EXPLOSION)
            
            if l.Length >= BallClusterSize then
                UpdateScore(l.Length, game_state)
            else
                UpdateScore(0, game_state)
            
    deletedList.contents 

let ApplyVelocityToVector (p:Vector3<m> , v: Vector3<m/s> ,dt : float32<s> ) : Vector3<m> =
     p + (v * dt)
let InvertVelocityX (v:Vector3<m/s>) : Vector3<m/s> =
    {v with X = -v.X}

let MoveBall(b: ClimbingBall, dt: float32<s> ) : ClimbingBall  =
     
    let newBall = {b with center = b.center + b.velocity * dt}
    if (newBall.center.X + newBall.radius > BoxDimension.X || newBall.center.X - newBall.radius < 0.0f<m>) then
        {newBall with velocity = InvertVelocityX (b.velocity)}
    else 
        newBall

let MoveFallingBall(b:FallingBall, dt: float32<s>) = 
    let newBall = {b with center = b.center + b.velocity * dt; velocity = b.velocity + b.acceleration * dt }
//    Console.WriteLine("newBall ")
    newBall
  //  game_state.ClimbingBalls := !game_state.ClimbingBalls
            



//let ArrowMinAngle : float32 = - ArrowMaxAngle
let update_arrow(a: Arrow ) =
    let input  = InputManager.getState();
    !a.angle +
        if  input.ArrowMovedLeft = true && !a.angle < (ArrowAngleInit() +  ArrowMaxAngle) then
//Sound.PuzzleBobbleSoundManager.playSound(Sound.PuzzleBobbleSoundManager.SoundsEvent.ARROW_MOVED)
            arrow_rotation_delta   
        elif  input.ArrowMovedRight = true && !a.angle > (ArrowAngleInit() - ArrowMaxAngle)  then
  //          Sound.PuzzleBobbleSoundManager.playSound(Sound.PuzzleBobbleSoundManager.SoundsEvent.ARROW_MOVED)
            -arrow_rotation_delta
        else
            0.0f


let MoveDownDockedBalls(list : List<Ball>, delta: float32<m> ) : List<Ball> = 
      
    [
        for docked_ball in list do
        
           yield {docked_ball with center = {X = docked_ball.center.X; Y = docked_ball.center.Y - delta; Z = docked_ball.center.Z}}
    ]


let MoveDownGrid (game_state: GameState) =
    game_state.GridSteps := !game_state.GridSteps + 1 
    
    game_state.Grid := Array2D.init<Option<Ball>> MaxLineNumber BallsPerLine (fun row col->
        match (!game_state.Grid).[row,col] with
            |Some(n) -> 
                let center = (!game_state.Grid).[row,col].Value.center -  GridStepDelta
                if center.Y <= BallCenterMinY then
                    game_state.LevelStatus.Status:= GameStatus.Lost
                Some({(!game_state.Grid).[row,col].Value with center = center})
            |None -> None
       
    )
    
let  BallShoot(readyBall : Ball, angle:float32) : ClimbingBall=
   
     {
    center = readyBall.center;
    radius = readyBall.radius;
    velocity = 
        {      
        X = (angle |> float32 |> cos) * ClimbingBallDefaultVelocity;// ClimbingBallDefaultVelocity.X ;
        Y = (angle |> float32 |> sin )* ClimbingBallDefaultVelocity;//ClimbingBallDefaultVelocity.Y;
        Z = 0.0f<m/s>
        }
    color = readyBall.color
    }

let BallsCollision (b1 : Ball, b2 : ClimbingBall) : bool =
    let distance = Vector3.Distance(b1.center, b2.center)
    if (distance > BallRadius * 2.0f) then 
        false
    else 
        true


let empty_level_state () : GameState =
    {
    LevelStatus = InitLevelStatus()
    Grid = Variable (fun () -> Array2D.init<Option<Ball>> MaxLineNumber BallsPerLine (fun row col-> None))
    GridSteps = Variable (fun ()-> 0)
    ReadyBall = Variable (fun () ->  new_random_ball(Map.empty))
    ClimbingBalls = Variable(fun () -> [])
    FallingBalls = Variable(fun () -> [])
    Arrow =  { center = FloorMiddlePoint ; angle = Variable(ArrowAngleInit)}//angle = Variable(fun () -> Microsoft.Xna.Framework.MathHelper.PiOver2)}//- Microsoft.Xna.Framework.MathHelper.PiOver2)};
    RoofStepTimeDelta = DefaultRoofStepTimeDelta
    SoundOn = variable(true)
    }
let random_level_state () : GameState =
    {
   
    LevelStatus = InitLevelStatus()
    Grid = Variable( fun() -> filled_grid_with_random_ball())
    GridSteps = Variable (fun ()-> 0)
    ReadyBall = Variable (fun () -> {
                                    center = { FloorMiddlePoint with Y = BallRadius }
                                    radius = BallRadius 
                                    color =  AvailableColors.Item (0)
                                    visited = false 
                                    } )
    ClimbingBalls = Variable(fun () -> [])
    FallingBalls = Variable(fun () -> [])
    Arrow =  { center = FloorMiddlePoint ; angle = Variable(ArrowAngleInit)}
    RoofStepTimeDelta = DefaultRoofStepTimeDelta
    SoundOn = variable(true)
    }




//note: the following code is ugly, but it's a workaround. In certain situations game_state value should be changed (game reset, new level, ...).
// I did it before declaring it mutable but I had some trouble in de/serialize a mutable record. So every 
let  game_state :GameState = 
    //random_level_state()//empty_level_state()
    empty_level_state()

let setLevelStatus(s) = 
    game_state.LevelStatus.AvailableColors := !s.AvailableColors 
    game_state.LevelStatus.ElapsedTime := !s.ElapsedTime 
    game_state.LevelStatus.Status := !s.Status
    game_state.LevelStatus.Score := !s.Score 
    game_state.LevelStatus.TopScores := List.concat [!game_state.LevelStatus.TopScores ;!s.TopScores]

let setGameState(newGameState : GameState) = 
    game_state.Arrow.angle := !newGameState.Arrow.angle
    game_state.ClimbingBalls := !newGameState.ClimbingBalls
    game_state.FallingBalls := !newGameState.FallingBalls
    game_state.Grid :=  !newGameState.Grid
    game_state.GridSteps := !newGameState.GridSteps 
    setLevelStatus(newGameState.LevelStatus)
    game_state.ReadyBall := !newGameState.ReadyBall
    game_state.SoundOn := !newGameState.SoundOn
    //Casanova.commit_variable_updates()
let setup_random_level() = 
    setGameState(random_level_state())
let load_game_state (state : GameState) =
//   Console.WriteLine("PuzzleBobble: load_game_state at time " + state.LevelStatus.ElapsedTime.Value.ToString())
   setGameState({state with LevelStatus = {state.LevelStatus with Status =  variable(GameStatus.Ready)}})//<- state
   Casanova.commit_variable_updates()
//   Console.WriteLine("NOW GAME TIME  " + game_state.LevelStatus.ElapsedTime.Value.ToString())
   

let ListOfDockedBalls (b:Ball) : Ball list =
    !game_state.Grid |> Seq.cast<Option<Ball>> |> Seq.fold (fun l n -> match n with 
                                                                       |Some(n) -> n :: l
                                                                       |None -> l ) []                                                                            
let BallHitTheRoof (game_state: GameState, b: ClimbingBall) : bool =
    if (b.center.Y + b.radius > BoxDimension.Y - (BallDiameter * float32  !game_state.GridSteps)) then
        true 
    else 
        false
//refactor using grid
let CheckForCollision (b : ClimbingBall) : bool =
    let  mutable found = false
    let l = !game_state.Grid |> Seq.cast<Option<Ball>> |> Seq.fold (fun l n -> match n with 
                                                                               |Some(n) -> n :: l
                                                                               |None -> l ) []
    for docked_ball in l do        
        if BallsCollision(docked_ball, b) then
            found <- true
    found


let play_against_roof(dt:float32<s>) =
    ()
let play_against_time(dt:float32<s>) =
    ()               

let update_state(dt:float32<s>) =
 //   Console.WriteLine("update_state")
    game_state.LevelStatus.TopScores := !game_state.LevelStatus.TopScores
    match !game_state.LevelStatus.Status with
        | GameStatus.Playing ->
      //      Console.WriteLine("Playing")
            game_state.LevelStatus.ElapsedTime := !game_state.LevelStatus.ElapsedTime + dt
        | GameStatus.Win  ->
            Console.WriteLine("Win")
            game_state.LevelStatus.ElapsedTime := !game_state.LevelStatus.ElapsedTime
        | GameStatus.Lost  ->
            (Console.WriteLine("Lost"))
            game_state.LevelStatus.ElapsedTime := !game_state.LevelStatus.ElapsedTime                
        | GameStatus.Ready  ->
           // Console.WriteLine("Ready")
            ()// (Console.WriteLine("Ready"))
            game_state.LevelStatus.ElapsedTime := !game_state.LevelStatus.ElapsedTime
        | _ -> ()
   


 //   let colMap = AvailableColorMap(!game_state.Grid)
    game_state.LevelStatus.Score := !game_state.LevelStatus.Score
  //  game_state.LevelStatus.AvailableColors := AvailableColorMap(!game_state.Grid)
   // game_state.LevelStatus.AvailableColors := !game_state.LevelStatus.AvailableColors
    //Console.WriteLine("available colors : " + AvailableColorMap(!game_state.Grid).Count.ToString())
    game_state.LevelStatus.Status := //!game_state.LevelStatus.Status
        
        if (!game_state.LevelStatus.Status) = GameStatus.Playing && AvailableColorMap(!game_state.Grid).IsEmpty then
            game_state.LevelStatus.TopScores := {PlayerName = DateTime.Today.ToString(); PlayerScore = !game_state.LevelStatus.Score} :: !game_state.LevelStatus.TopScores//[ List.sortWith CompareScores ({PlayerName = "foobardfsfds"; PlayerScore = 14} :: !game_state.LevelStatus.TopScores) ]
            PlaySound(game_state, Sound.PuzzleBobbleSoundManager.SoundsEvent.WIN);
            GameStatus.Win
        else
            do game_state.LevelStatus.TopScores := !game_state.LevelStatus.TopScores 
             
            (!game_state.LevelStatus.Status) 
    game_state.GridSteps := !game_state.GridSteps
    game_state.ReadyBall := !game_state.ReadyBall
    game_state.Arrow.angle := 
        !game_state.Arrow.angle 
    
    game_state.Grid := !game_state.Grid
    game_state.LevelStatus.AvailableColors := !game_state.LevelStatus.AvailableColors 
    game_state.FallingBalls := [for fb in !game_state.FallingBalls do
                                    let newBallPos = MoveFallingBall(fb, dt) 
                                    if (newBallPos.center.Length < BallMaxDistance) then
                                        yield newBallPos                            
                                ]
    game_state.ClimbingBalls :=
        [
        for b in !game_state.ClimbingBalls do
            let newBallPos = MoveBall (b, dt) 
            if newBallPos.center.Length < BallMaxDistance then
                if ( CheckForCollision(newBallPos) || BallHitTheRoof(game_state, newBallPos))  then
                    let dock_b : Ball = { center =  newBallPos.center; radius = b.radius ; color = b.color; visited = false}//variable(false)}
                    let deletedBall = DockBall(game_state, !game_state.Grid, dock_b)                    
                    game_state.FallingBalls := List.concat [deletedBall ; 
                                                                [for fb in !game_state.FallingBalls do
                                                                
                                                                    yield MoveFallingBall(fb, dt)
                                                                
                                                                ]
                                                            ]
                else
                    yield newBallPos
        ]
    if ((!game_state.LevelStatus.ElapsedTime / (game_state.RoofStepTimeDelta)) >  float32 !game_state.GridSteps + 1.0f ) then
        do MoveDownGrid(game_state)
//
//    //----- GAME OPTIONS -----
    game_state.SoundOn := !game_state.SoundOn
//------------------------------------------ COROUTINES STUFF ----------------------------------------------------------- 
let private (!) = immediate_read
let private (:=) = immediate_write


//this call all coroutines..
let private main () =
   // let state = game_state
    
    let input_handler()  =
        co{
            do! yield_
            let input = InputManager.getState()
            if (!game_state.LevelStatus.Status = GameStatus.Ready && input.Continue ) then
                game_state.LevelStatus.Status := GameStatus.Playing
            if ((input.SoundOff && !game_state.SoundOn) || (input.SoundOn && not <| !game_state.SoundOn)) then
                game_state.SoundOn := not <| !game_state.SoundOn
            return ()
        }
//    let roof_down() =
//        co{
//            do! wait 15.0
//         //   MoveDownGrid(game_state)
//          //  game_state.DockedBalls := MoveDownDockedBalls(!game_state.DockedBalls, 0.4f<m>)
//            return ()
//        }
    let shoot_ball() =
        co{
            do! yield_
            let input = InputManager.getState();
            if (!game_state.LevelStatus.Status =  GameStatus.Playing) then 
                if (input.BallShoot) then
                    
                    if (!game_state.SoundOn) then
                        Console.WriteLine("SoundOn")
                        Sound.PuzzleBobbleSoundManager.playSound(Sound.PuzzleBobbleSoundManager.SoundsEvent.BALL_SHOOT);
                    game_state.ClimbingBalls :=  BallShoot(!game_state.ReadyBall, !game_state.Arrow.angle)  :: !game_state.ClimbingBalls
                    let newb =  RandomBallFromRemainingColors(AvailableColorMap(!game_state.Grid))//new_random_ball (!game_state.LevelStatus.AvailableColors)
                    game_state.ReadyBall := newb
                    do! wait 0.3
            return ()
        }
    let move_arrow() =
        co{
            do! yield_
                     
            let angleDelta = 0.05f 
            if (!game_state.LevelStatus.Status =  GameStatus.Playing) then 
                game_state.Arrow.angle := 
                    update_arrow game_state.Arrow
            return()
        }
   
    let  idle_co() = 
        co{
            do! yield_
            return ()
        }
    (repeat_ (idle_co()) .||> repeat_ (move_arrow()) .||> repeat_ (shoot_ball()) .||> repeat_(input_handler()))

let mutable update_main = 
    main()
let update_script() = update_main <- update_ai update_main
