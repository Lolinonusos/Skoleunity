using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtSun : MonoBehaviour {
    [SerializeField]GameObject sunObject;
    
    // Update is called once per frame
    void Update() {
        Vector3 relativePos = sunObject.transform.position - transform.position;
        
        transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
    }
}
