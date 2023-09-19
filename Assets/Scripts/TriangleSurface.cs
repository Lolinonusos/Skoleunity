using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TriangleSurface : MonoBehaviour {

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    string longString; 
    List<string> eachLine;
    
    [SerializeField]string vertexData;
    [SerializeField]string indicesAndNeighbourData;
    
    // Start is called before the first frame update
    void Start() {
        Mesh mesh = new Mesh(); 
        
        StreamReader sr = new StreamReader("Filepath");

        string[] lines = File.ReadAllLines(vertexData);
        
        // Place vertices
        for (int i = 0; i < UPPER; i++) {
            
        }

        mesh.vertices = newVertices;

        longString = string.Empty;
        longString = indicesAndNeighbourData.text;

        
        // Place indices and neighbours
        for (int i = 0; i < UPPER; i++) {
            
        }
    }

    void readFile() {
        
    }
}
