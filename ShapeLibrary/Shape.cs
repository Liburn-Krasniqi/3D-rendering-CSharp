using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization; // Required for CultureInfo, dont really need it i think
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;  // Add this line

namespace ShapeLibrary;

public class Shape 
{
    public string name;
    public Vector3 [] vectors;
    public int[][] faces;
    public Vector3 center;
    public float scale;

    public Shape(string name, Vector3 [] vectors, int[][] faces, Vector3? center = null, float? scale = null)
    {
        this.name = name;
        this.center = center ?? new Vector3(0, 0, 0);  // Use default if null
        this.scale = scale ?? 1.0f;
        if(center!=null)
        {
            this.vectors = vectors.Select(vector => vector + center ?? new Vector3(0, 0, 0)).ToArray();
        }

        if(scale!=null)
        {
            this.vectors = vectors.Select(vector => vector / (scale ?? 1.0f)).ToArray();
        }

        this.vectors = vectors;
        this.faces = faces;
    }

    public void Recenter(Vector3 center)
    {
        this.center = center;
        vectors = vectors.Select(vector => vector + center).ToArray();
    }

    public void Scale(float scale)
    {
        this.scale = scale;
        vectors = vectors.Select(vector => vector / scale).ToArray();
    }

    public void TransformAngle(float scale)
    {
        this.scale = scale;
        vectors = vectors.Select(vector => vector / scale).ToArray();
    }
}

public class Parser{

    public static Shape ReadShape(string shape){

        string filePath = $"/Users/ritech/Desktop/liburn/3D-rendering-CSharp/Shapes/{shape}.txt";

        if (File.Exists(filePath))
        {
            try
            {
                string [] fileContents = File.ReadAllLines(filePath);

                List<Vector3> vectors = new List<Vector3>();
                List<int[]> faces = new List<int[]>();


                Console.WriteLine("Here are the contents of the file:");
                // Console.WriteLine(fileContents);
                Console.WriteLine(fileContents.Length);

                for (int i = 0; i < fileContents.Length;  i++){
                    if(fileContents[i].Contains("vectors") && !fileContents[i].Contains("paths")){
                        Console.WriteLine(fileContents[i]);
                        for(int j = i+1; j < fileContents.Length; j++){
                            if(fileContents[j].Contains("]")) {
                             i=j+1;
                             break;
                             }
                            float [] vector = fileContents[j].Trim().Split(',').Select(s => float.Parse(s)).ToArray();
                            
                            vectors.Add(new Vector3(vector[0], vector[1], vector[2]));

                        }
                    }

                    if(fileContents[i].Contains("vector_paths")){
                        Console.WriteLine(fileContents[i]);
                        for(int j = i+1; j < fileContents.Length; j++){
                            if(fileContents[j].Equals("]")) break;

                            char [] trim = {' ', '[',']', ','};

                            int [] vector_path = fileContents[j].Trim(trim).Split(',').Select(s => int.Parse(s)).ToArray();

                            faces.Add(vector_path);
                        }
                    }
                }
                return new Shape(shape, vectors.ToArray(), faces.ToArray(), null);
            }
            catch (IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return null;
            }
        }
        else
        {
            Console.WriteLine($"File not found: {filePath}");
            return null;
        }
    }



    public static Shape ReadWavefrontObject(string shape)
    {
        string filePath = $"/Users/ritech/Desktop/liburn/3D-rendering-CSharp/Shapes/{shape}/{shape}.obj";

        if (File.Exists(filePath))
        {
            try
            {
                
            string [] fileContents = File.ReadAllLines(filePath);

            List<Vector3> vectors = new List<Vector3>();
            List<int[]> faces = new List<int[]>();

            for (int i = 0; i < fileContents.Length; i++)
            {
                if(fileContents[i].StartsWith("v "))
                {

                    float [] temp = fileContents[i].Substring(1).Trim(' ').Split(' ').Select(s => float.Parse(s)).ToArray();

                    vectors.Add(new Vector3(temp[0], temp[1], temp[2]));

                }else if (fileContents[i].StartsWith("f"))
                {
                    int [] temp = fileContents[i].Substring(2).Trim(' ').Split(' ').Select(s => int.Parse(s.Split('/')[0])-1).ToArray();
                    
                    faces.Add(temp);
                }
            }
            
            return new Shape(shape, vectors.ToArray(), faces.ToArray(), null);
            
            }
            catch(IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return null;
            }
        }
        else
        {
            Console.WriteLine($"File not found: {filePath}");
            return null;
        }
    }
}