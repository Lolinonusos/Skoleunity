using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {
    [SerializeField] Vector3 orbitPoint = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] GameObject orbitObject;
    
    
    [SerializeField][Range(0.0f, 100.0f)] float orbitSpeed = 10.0f;
    [SerializeField] Vector3 orbitAxis = new Vector3(0.0f, 1.0f, 0.0f);

    [SerializeField] float orbitRadius;
    
    // Start is called before the first frame update
    void Start() {
        if (orbitRadius < 0.5f) {
            orbitRadius = 15.0f;
        }
        if (orbitObject) {
            orbitPoint = orbitObject.transform.position;
        }
    }

    // Update is called once per frame
    void Update() {
        transform.RotateAround(orbitPoint, orbitAxis, orbitSpeed * Time.deltaTime);
    }
}
