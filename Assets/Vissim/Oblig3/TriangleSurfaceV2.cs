using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TriangleSurfaceV2 : MonoBehaviour
{
    [SerializeField] private string pointFilePath;
    private Vector3[] points;

    [SerializeField] private bool isYup = true;
    
    // Squares to be placed in each direction
    [SerializeField] private float resolution; 
    
    
    void Start()
    {   
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
            
        float yMin = float.MaxValue;
        float yMax = float.MinValue;
            
        float zMin = float.MaxValue;
        float zMax = float.MinValue;
            
        StreamReader sr = new StreamReader(pointFilePath);

        int lineCount = int.Parse(sr.ReadLine());
        points = new Vector3[lineCount]; // Give correct array size

        // Leser gjennom fila
        int counter = 0;
        while (!sr.EndOfStream) {

            string tempLine = sr.ReadLine();

            string[] splitLines = tempLine.Split(" ");
            
            float x = float.Parse(splitLines[0]);
            float y;
            float z;
            if (isYup) {
                y = float.Parse(splitLines[2]);
                z = float.Parse(splitLines[1]);
            }
            else {
                y = float.Parse(splitLines[1]);
                z = float.Parse(splitLines[2]);
            }

            if (xMax < x) { xMax = x; }
            if (xMin > x) { xMin = x; }

            if (yMax < y) { yMax = y; }
            if (yMin > y) { yMin = y; }
                
            if (zMax < z) { zMax = z; }
            if (zMin > z) { zMin = z; }
              
            Vector3 pointPos = new Vector3(x, y, z); 
            points[counter] = pointPos; // Insert at end
            //newTriangles[counter] = counter;

            counter++;
        }

        print("Total amount of points: " + points.Length);

        // Sentrer punkter til origo
        for (int i = 0; i < points.Length; i++) {
            points[i].x -= 0.5f * (xMin + xMax);
            points[i].y -= 0.5f * (yMin + yMax);
            points[i].z -= 0.5f * (zMin + zMax);
        }
        
        
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
