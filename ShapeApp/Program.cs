using Raylib_cs;
using System;
using System.Numerics;
using ShapeLibrary;
using static Raylib_cs.Raylib;  // Add this line



class Program
{
    const int screenWidth = 800;
    const int screenHeight = screenWidth;
    public static float universeCenter = 2;
    public static readonly float[] scroll_limit = [2.1f, 3.2f];

    public static double xz_angle = 0;
    // double xy_angle = 80;
    public static double yz_angle = 0;

    static void DrawTexturePoly(
    Texture2D texture,
    Vector2 center,
    Vector2[] points,
    Vector2[] texcoords,
    int pointCount,
    Color tint)
    {
        if (pointCount < 3) return;

        Rlgl.SetTexture(texture.Id);

        Rlgl.Begin(DrawMode.Triangles);
        Rlgl.Color4ub(tint.R, tint.G, tint.B, tint.A);

        for (int i = 0; i < pointCount - 1; i++)
        {
            // Center vertex
            Rlgl.TexCoord2f(0.5f, 0.5f);
            Rlgl.Vertex2f(center.X, center.Y);

            // Vertex i
            Rlgl.TexCoord2f(texcoords[i].X, texcoords[i].Y);
            Rlgl.Vertex2f(points[i].X + center.X, points[i].Y + center.Y);

            // Vertex i+1
            Rlgl.TexCoord2f(texcoords[i + 1].X, texcoords[i + 1].Y);
            Rlgl.Vertex2f(points[i + 1].X + center.X, points[i + 1].Y + center.Y);
        }

        Rlgl.End();

        Rlgl.SetTexture(0);
    }
    static Vector2 onScreen(Vector2 point)
    {
        // -1..1 => 0..2 => 0..1 => 0..w/h 
        // type('Obj', (object,), { 'x' : (p.x + 1)/2 * width, 'y' : (p.y + 1)/2 * height})()

        return new Vector2(((point.X + 1) / 2) * screenWidth,
                             (1 - (point.Y + 1) / 2) * screenHeight);
    }

    static Vector2 project(Vector3 point)
    {
        //(x,y,z) -> 3d point behind screen

        // # points onScreened on screen
        // x' = x/z
        // y' = y/z

        return new Vector2(point.X / point.Z, point.Y / point.Z);
    }

    static Vector3 translate_z(Vector3 point, float dz)
    {
        // dz ~ delta z
        return new Vector3(point.X, point.Y, point.Z + dz);
    }

    static void point(Vector2 point)
    {
        float size = 10.0f;
        DrawRectangle((int)(point.X - (size / 2)), (int)(point.Y - (size / 2)), (int)size, (int)size, Color.Green);
    }

    static void line(Vector2 startPos, Vector2 endPos)
    {

        DrawLineV(startPos, endPos, Color.Pink);
    }

    static void drawShape(float[][] vectors, int[][] faces)
    {
        foreach (int[] i in faces)
        {
            for (int j = 0; j < i.Length; j++)
            {
                float[] a = vectors[i[(j + 1) % i.Length]];
                float[] b = vectors[i[(j + 2) % i.Length]];
                Vector3 A = new Vector3(a[0], a[1], a[2]);
                Vector3 B = new Vector3(b[0], b[1], b[2]);
                line(
                   onScreen(project(translate_z(A, universeCenter))),
                   onScreen(project(translate_z(B, universeCenter)))
               );
            }
        }
    }

    // public static void drawShapeObj(Shape shape)
    // {
    //     foreach(Face i in shape.faces)
    //     {
    //         for (int j = 0; j < i.Length; j++)
    //         {
    //             Vector3 A = shape.vectors[i[(j + 1) % i.Length]];
    //             Vector3 B = shape.vectors[i[(j + 2) % i.Length]];

    //             line(
    //                onScreen(project(translate_z(rotate_yz(rotate_xz(A, xz_angle), yz_angle), universeCenter))),
    //                onScreen(project(translate_z(rotate_yz(rotate_xz(B, xz_angle), yz_angle), universeCenter)))
    //            );
    //         }
    //     }
    // }

    public static void drawShapeObj1(Shape shape)
    {
        foreach (Face face in shape.faces)
        {
            var indices = face.vertex_indicies; // this can be better

            for (int i = 0; i < indices.Count; i++)
            {
                /*
                Example:
                We have 3 vectors ~ v0, v1, v2

                we draw lines v0 » v1, v1 » v2, v2 » v0.
                */

                //indices
                int idxA = indices[i];
                int idxB = indices[(i + 1) % indices.Count];

                // accessing the vectors via the indicies
                Vector3 A = shape.vectors[idxA];
                Vector3 B = shape.vectors[idxB];

                // Transform 3d » into 2d (amongst other transformations).
                var pointA = onScreen(project(translate_z(rotate_yz(rotate_xz(A, xz_angle), yz_angle), universeCenter)));
                var pointB = onScreen(project(translate_z(rotate_yz(rotate_xz(B, xz_angle), yz_angle), universeCenter)));

                //using said vectors to draw the lines between them
                line(pointA, pointB);
            }
        }
    }

    public static void drawShapeObj(Shape shape)
    {
        foreach (Face face in shape.faces)
        {
            var indices = face.vertex_indicies; // this can be better

            // get the vector indices » get the 3d Vectors » transform into 2D vectors » draw polygon with that

            Vector2 [] vectors = new Vector2[3];

            for(int i = 0; i < indices.Count; i++)
            {
                vectors[i] = onScreen(project(translate_z(rotate_yz(rotate_xz(shape.vectors[indices[i]], xz_angle), yz_angle), universeCenter)));
            }

            DrawTriangle(vectors[0], vectors[1], vectors[2], Color.DarkBlue);
        }
    }
   
    public static void drawShapeObj2(Shape shape)
    {
        foreach (Face face in shape.faces)
        {
            var indices = face.vertex_indicies; // this can be better

            // get the vector indices » get the 3d Vectors » transform into 2D vectors » draw polygon with that

            Vector2 [] vectors = new Vector2[3];

            for(int i = 0; i < indices.Count; i++)
            {
                vectors[i] = onScreen(project(translate_z(rotate_yz(rotate_xz(shape.vectors[indices[i]], xz_angle), yz_angle), universeCenter)));
            }

            // DrawTexturePoly(texture, new Vector2(400, 300),points,texcoords, points.Length, Color.White);
        }
    }

    public static Vector3 rotate_xz(Vector3 point, double angle)
    {

        float C = (float)Math.Cos(angle);
        float S = (float)Math.Sin(angle);

        // \(x^{\prime }=x\cos \theta +z\sin \theta \)

        // \(z^{\prime }=-x\sin \theta +z\cos \theta \)

        return new Vector3(
            (point.X * C) + (point.Z * S),
            point.Y,
            (-(point.X * S)) + (point.Z * C)
        );
    }

    public static Vector3 rotate_xy(Vector3 point, double angle)
    {

        float C = (float)Math.Cos(angle);
        float S = (float)Math.Sin(angle);

        // \(x^{\prime }=x\cos \theta +z\sin \theta \)

        // \(z^{\prime }=-x\sin \theta +z\cos \theta \)

        return new Vector3(
            (point.X * C) - (point.Y * S),
            (point.X * S) + (point.Y * C),
            point.Z
        );
    }

    public static Vector3 rotate_yz(Vector3 point, double angle)
    {

        float C = (float)Math.Cos(angle);
        float S = (float)Math.Sin(angle);

        // \(x^{\prime }=x\cos \theta +z\sin \theta \)

        // \(z^{\prime }=-x\sin \theta +z\cos \theta \)

        return new Vector3(
            point.X,
            (point.Y * C) - (point.Z * S),
            (point.Y * S) + (point.Z * C)
        );
    }

    public static void Scroll(float wheel)
    {
        if (wheel > 0 && universeCenter <= scroll_limit[1])
        {
            universeCenter += 0.75f;
        }
        else if (wheel < 0 && universeCenter >= scroll_limit[0])
        {
            universeCenter -= 0.75f;
        }


    }

    public static void Rotate(Vector2 delta)
    {
        //     # yz_angle change
        // if rel_y > 0:
        //     yz_angle -= math.pi * (rel_y*0.001)
        // elif rel_y < 0:
        //     yz_angle -= math.pi * (rel_y*0.001)

        // X - AXIS ROTATION
        if (delta.Y > 0) yz_angle -= Math.PI * (delta.Y * 0.001);
        else if (delta.Y < 0) yz_angle -= Math.PI * (delta.Y * 0.001);

        // # xz_angle change
        // if rel_x > 0:
        //     xz_angle += math.pi * (rel_x*0.001)
        // elif rel_x < 0:
        //     xz_angle += math.pi * (rel_x*0.001)

        // Y - AXIS ROTATION
        if (delta.X > 0) xz_angle -= Math.PI * (delta.X * 0.001);
        else if (delta.X < 0) xz_angle -= Math.PI * (delta.X * 0.001);

    }

    static void Main()
    {
        // Shape penger = Parser.ReadShape("Penger");
        // Shape cube = Parser.ReadShape("Cube");
        Shape pengerWavefront = Parser.LoadObj("Penger");
        // Shape penguin = Parser.ReadWavefrontObject("Penguin");
        // Shape barbie = Parser.ReadWavefrontObject("Barbie");

        // barbie.Recenter(new Vector3(0,-150,10));
        // penguin.Recenter(new Vector3(0,0,50));
        // barbie.Scale(85f);
        // penguin.Scale(60f);
        pengerWavefront.Recenter(new Vector3(0, -0.5f, 0));

        int currentFps = 60;



        InitWindow(screenWidth, screenHeight, "3D engine");

        SetTargetFPS(currentFps);
        SetWindowPosition(100, -400); // to place on second monitor

        SetExitKey(KeyboardKey.Null);

        bool dragging = false;
        Vector2 lastMousePos = Vector2.Zero;

        while (!WindowShouldClose())
        {

            // Console.WriteLine(deltaCircle.X);
            // angle +=  Math.PI * (1/(double)currentFps * 0.5d);
            // Console.WriteLine(angle);-

            float wheel = GetMouseWheelMove();

            Vector2 mouse = GetMousePosition();

            Scroll(wheel);

            if (IsMouseButtonPressed(MouseButton.Left))
            {
                dragging = true;
                lastMousePos = mouse;
            }

            if (dragging)
            {
                if (IsMouseButtonDown(MouseButton.Left))
                {
                    Vector2 delta = mouse - lastMousePos;

                    Rotate(delta);

                    lastMousePos = mouse;
                }
            }
            else
            {
                dragging = false;
            }


            BeginDrawing();
            ClearBackground(Color.Black);
            // point(onScreen(project(new Vector3(0, 0, 1))));
            // DrawCircleV(deltaCircle, circleRadius, Color.Green);
            // drawShape(VS, FS);
            // drawShapeObj(penguin, angle);
            drawShapeObj(pengerWavefront);
            drawShapeObj1(pengerWavefront);

            // drawShapeObj(pengerWavefront, angle);
            // drawShapeObj(cube, angle);
            // werk on here
            // foreach(int [] i in FS){
            //     for(int j = 0; j<i.Length; j++){
            //     float [] a = VS[i [(j+1)% i.Length ]];
            //     float [] b = VS[i [(j+2)% i.Length ]];
            //     Vector3 A = new Vector3(a[0], a[1], a[2]);  
            //     Vector3 B = new Vector3(b[0], b[1], b[2]);
            //      line(
            //         onScreen(project(translate_z(A, universeCenter))),
            //         onScreen(project(translate_z(B, universeCenter)))
            //     );
            //     }
            // }

            // foreach(float [] v in VS){
            //     point(onScreen(project(translate_z(new Vector3(v[0], v[1], v[2]), universeCenter))));
            // }                
            // for v in VSB:
            //     pointRed(onScreen(project(translate_z(rotate_xz(v, angle), dz))))

            // point(onScreen(project(translate_z(new Vector3(VS[0][0],VS[0][1],VS[0][2]), universeCenter))));
            // point(onScreen(project(translate_z(new Vector3(VS[1][0],VS[1][1],VS[1][2]), universeCenter))));

            // DrawLineV(onScreen(project(translate_z(new Vector3(VS[0][0],VS[0][1],VS[0][2]), universeCenter))),
            //          onScreen(project(translate_z(new Vector3(VS[2][0],VS[2][1],VS[2][2]), universeCenter))), Color.Green);
            // line(
            //     onScreen(project(translate_z(new Vector3(VS[0][0],VS[0][1],VS[0][2]), universeCenter))),
            //     onScreen(project(translate_z(new Vector3(VS[1][0],VS[1][1],VS[1][2]), universeCenter)))
            //     );
            EndDrawing();
        }

        CloseWindow();
    }
}