//namespace Vec

module Vec 

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

    [<Measure>]
    type rad

    let lerp (x:float32<'u>, y:float32<'u>, w:float32) =
        x*w+y*(1.0f-w)
    type Vector3<[<Measure>] 'a> =
        {
        X : float32<'a>
        Y : float32<'a>
        Z : float32<'a>
        }

        static member Zero : Vector3<'a> =
            {   X = 0.0f<_>;Y = 0.0f<_>;Z = 0.0f<_>; }

        static member (+)
            (v1:Vector3<'a>,v2:Vector3<'a>):Vector3<'a> =
            { X = v1.X+v2.X; Y = v1.Y+v2.Y ; Z = v1.Z+v2.Z }
        static member (+)
            (v:Vector3<'a>,k:float32<'a>):Vector3<'a> =
            { X = v.X+k; Y = v.Y+k ; Z = v.Z + k}
        static member (+)
            (k:float32<'a>,v:Vector3<'a>):Vector3<'a> = v+k
        static member ( ~- ) (v:Vector3<'a>):Vector3<'a> =
            { X = -v.X; Y = -v.Y ; Z = -v.Z}
        static member (-)
            (v1:Vector3<'a>,v2:Vector3<'a>):Vector3<'a> =
                v1+(-v2)
        static member (-)
            (v:Vector3<'a>,k:float32<'a>):Vector3<'a> = v+(-k)
        static member (-)
            (k:float32<'a>,v:Vector3<'a>):Vector3<'a> = k+(-v)
        static member (*)
            (v1:Vector3<'a>,v2:Vector3<'b>):Vector3<'a*'b> =
                { X = v1.X*v2.X; Y = v1.Y*v2.Y ; Z = v1.Z * v2.Z}
        static member (*)
            (v:Vector3<'a>,f:float32<'b>):Vector3<'a*'b> =
            { X = v.X*f; Y = v.Y*f ; Z = v.Z * f}
        static member (*)
            (f:float32<'b>,v:Vector3<'a>):Vector3<'b*'a> =
                v * f
        static member (/)
            (v:Vector3<'a>,f:float32<'b>):Vector3<'a/'b> =
                v*(1.0f/f)
        member this.Length : float32<'a> =
            sqrt((this.X*this.X+this.Y*this.Y+this.Z * this.Z))
        static member Distance(v1:Vector3<'a>,v2:Vector3<'a>) =
            (v1-v2).Length
        static member Lerp (v1:Vector3<'a>,v2:Vector3<'a>, w:float32) =
            { X = lerp (v1.X, v2.X, w); Y = lerp (v1.Y, v2.Y, w); Z = lerp (v1.Z, v2.Z, w);}
        //static member Normalize(v:Vector3<'a>):Vector3<1> =
          //  v/10.0f //v.Length

        
        member this.toXNAVector =
            Microsoft.Xna.Framework.Vector3(this.X |> float32, this.Y |> float32, this.Z |> float32)


    let vector3(x,y,z) : Vector3<m> = { X = y; Y = y; Z = z }
