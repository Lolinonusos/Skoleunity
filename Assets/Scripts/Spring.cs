using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    
    float chargeTime = 0.00f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) {
            chargeTime += Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.Space)) {
            chargeTime = 0.00f;
        }
        print(chargeTime);
    }
}
