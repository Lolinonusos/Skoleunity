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
    
    ComputeBuffer argsBuffer;
    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private int subMeshIndex = 0;

    ComputeBuffer positionBuffer;
    GraphicsBuffer commandBuffer;
    GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;
    const int commandCount = 1;
    
    Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
    
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

        //print("Points: " + vertices.Length);

        xAvg = 0.5f * (xMin + xMax);
        yAvg = 0.5f * (yMin + yMax);
        zAvg = 0.5f * (zMin + zMax);
        
        //print("Avg values: " + new Vector3(xAvg, yAvg, zAvg));

        // Sentrer punkter til origo
        for (int i = 0; i < vertices.Length; i++) {
            vertices[i].x -= 0.5f * (xMin + xMax);
            vertices[i].y -= 0.5f * (yMin + yMax);
            vertices[i].z -= 0.5f * (zMin + zMax);
        }

        commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, commandCount, GraphicsBuffer.IndirectDrawIndexedArgs.size);
        commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[commandCount];
        positionBuffer = new ComputeBuffer(vertices.Length, 4 * 3);
        positionBuffer.SetData(vertices);
        // argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        //UpdateBuffers();
    }
    
    void Update() {

        Matrix4x4 tempMatrix = Matrix4x4.identity; 
        tempMatrix.SetTRS(new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, scale);
        
        RenderParams rp = new RenderParams(material);
        rp.worldBounds = new Bounds(Vector3.zero, new Vector3(xAvg, yAvg, zAvg));
        rp.matProps = new MaterialPropertyBlock();
        rp.matProps.SetMatrix("objectToWorld", tempMatrix);
        rp.matProps.SetBuffer("positions", positionBuffer);
        commandData[0].indexCountPerInstance = mesh.GetIndexCount(0);
        commandData[0].instanceCount = (uint)vertices.Length;
        //commandData[1].indexCountPerInstance = mesh.GetIndexCount(0);
        //commandData[1].instanceCount = (uint)vertices.Length;
        commandBuffer.SetData(commandData);
        Graphics.RenderMeshIndirect(rp, mesh, commandBuffer, commandCount);
    }

    private void OnDisable() {
        commandBuffer?.Release();
        commandBuffer = null;
    }

    void UpdateBuffers() {
        subMeshIndex = Mathf.Clamp(subMeshIndex, 0, mesh.subMeshCount - 1);

        if (positionBuffer != null) { positionBuffer.Release(); }
        
        positionBuffer = new ComputeBuffer(vertices.Length, 4 * 3);

        Vector4[] positions = new Vector4[vertices.Length];

        for (int i = 0; i < vertices.Length; i++) {
            //print("Position: " + vertices[i]);
            positions[i] = new Vector4(vertices[i].x, vertices[i].y, vertices[i].z);
            print("Positions as Vector4: " + positions[i]);
            //print(i);
        }

        positionBuffer.SetData(vertices);
        material.SetBuffer("positionBuffer", positionBuffer);

        args[0] = (uint)mesh.GetIndexCount(subMeshIndex);
        args[1] = (uint)vertices.Length;
        args[2] = (uint)mesh.GetIndexStart(subMeshIndex);
        args[3] = (uint)mesh.GetBaseVertex(subMeshIndex);
        argsBuffer.SetData(args);
    }
}
