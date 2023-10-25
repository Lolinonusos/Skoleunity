using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PunktskyRender : MonoBehaviour
{
    [SerializeField]string vertexData;

    Vector3[] vertices;
    ComputeBuffer positionsBuffer;

    // Start is called before the first frame update
    void Start() {
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
        
        float zMin = float.MaxValue;
        float zMax = float.MinValue;
        
        StreamReader sr = new StreamReader(vertexData);

        int lineCount = int.Parse(sr.ReadLine());
        vertices = new Vector3[lineCount]; // Give correct array size

        int counter = 0;
        while (!sr.EndOfStream) {

            string tempLine = sr.ReadLine();

            string[] splitLines = tempLine.Split(" ");

            float x = float.Parse(splitLines[0]);
            float y = float.Parse(splitLines[2]);
            float z = float.Parse(splitLines[1]);
          
            Vector3 vertPos = new Vector3(x, y, z); 
            vertices[counter] = vertPos; // Insert at end
            //newTriangles[counter] = counter;

            if (xMax < x) { xMax = x; }
            if (xMin > x) { xMin = x; }

            if (yMax < y) { yMax = y; }
            if (yMin > y) { yMin = y; }
            
            if (zMin > z) { zMin = z; }
            if (zMax > z) { zMax = z; }

            counter++;
        }

        for (int i = 0; i < vertices.Length; i++) {
            vertices[i].x -= 0.5f * (xMin + xMax);
            vertices[i].y -= 0.5f * (yMin + yMax);
            vertices[i].z -= 0.5f * (zMin + zMax);
        }
        
        positionsBuffer = new ComputeBuffer(vertices.Length, 3*4);
        positionsBuffer.SetData(vertices);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
