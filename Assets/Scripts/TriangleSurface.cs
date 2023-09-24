using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TriangleSurface : MonoBehaviour {

    Vector3[] newVertices;
    int[] newTriangles = {
        0, 3, 1,
        1, 3, 4,
        1, 4, 5,
        1, 5, 2
    };

    string longString; 
    List<string> eachLine;
    
    [SerializeField]string vertexData;
    [SerializeField]string indicesAndNeighbourData;

    public Vector3 normalVector;

    private int previousTriangle = -1;
    private int currentTriangle = 0;
    // Start is called before the first frame update
    void Start() {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        StreamReader sr = new StreamReader(vertexData);

        int lineCount = int.Parse(sr.ReadLine());
        newVertices = new Vector3[lineCount]; // Give correct array size

        int counter = 0;
        while (!sr.EndOfStream) {

            string tempLine = sr.ReadLine();

            string[] splitLines = tempLine.Split(" ");

            float x = float.Parse(splitLines[0]);
            float y = float.Parse(splitLines[1]);
            float z = float.Parse(splitLines[2]);

            Vector3 vertPos = new Vector3(x, y, z);
            newVertices[counter] = vertPos; // Insert at end
            
            counter++;
        }

        for (int i = 0; i < newVertices.Length; i++) {
            Debug.Log(newVertices[i]);
        }
  
        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;

        mesh.RecalculateNormals();


        // Place indices and neighbours
        //for (int i = 0; i < UPPER; i++) {

        //}
    }

    public Vector3 baryc(Vector2 objectPosition) {

        Vector3 v1 = new Vector3();
        Vector3 v2 = new Vector3();
        Vector3 v3 = new Vector3();

        Vector3 baryc = new Vector3(-1, -1 , -1);
        
        for (int i = 0; i < newTriangles.Length / 3; i++) {
            int i1 = newTriangles[i * 3];
            int i2 = newTriangles[i * 3 + 1];
            int i3 = newTriangles[i * 3 + 2];

            v1 = newVertices[i1];
            v2 = newVertices[i2];
            v3 = newVertices[i3];

            baryc = getBarycentricCoordinate(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z), new Vector2(v3.x, v3.z), objectPosition);
            if (baryc is { x: >= 0, y: >= 0, z: >= 0 }) {
                currentTriangle = i;
                break;
            }
        }

        // Check if we are in a different triangle, update normal vector if true
        if (previousTriangle != currentTriangle) {
            previousTriangle = currentTriangle;
            CalculateNormalVector(v1, v2, v3);
            print("Entered triangle number: " + currentTriangle);
        }

        return baryc.x * v1 + baryc.y * v2 + baryc.z * v3;
    }

    Vector3 getBarycentricCoordinate(Vector2 a, Vector2 b, Vector2 c, Vector2 x) {

        Vector2 v0 = b - a;
        Vector2 v1 = c - a;
        Vector2 v2 = x - a;

        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        return new Vector3(u, v, w);
    }
    
    private void CalculateNormalVector(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // Calculates two vector along the triangle's edge
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;
    
        // Calculates the cross product of the two vectors to get the normal vector
        normalVector = Vector3.Cross(v1, v2).normalized;
        //print("Triangle normal" + normalVector + " Magnitude: " + normalVector.magnitude);
    }
}
