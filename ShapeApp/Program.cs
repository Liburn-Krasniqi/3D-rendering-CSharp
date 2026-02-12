using Raylib_cs;
using System;
using System.Numerics;
using ShapeLibrary;
using static Raylib_cs.Raylib;  // Add this line

class Program
{
    const int screenWidth = 800;
    const int screenHeight = screenWidth;
    const int universeCenter = 2;

    public static readonly float[] scroll_limit = [2.1f, 3.2f];

    

    static Vector2 onScreen(Vector2 point){
        // -1..1 => 0..2 => 0..1 => 0..w/h 
        // type('Obj', (object,), { 'x' : (p.x + 1)/2 * width, 'y' : (p.y + 1)/2 * height})()

        return new Vector2(((point.X + 1)/2) * screenWidth,
                             (1- (point.Y + 1)/2) * screenHeight);
    }

    static Vector2 project(Vector3 point){
        //(x,y,z) -> 3d point behind screen

        // # points onScreened on screen
        // x' = x/z
        // y' = y/z
        
        return new Vector2(point.X/point.Z ,point.Y/point.Z);
    }

    static Vector3 translate_z(Vector3 point, float dz){
        // dz ~ delta z
        return new Vector3(point.X ,point.Y, point.Z + dz);
    }

    static void point(Vector2 point){
        float size = 10.0f;
        DrawRectangle((int)(point.X - (size/2)),(int)(point.Y  - (size/2)), (int)size, (int)size, Color.Green);
    }
    
    static void line(Vector2 startPos, Vector2 endPos){

        DrawLineV(startPos, endPos, Color.Pink);
    }

    static void drawShape(float [][] vectors, int [][] faces){
        foreach(int [] i in faces){
                    for(int j = 0; j<i.Length; j++){
                    float [] a = vectors[i [(j+1)% i.Length ]];
                    float [] b = vectors[i [(j+2)% i.Length ]];
                    Vector3 A = new Vector3(a[0], a[1], a[2]);  
                    Vector3 B = new Vector3(b[0], b[1], b[2]);
                     line(
                        onScreen(project(translate_z(A, universeCenter))),
                        onScreen(project(translate_z(B, universeCenter)))
                    );
                    }
                }
    }

    static void drawShapeObj(Shape shape, double angle){
        foreach(int [] i in shape.faces){
                    for(int j = 0; j<i.Length; j++){
                    Vector3 A = shape.vectors[i [(j+1)% i.Length ]];
                    Vector3 B = shape.vectors[i [(j+2)% i.Length ]];

                     line(
                        onScreen(project(translate_z(rotate_xz(A, angle), universeCenter))),
                        onScreen(project(translate_z(rotate_xz(B, angle), universeCenter)))
                    );
                    }
                }
    }

    public static Vector3 rotate_xz(Vector3 point, double angle){

    float C =(float) Math.Cos(angle);
    float S =(float) Math.Sin(angle);

    // \(x^{\prime }=x\cos \theta +z\sin \theta \)

    // \(z^{\prime }=-x\sin \theta +z\cos \theta \)

    return new Vector3(
        (point.X*C) + (point.Z*S),
        point.Y,
        (-(point.X*S)) + (point.Z*C)
    );
    }

    public static Vector3 rotate_xy(Vector3 point, double angle){

    float C =(float) Math.Cos(angle);
    float S =(float) Math.Sin(angle);

    // \(x^{\prime }=x\cos \theta +z\sin \theta \)

    // \(z^{\prime }=-x\sin \theta +z\cos \theta \)

    return new Vector3(
        (point.X*C) - (point.Y*S),
        (point.X*S) + (point.Y*C),
        point.Z
    );
    }

    public static Vector3 rotate_yz(Vector3 point, double angle){

    float C =(float) Math.Cos(angle);
    float S =(float) Math.Sin(angle);

    // \(x^{\prime }=x\cos \theta +z\sin \theta \)

    // \(z^{\prime }=-x\sin \theta +z\cos \theta \)

    return new Vector3(
        point.X,
        (point.Y*C) - (point.Z*S),
        (point.Y*S) + (point.Z*C)
    );
    }

    public static void Scroll(float wheel)
    {
        
    }

    static void Main()
    {
        // Shape penger = Parser.ReadShape("Penger");
        // Shape cube = Parser.ReadShape("Cube");
        // Shape pengerWavefront = Parser.ReadWavefrontObject("Penger");
        // Shape penguin = Parser.ReadWavefrontObject("Penguin");
        Shape barbie = Parser.ReadWavefrontObject("Barbie");

        barbie.Recenter(new Vector3(0,-150,10));
        // penguin.Recenter(new Vector3(0,0,50));
        barbie.Scale(85f);
        // penguin.Scale(60f);
        // pengerWavefront.Recenter(new Vector3(0,-0.5f,0));

        int currentFps = 60;
        double angle = 80;

        InitWindow(screenWidth, screenHeight, "3D engine");

        SetTargetFPS(currentFps);
        SetWindowPosition(100, -400); // to place on second monitor

        SetExitKey(KeyboardKey.Null);

        Vector2 deltaCircle = new Vector2(0, (float)screenHeight/3.0f);

        const float speed = 10.0f;

        while (!WindowShouldClose())
        {
            if (deltaCircle.X > screenWidth) deltaCircle.X = 0;
            deltaCircle.X += GetFrameTime()*6.0f*speed;

            // Console.WriteLine(deltaCircle.X);
            angle +=  Math.PI * (1/(double)currentFps * 0.5d);
            // Console.WriteLine(angle);

            float wheel = Raylib.GetMouseWheelMove();

            Scroll(wheel);

            BeginDrawing();
                ClearBackground(Color.Black);
                // point(onScreen(project(new Vector3(0, 0, 1))));
                // DrawCircleV(deltaCircle, circleRadius, Color.Green);
                // drawShape(VS, FS);
                // drawShapeObj(penguin, angle);
                drawShapeObj(barbie, angle);
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