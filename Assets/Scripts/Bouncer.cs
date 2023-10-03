using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour {
    public int pointsAwarded = 10;

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == "DaBall") {
            // Gib points
        }
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
