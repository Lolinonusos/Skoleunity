using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Indirect : MonoBehaviour
{
// Filadresse
    [SerializeField]string vertexData;
    
    [SerializeField]private Mesh mesh;
    [SerializeField]private Material material;

    // Punkt koordinater
    Vector3[] vertices; 
    
    ComputeBuffer positionBuffer;
    ComputeBuffer argsBuffer;
    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private int subMeshIndex = 0;
    
    private float xAvg;
    private float yAvg;
    private float zAvg;
    
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
          
            vertices[counter] = new Vector3(x, y, z);

            counter++;
        }

        print("Points: " + vertices.Length);
        
        // print("xMin: " + xMin);
        // print("xMax: " + xMax);
        // print("yMin: " + yMin);
        // print("yMax: " + yMax);
        // print("zMin: " + zMin);
        // print("zMax: " + zMax);

        xAvg = 0.5f * (xMin + xMax);
        yAvg = 0.5f * (yMin + yMax);
        zAvg = 0.5f * (zMin + zMax);
        
        print("Avg values: " + new Vector3(xAvg, yAvg, zAvg));

        // Sentrer punkter til origo
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i].x -= 0.5f * (xMin + xMax);
            vertices[i].y -= 0.5f * (yMin + yMax);
            vertices[i].z -= 0.5f * (zMin + zMax);
        }
        
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    // Update is called once per frame
    void Update() {
        Graphics.DrawMeshInstancedIndirect(mesh, subMeshIndex, material, new Bounds(Vector3.zero, new Vector3(xAvg, yAvg, zAvg) * 3), argsBuffer);
    }

    private void OnDisable()
    {
        if (positionBuffer != null) {positionBuffer.Release();}
        positionBuffer = null;
        
        if (argsBuffer != null) {argsBuffer.Release();}
        argsBuffer = null;
    }

    void UpdateBuffers()
    {
        subMeshIndex = Mathf.Clamp(subMeshIndex, 0, mesh.subMeshCount - 1);

        if (positionBuffer != null) { positionBuffer.Release(); }
        
        positionBuffer = new ComputeBuffer(vertices.Length, 16);

        Vector4[] positions = new Vector4[vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            //print("Position: " + vertices[i]);
            positions[i] = new Vector4(vertices[i].x, vertices[i].y, vertices[i].z, 5f);
            print("Positions as Vector4: " + positions[i]);
            //print(i);
        }
        
        
        positionBuffer.SetData(positions);
        material.SetBuffer("positionBuffer", positionBuffer);
        //material.SetColorArray();

        args[0] = (uint)mesh.GetIndexCount(subMeshIndex);
        args[1] = (uint)vertices.Length;
        args[2] = (uint)mesh.GetIndexStart(subMeshIndex);
        args[3] = (uint)mesh.GetBaseVertex(subMeshIndex);
        
        argsBuffer.SetData(args);
    }
}
