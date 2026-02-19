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
    // arrays can be of many lengths (+3)
    public List<int> vertex_indicies;  // we use this cause faces have a 3+ number of verteces (triangle, quad, polygon)
    public List<int> texture_indicies;
    public List<int> vertex_normals_indicies;  

    public Face(List<int> vertex_indicies, List<int> texture_indicies, List<int> vertex_normals_indicies)
    {
        this.vertex_indicies = vertex_indicies;
        this.texture_indicies = texture_indicies;
        this.vertex_normals_indicies = vertex_normals_indicies;
    }
    public Face()
    {
        vertex_indicies = new List<int>();
        texture_indicies = new List<int>();
        vertex_normals_indicies = new List<int>();
    }

    // public void Add(int [] vertex_indicies)
    // {
    //     this.vertex_indicies.Add(vertex_indicies);
    // }

    public void AddVI(List<int> vertex_indicies)
    {
        this.vertex_indicies = vertex_indicies;
    }
    public void AddTI(List<int> texture_indicies)
    {
        this.texture_indicies = texture_indicies;
    }

    public void AddNI(List<int> vertex_normals_indicies)
    {
        this.vertex_normals_indicies = vertex_normals_indicies;
    }
}

public class Shape 
{
    public string name;
    public Vector3 [] vectors;
    public Vector2 [] texture_coordinates;
    public Face [] faces;
    public Vector3 center;
    public float scale;
    public Texture2D texture;

    public Shape(string name, Vector3 [] vectors, Vector2 [] texture_coordinates, Face [] faces, Vector3? center = null, float? scale = null)
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

    public Shape(string name, Vector3 [] vectors, Vector2 [] texture_coordinates, Face [] faces, Texture2D texture, Vector3? center = null, float? scale = null)
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
        this.texture = texture;
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

    public void TransformAngle(float scale)  // huh??
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
        string filePath        = $"/Users/ritech/Desktop/liburn/3D-rendering-CSharp/Shapes/{shape}/{shape}.obj";
        string filePathTexture = $"/Users/ritech/Desktop/liburn/3D-rendering-CSharp/Shapes/{shape}/{shape}.png";

        if (File.Exists(filePath) && File.Exists(filePathTexture))
        {
            try
            {
                
            string [] fileContents = File.ReadAllLines(filePath);
            Texture2D texture = LoadTexture(filePathTexture);
            List<Vector3> vectors = new List<Vector3>();
            List<Vector2> texture_coordinates = new List<Vector2>();
            List<Face> faces = new List<Face>();


            for (int i = 0; i < fileContents.Length; i++)
            {
                
                if(fileContents[i].StartsWith("v "))
                {

                    float [] temp = fileContents[i].Substring(1).Trim(' ').Split(' ').Select(float.Parse).ToArray();    
                    vectors.Add(new Vector3(temp[0], temp[1], temp[2]));

                }
                else if (fileContents[i].StartsWith("vt"))
                {
                    float [] temp = fileContents[i].Substring(2).Trim(' ').Split(' ').Select(float.Parse).ToArray();
                    texture_coordinates.Add(new Vector2(temp[0], temp[1]));
                }
                else if (fileContents[i].StartsWith('f'))
                {
                    Face temp_face = new Face();
                    // reading face vertex indicies
                    var temp = fileContents[i].Substring(2).Trim(' ').Split(' ').Select(s => int.Parse(s.Split('/')[0])-1).ToList();
                    var temp1 = fileContents[i].Substring(2).Trim(' ').Split(' ').Select(s => int.Parse(s.Split('/')[1])-1).ToList();
                    var temp2 = fileContents[i].Substring(2).Trim(' ').Split(' ').Select(s => int.Parse(s.Split('/')[2])-1).ToList();
                    
                    // vertex_indicies.Add(temp);
                    temp_face.AddVI(temp);
                    temp_face.AddTI(temp1);
                    temp_face.AddNI(temp2);

                    faces.Add(temp_face);

                }

            }
            
            return new Shape(shape, vectors.ToArray(), texture_coordinates.ToArray(), faces.ToArray(), texture, null);
            
            }
            catch(IOException e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return null;
            }
        }
        else
        {
            if (!File.Exists(filePath))
                Console.WriteLine($"OBJ file not found: {filePath}");
            else
                Console.WriteLine($"Texture file not found (tried e.g. {shape}.png and {shape?.ToLowerInvariant()}.png in Shapes/{shape}/). Add a PNG there.");
            return null;
        }
    }
}