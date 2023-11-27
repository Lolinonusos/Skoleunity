using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TriangleSurfaceV2 : MonoBehaviour
{
    [SerializeField] private string pointFilePath;
    private Vector3[] points;

    private float xAvg;
    private float yAvg;
    private float zAvg;
    
    [SerializeField] private bool isYup = false;
    // Quads to be placed in each direction
    [SerializeField][Range(1, 10000)] private int resolution;
    private Mesh mesh;

    private int previousTriangle = -1;
    private int currentTriangle = 0;
    public Vector3 previousNormalVector;
    public Vector3 normalVector;
    public bool enteredTriangle = false;

    List<Vector3> nonModifiedVerts = new List<Vector3>();
    
    void Start() {
        mesh = new Mesh();
        
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
            if (!isYup) {
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

        print("Total amount of points read from file: " + points.Length);

        // Sentrer punkter til origo
        xAvg = 0.5f * (xMin + xMax);
        yAvg = 0.5f * (yMin + yMax);
        zAvg = 0.5f * (zMin + zMax);
        
        for (int i = 0; i < points.Length; i++) {
            points[i].x -= xAvg;
            points[i].y -= yAvg;
            points[i].z -= zAvg;
        }
        // END OF READING FILE AND SETTING UP POINTCLOUD POINTS
        //#####################################################
        
        // Finding new min and max values for vertex placement
        xMin = float.MaxValue;
        xMax = float.MinValue;
        yMin = float.MaxValue;
        yMax = float.MinValue;
        zMin = float.MaxValue;
        zMax = float.MinValue;
        for (int i = 0; i < points.Length; i++) {
            
            float x = points[i].x;
            float y = points[i].y;
            float z = points[i].z;
            
            if (xMax < x) { xMax = x; }
            if (xMin > x) { xMin = x; }

            if (yMax < y) { yMax = y; }
            if (yMin > y) { yMin = y; }
                
            if (zMax < z) { zMax = z; }
            if (zMin > z) { zMin = z; }
        }

        print("Max positions: " + new Vector3(xMax, yMax, zMax));
        print("Min positions: " + new Vector3(xMin, yMin, zMin));
        
        // ############################################
        // SETTING UP VERTICES, INDICES AND NEIGHBOURS
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> indices = new List<int>();
        List<int> neighbours = new List<int>();

        // Make a perfect square
        float max = xMax * 0.5f;
        if (xMax < zMax) {
                max = zMax * 0.5f;
        } 
        
        float min = - max;
        float size = max - min;
        float stepLength = size / resolution;
        //float hSize = size / 2.0f;
        
        for (int z = 0; z < resolution + 1; z++) {
            for (int x = 0; x < resolution + 1; x++) {
                Vector3 vertex = new Vector3(min + (x * stepLength), 0, min + (z * stepLength));
                vertex.y = CheckForPoints(new Vector2(vertex.x, vertex.z), stepLength * 0.5f);
                vertices.Add(vertex);
                
                Vector2 uvTemp = new Vector2(x / (float) resolution, z / (float) resolution);
                uv.Add(uvTemp);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        
        // Indices and neighbours
        int trisInRow = 2 * resolution;
        int totalTriangles = trisInRow * resolution;
        for (int x = 0; x < resolution; x++) {
            // useful constant:
            int trisUpToThisRow = 2 * x * (resolution);
            
            for (int z = 0; z < resolution; z++) {
                int trisUpToThisCol = 2 * z * (resolution - 2);
                
                // My own modified from last year 3D programming
                int i = (x * resolution) + x + z;
                // First triangle
                indices.Add(i);
                indices.Add(i + resolution + 1);
                indices.Add(i + resolution + 2);
                // Second triangle
                indices.Add(i);
                indices.Add(i + resolution + 2);
                indices.Add(i + 1);
                
                // Her har jeg sett en del på Anders Åsbø's måte å kalkulere naboer på
                // jeg måttet endre litt i kalkuleringene slik at de funker med indekseringen min
                // useful constants
                int evenTriangle = 2 * (x * resolution + z);
                int oddTriangle = evenTriangle + 1;
                
                // First (even) triangle's neightbours
                // calculate neighbour-triangles and set to -1 if out of bounds:
                int T0 = oddTriangle + trisInRow;
                T0 = T0 < totalTriangles ? T0 : -1; // Denne funker som den skal
                
                int T1 = oddTriangle;
                T1 = T1 < totalTriangles ? T1 : -1; // Denne funker som den skal
                
                int T2 = evenTriangle - 1;
                T2 = T2 > trisUpToThisRow ? T2 : -1; // Denne funker som den skal
                
                neighbours.Add(T0);
                neighbours.Add(T1);
                neighbours.Add(T2);
                
                //print("First triangle neighbours:  T0: " + T0 + "   T1: " + T1 + "   T2: " + T2);
                
                // Second (odd) triangle's neighbours
                T0 = oddTriangle + 1;
                T0 = T0 < trisUpToThisRow + trisInRow ? T0 : -1;
                
                T1 = evenTriangle - trisInRow;
                T1 = T1 >= 0 ? T1 : -1;

                T2 = evenTriangle;
                T2 = T2 >= 0 ? T2 : -1;

                neighbours.Add(T0);
                neighbours.Add(T1);
                neighbours.Add(T2);
                
                //print("Second triangle neighbours:  T0: " + T0 + "   T1: " + T1 + "   T2: " + T2);
            }
        }

        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Definerer et kvadratisk område rundt et midtpunkt og skjekker for punkter innenfor kvadratet
    float CheckForPoints(Vector2 vertex, float size) {
        List<float> heightValues = new List<float>();
        float averageHeight = 0;

        // Defining the square area:
        Vector2 topLeft = new Vector2(vertex.x - size, vertex.y + size);
        Vector2 topRight = new Vector2(vertex.x + size, vertex.y + size);
        Vector2 bottomLeft = new Vector2(vertex.x - size, vertex.y - size);
        Vector2 bottomRight = new Vector2(vertex.x + size, vertex.y - size);

        for (int i = 0; i < points.Length; i += 50) {
            // Hvis punkt er inne i området, legg de til i en array
            // Check first triangle
            Vector3 temp;
            temp = getBarycentricCoordinate(topLeft, topRight, bottomLeft, new Vector2(points[i].x, points[i].z));
            if (temp is {x: >= 0, y: >= 0, z: >= 0}) {
                heightValues.Add(points[i].y);
            }
            else {
                // Check second triangle          
                temp = getBarycentricCoordinate(topRight, bottomRight, bottomLeft, new Vector2(points[i].x, points[i].z));
                if (temp is {x: >= 0, y: >= 0, z: >= 0}) {
                    heightValues.Add(points[i].y);
                }
            }
        }
        
        //print(heightValues.Count);
        if (heightValues.Count > 0) {
            for (int i = 0; i < heightValues.Count; i++) {
                averageHeight += heightValues[i];
            }
            averageHeight = averageHeight / heightValues.Count;
        }
        return averageHeight;
    }
    
    public Vector3 baryc(Vector2 objectPos) {
        // Returns world coordinate based on the triangles barycentric coordinate
        Vector3 v1 = new Vector3();
        Vector3 v2 = new Vector3();
        Vector3 v3 = new Vector3();

        Vector3 baryc = new Vector3(-1, -1 , -1);
        
        for (int i = 0; i < mesh.triangles.Length / 3; i++) {
            int i1 = mesh.triangles[i * 3 + 1];
            int i2 = mesh.triangles[i * 3 + 2];
            int i3 = mesh.triangles[i * 3 + 0];

            v1 = mesh.vertices[i1];
            v2 = mesh.vertices[i2];
            v3 = mesh.vertices[i3];

            baryc = getBarycentricCoordinate(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z), new Vector2(v3.x, v3.z), objectPos);
            if (baryc is { x: >= 0, y: >= 0, z: >= 0 }) {
                currentTriangle = i;
                break;
            }
        }

        // Check if we are in a different triangle, update normal vector if true
        if (previousTriangle != currentTriangle) {
            //print("Entered triangle number: " + currentTriangle);
            previousTriangle = currentTriangle;
            previousNormalVector = normalVector;
            CalculateNormalVector(v1, v2, v3);
            enteredTriangle = true;
        }
        
        // Convert the barycentric coordinates to world coordinates
        return baryc.x * v1 + baryc.y * v2 + baryc.z * v3;
    }

    Vector3 educatedBaryc() {
        return Vector3.zero;
    }   
    
    public Vector3 getBarycentricCoordinate(Vector2 a, Vector2 b, Vector2 c, Vector2 x) {

        Vector2 v0 = (b - a);
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
    
    private void CalculateNormalVector(Vector3 p1, Vector3 p2, Vector3 p3) {
        // Calculates two vector along the triangle's edge
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;

        // Calculates the cross product of the two vectors to get the normal vector
        normalVector = Vector3.Cross(v1, v2).normalized;
        //print("Triangle normal" + normalVector + " Magnitude: " + normalVector.magnitude);
    }
}
