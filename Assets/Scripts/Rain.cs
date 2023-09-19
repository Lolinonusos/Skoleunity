using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour {
    [SerializeField] float xPos = 0;
    [SerializeField] float yPos = 0;
    [SerializeField] float zPos = 0;

    [SerializeField] float dropMass = 200;
    void Start()
    {
        Destroy(this, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
