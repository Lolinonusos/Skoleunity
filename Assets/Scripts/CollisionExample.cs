using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionExample : MonoBehaviour
{
    

    void OnCollisionEnter(Collision collision) {
        Debug.LogWarning($"Object {collision.body.name} collided with collider.");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
