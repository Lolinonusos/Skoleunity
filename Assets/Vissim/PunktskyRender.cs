using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PunktskyRender : MonoBehaviour
{
    // I denne fila har jeg brukt disse som referanser https://www.youtube.com/watch?v=6mNj3M1il_c og https://github.com/Matthew-J-Spencer/pushing-unity/tree/main/Assets/_Game/Levels
    // Dette er en video og et git repository laget av Matthew "Tarodev" Spencer
    
    [SerializeField]string vertexData;
    
    [SerializeField]private Mesh mesh;
    [SerializeField]private Material material;

    private RenderParams rp;
    
    
    Vector3[] vertices; // Punkt koordinater
    ComputeBuffer positionsBuffer;

    
    
    // GPU instancing
    private int listCount = 0;
    private List<Matrix4x4> vertMatrices = new List<Matrix4x4>();
    private List<List<Matrix4x4>> matrices = new List<List<Matrix4x4>>();

    // Indirect GPU instancing
    uint[] args = { 0, 0, 0, 0, 0 };
    ComputeBuffer argsBuffer;

    ComputeBuffer positionBuffer1, positionBuffer2;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
        
        float zMin = float.MaxValue;
        float zMax = float.MinValue;
        
        StreamReader sr = new StreamReader(vertexData);

        int lineCount = int.Parse(sr.ReadLine());
        vertices = new Vector3[lineCount]; // Give correct array size

        // Leser gjennom fila
        int counter = 0;
        while (!sr.EndOfStream) {

            string tempLine = sr.ReadLine();

            string[] splitLines = tempLine.Split(" ");

            float x = float.Parse(splitLines[0]);
            float y = float.Parse(splitLines[2]);
            float z = float.Parse(splitLines[1]);

            if (xMax < x) { xMax = x; }
            if (xMin > x) { xMin = x; }

            if (yMax < y) { yMax = y; }
            if (yMin > y) { yMin = y; }
            
            if (zMax < z) { zMax = z; }
            if (zMin > z) { zMin = z; }
          
            Vector3 vertPos = new Vector3(x, y, z); 
            vertices[counter] = vertPos; // Insert at end
            //newTriangles[counter] = counter;

            counter++;
        }

        print("Points: " + vertices.Length);
        
        print("xMin: " + xMin);
        print("xMax: " + xMax);
        print("yMin: " + yMin);
        print("yMax: " + yMax);
        print("zMin: " + zMin);
        print("zMax: " + zMax);

        // Sentrer punkter til origo
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i].x -= 0.5f * (xMin + xMax);
            vertices[i].y -= 0.5f * (yMin + yMax);
            vertices[i].z -= 0.5f * (zMin + zMax);
        }
        
        //vertMatrices = new Matrix4x4[vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {
        
            Matrix4x4 matrixToAdd = Matrix4x4.Translate(vertices[i]);
            
            vertMatrices.Add(matrixToAdd);

            if (vertMatrices.Count > 100000) {
                 listCount++;
                 matrices.Add(vertMatrices);
                 vertMatrices.Clear();
             }
        }
        
        // GPU instancing
        rp = new RenderParams(material);
        
        //positionsBuffer = new ComputeBuffer(vertices.Length, 3*4);
        //positionsBuffer.SetData(vertices);
        
        // Indirect GPU instancing
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
    }

    // Update is called once per frame
    void Update() {

        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(Vector3.zero, Vector3.one), argsBuffer);
        
        for (int i = 0; i < listCount; i++)
        {
            Graphics.RenderMeshInstanced(rp, mesh, 0, matrices[i]);
        }
    }


    void UpdateBuffers() {
        
    }
}
