using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization; // Required for CultureInfo, dont really need it i think
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;  // Add this line

namespace ShapeLibrary;

public class Face
{
    public int [] vertex_indicies;
    public int [] texture_indicies;

    // public int [] vertex_normals_indicies;


    public Face(int [] vertex_indicies, int [] texture_indicies)
    {
        this.vertex_indicies = vertex_indicies;
        this.texture_indicies = texture_indicies;

    }
}

public class Shape 
{
    public string name;
    public Vector3 [] vectors;
    public Vector2 [] texture_coordinates;
    public int[][] faces;
    public Vector3 center;
    public float scale;

    public Shape(string name, Vector3 [] vectors, Vector2 [] texture_coordinates, int[][] faces, Vector3? center = null, float? scale = null)
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
        this.texture_coordinates = texture_coordinates;
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
    /*
    Parser takes in file path (for now just the shape name)

    Reads the object's:
     - Vertices (v) 
     - Vertex texture coordinates (vt)
     - Vertex normals (vn)
     - Face elements (f) which are: 
        - Vertex Indicies (1-based indexed)
        - Vertex texture coordinate indices (1-based indexed)
        - Vertex normal indices (1-based indexed)
    */
    public static Shape LoadObj(string shape)
    {
        string filePath = $"/Users/ritech/Desktop/liburn/3D-rendering-CSharp/Shapes/{shape}/{shape}.obj";

        if (File.Exists(filePath))
        {
            try
            {
                
            string [] fileContents = File.ReadAllLines(filePath);

            List<Vector3> vectors = new List<Vector3>();
            List<Vector2> texture_coordinates = new List<Vector2>();
            List<int[]> faces = new List<int[]>();

            for (int i = 0; i < fileContents.Length; i++)
            {
                if(fileContents[i].StartsWith("v "))
                {

                    float [] temp = fileContents[i].Substring(1).Trim(' ').Split(' ').Select(s => float.Parse(s)).ToArray();

                    vectors.Add(new Vector3(temp[0], temp[1], temp[2]));

                }
                else if (fileContents[i].StartsWith("vt"))
                {
                    float [] temp = fileContents[i].Substring(2).Trim(' ').Split(' ').Select(s => float.Parse(s)).ToArray();
                    texture_coordinates.Add(new Vector2(temp[0], temp[1]));
                }
                else if (fileContents[i].StartsWith("f"))
                {
                    int [] temp = fileContents[i].Substring(2).Trim(' ').Split(' ').Select(s => int.Parse(s.Split('/')[0])-1).ToArray();
                    
                    faces.Add(temp);
                }
            }
            
            return new Shape(shape, vectors.ToArray(), texture_coordinates.ToArray(), faces.ToArray(), null);
            
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