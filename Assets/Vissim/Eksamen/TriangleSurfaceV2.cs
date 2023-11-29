using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.ShaderGraph.Legacy;

public struct Triangle {
    public int ID;
    public Vector3 normal;
    public Vector3[] vertices;
    public int[] indices;
    public int[] neighbours;

    public Triangle(int newID, Vector3 v0, Vector3 v1, Vector3 v2, int i0, int i1, int i2, int n0, int n1, int n2) {
        ID = newID;
        normal = new Vector3();
        vertices = new Vector3[]{ v0, v1, v2};
        indices = new int[]{ i0, i1, i2 };
        neighbours = new int[]{ n0, n1, n2 };
    }
    
    public Triangle(int newID, Vector3 newNormal, int i0, int i1, int i2, int n0, int n1, int n2) {
        ID = newID;
        normal = newNormal;
        vertices = new Vector3[]{};
        indices = new int[]{ i0, i1, i2 };
        neighbours = new int[]{ n0, n1, n2 };
    }

    public void SetID(int newID) {
        ID = newID;
    }

    public void SetNormal(Vector3 newNormal) {
        normal = newNormal;
    }
}

public class TriangleSurfaceV2 : MonoBehaviour
{
    [SerializeField] private string pointFilePath;
    private Vector3[] points;

    private float xAvg;
    private float yAvg;
    private float zAvg;

    private List<Triangle> triangles = new List<Triangle>();
    
    [SerializeField] private bool isYup = false;
    // Quads to be placed in each direction
    [SerializeField][Range(1, 200)] private int resolution;
    private Mesh mesh;

    [SerializeField][Range(1, 100)] private int skip;
    private float min, max;
    private float depth, height;
    private float size;
    private float stepLength;
    
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
        max = xMax * 0.5f;
        if (xMax < zMax) {
                max = zMax * 0.5f;
        }

        height = yMax;
        
        min = - max;
        size = max - min;
        stepLength = size / resolution;
        print("stepLength: " + stepLength);
        //float hSize = size / 2.0f;
        
        for (int z = 0; z < resolution + 1; z++) {
            for (int x = 0; x < resolution + 1; x++) {
                Vector3 vertex = new Vector3(min + (x * stepLength), 0, min + (z * stepLength));
                vertex.y = CheckForPoints(vertex, stepLength * 0.5f);
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
            // Teller triangler i og opp til nåværende rad:
            int trisUpToThisRow = 2 * x * (resolution);
            
            for (int z = 0; z < resolution; z++) {
                // My own index placing code modified from last year 3D programming
                int i = (x * resolution) + x + z;

                // First triangle (even)
                // Calculating indices
                int I0 = i;
                int I1 = i + resolution + 1;
                int I2 = i + resolution + 2;

                indices.Add(I0);
                indices.Add(I1);
                indices.Add(I2);
                
                // For å finne ut av nabodataene har jeg sett en del på Anders Åsbø's måte å kalkulere naboer på
                // jeg måttet endre litt i kalkuleringene slik at de funker med indekseringen min
                // useful constants
                int evenTriangle = 2 * (x * resolution + z);
                int oddTriangle = evenTriangle + 1;
                
                // calculate neighbour-triangles and set to -1 if out of bounds:
                int T0 = oddTriangle + trisInRow;
                T0 = T0 < totalTriangles ? T0 : -1; 
                
                int T1 = oddTriangle;
                T1 = T1 < totalTriangles ? T1 : -1; 
                
                int T2 = evenTriangle - 1;
                T2 = T2 > trisUpToThisRow ? T2 : -1;
                
                neighbours.Add(T0);
                neighbours.Add(T1);
                neighbours.Add(T2);
                
                // Adding triangle
                Triangle newTri = new Triangle(evenTriangle, vertices[I0], vertices[I1], vertices[I2], I0, I1, I2, T0, T1, T2);
                newTri.SetNormal(CalculateNormalVector(newTri.vertices[0], newTri.vertices[1], newTri.vertices[2]));
                triangles.Add(newTri);

                // Second triangle (odd)
                // Calculating indices
                I0 = i;
                I1 = i + resolution + 2;
                I2 = i + 1;
                
                indices.Add(I0);
                indices.Add(I1);
                indices.Add(I2);
                
                // calculate neighbour-triangles and set to -1 if out of bounds:
                T0 = oddTriangle + 1;
                T0 = T0 < trisUpToThisRow + trisInRow ? T0 : -1;
                
                T1 = evenTriangle - trisInRow;
                T1 = T1 >= 0 ? T1 : -1;

                T2 = evenTriangle;
                T2 = T2 >= 0 ? T2 : -1;

                neighbours.Add(T0);
                neighbours.Add(T1);
                neighbours.Add(T2);
                
                // Adding triangle
                newTri = new Triangle(oddTriangle, vertices[I0], vertices[I1], vertices[I2], I0, I1, I2, T0, T1, T2);
                newTri.SetNormal(CalculateNormalVector(newTri.vertices[0], newTri.vertices[1], newTri.vertices[2]));
                triangles.Add(newTri);
            }
        }

        //for (int i = 0; i < triangles.Count; i++) {
            //triangles.Add(newTri);
            //print("ID: " + triangles[i].ID);
            //print("triangle normal: " + triangles[i].normal);
            //print("Indices: " + triangles[i].indices[0] + ", " + triangles[i].indices[1] + ", " + triangles[i].indices[2]);
            //print("Neighbours: " + triangles[i].neighbours[0] + ", " + triangles[i].neighbours[1] + ", " + triangles[i].neighbours[2]);
        //}
        //print("Total triangles:  "+ triangles.Count);
        
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    // Definerer et kvadratisk område rundt et midtpunkt og skjekker for punkter innenfor kvadratet
    float CheckForPoints(Vector3 vertex, float size) {
        List<float> heightValues = new List<float>();
        float averageHeight = 0;

        // Defining the square area:
        Vector3 topLeft = new Vector3(vertex.x - size, 0.0f, vertex.z + size);
        Vector3 topRight = new Vector3(vertex.x + size, 0.0f, vertex.z + size);
        Vector3 bottomLeft = new Vector3(vertex.x - size, 0.0f,vertex.z - size);
        Vector3 bottomRight = new Vector3(vertex.x + size, 0.0f, vertex.z - size);

        for (int i = 0; i < points.Length; i += skip) {
            // Hvis punkt er inne i området, legg de til i en array
            // Check first triangle
            Vector3 temp;
            temp = CalcBarycentricCoordinate(topLeft, topRight, bottomLeft, points[i]);
            if (temp is {x: >= 0, y: >= 0, z: >= 0}) {
                heightValues.Add(points[i].y);
            }
            else {
                // Check second triangle          
                temp = CalcBarycentricCoordinate(topRight, bottomRight, bottomLeft, points[i]);
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

            baryc = CalcBarycentricCoordinate(v1, v2, v3, objectPos);
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
            normalVector = CalculateNormalVector(v1, v2, v3);
            enteredTriangle = true;
        }
        
        // Convert the barycentric coordinates to world coordinates
        return baryc.x * v1 + baryc.y * v2 + baryc.z * v3;
    }

    // Anders Åsbø
    public Vector3 SurfaceCollision(Vector3 objectPos) {

        //int quadsInRow = Mathf.FloorToInt(stepLength / resolution);
        //print("Size: "+ stepLength);
        //print("QuadsinRow: "+ quadsInRow);

        int i = Mathf.FloorToInt((objectPos.x - min) / stepLength);
        int j = Mathf.FloorToInt((objectPos.z - min) / stepLength);
        //print("i: "+ i);
        //print("j: "+ j);
        
        
        int triangleIndex = 2 * (j + i * resolution);

        // Check to see if index out of bounds, set to 0 if it is 
        triangleIndex = triangleIndex < 0 ? 0 : triangleIndex;
        triangleIndex = triangleIndex > triangles.Count - 1 ? 0 : triangleIndex;
        //print("TriangleIndex: "+ triangleIndex);
        
        //print("Triangles: "+ triangles.Count);
        Triangle currentTri = triangles[triangleIndex];


        int f = 0;
        while (true)
        {
            //Debug.Log("Chinchenghanchi " + f);
            f++;
            //Vector3 a = currentTri.vertices[0];
            //Vector3 b = currentTri.vertices[1];
            //Vector3 c = currentTri.vertices[2];

            Vector3 a = mesh.vertices[currentTri.indices[0]];
            Vector3 b = mesh.vertices[currentTri.indices[1]];
            Vector3 c = mesh.vertices[currentTri.indices[2]];
            // print("a: " + a);
            // print("b: " + b);
            // print("c: " + c);
            
            
            Vector3 uvw = CalcBarycentricCoordinate(a, b, c, objectPos);
            
            if (uvw is {x: >= 0, y: >= 0, z: >= 0}) {
                normalVector = CalculateNormalVector(a, b, c);
                Vector3 hitPos = uvw.x * a + uvw.y * b + uvw.z * c;
                
                // if (previousTriangle != currentTri.ID) {
                //     //print("Entered triangle number: " + currentTriangle);
                //     previousTriangle = currentTri.ID;
                //     previousNormalVector = normalVector;
                //     normalVector = CalculateNormalVector(a, b, c);
                //     // normalVector = currentTri.normal;
                //     enteredTriangle = true;
                // }
                Vector3 difference = hitPos - objectPos;
                hitPos = objectPos + Vector3.Dot(difference, normalVector) * normalVector;
                
                //print("Suksess " + currentTri.ID);
                return hitPos;
            }
            
            // We are not inside the current triangle
            int opposingIndex;
            if (uvw.x <= uvw.y && uvw.x <= uvw.z) {
                opposingIndex = 0;
            }
            else if (uvw.y <= uvw.z) {
                opposingIndex = 1;
            }
            else {
                opposingIndex = 2;
            }

            if (currentTri.neighbours[opposingIndex] >= 0) {
                currentTri = triangles[currentTri.neighbours[opposingIndex]];
                //Debug.Log("Spam? " + currentTri.ID);
                continue;
            }
            
            // No neighbour triangle was found
            Debug.Log("Out of bounds");
            return Vector3.zero;
        }            
        // Debug.Log("Wrong out of bounds");
        // return Vector3.zero;
    }   
    
    public Vector3 CalcBarycentricCoordinate(Vector3 a, Vector3 b, Vector3 c, Vector3 x) {

        Vector2 v0 = new Vector2(b.x, b.z) - new Vector2(a.x, a.z);
        Vector2 v1 = new Vector2(c.x, c.z) - new Vector2(a.x, a.z);
        Vector2 v2 = new Vector2(x.x, x.z) - new Vector2(a.x, a.z);
        
        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;
        
        Vector3 uvw = Vector3.zero;
        
        uvw.y = (d11 * d20 - d01 * d21) / denom;
        uvw.z = (d00 * d21 - d01 * d20) / denom;
        uvw.x = 1.0f - uvw.y - uvw.z;
        
        return uvw;
        //
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ax = x - a; 
        float signedArea = ab.x * ac.z - ac.x * ab.z;
        uvw.y = (ax.x * ac.z - ac.x * ax.z) / signedArea;
        uvw.z = (ab.x * ax.z - ax.x * ab.z) / signedArea;
        uvw.x = 1.0f - uvw.y - uvw.z;
        //
        return uvw;
    }
    
    public Vector3 CalculateNormalVector(Vector3 p1, Vector3 p2, Vector3 p3) {
        // Calculates two vector along the triangle's edge
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;

        // Calculates the cross product of the two vectors to get the normal vector
        return Vector3.Cross(v1, v2).normalized;
        //print("Triangle normal" + normalVector + " Magnitude: " + normalVector.magnitude);
    }
}
