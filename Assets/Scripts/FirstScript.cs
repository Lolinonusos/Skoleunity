using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FirstScript : MonoBehaviour {
    public float xMove;
    public float yMove;
    [SerializeField] float zMove;
    [SerializeField] float speedVal = 10.0f;

    float laTime = 0;
    
    // Start is called before the first frame update
    void Start() {
        Debug.Log("Hello world!");
    }

    // Update is called once per frame
    void Update() {
        Debug.Log("Updating working fine! ;)");

        xMove = Input.GetAxis("Horizontal") * Time.deltaTime * speedVal;
        zMove = Input.GetAxis("Vertical") * Time.deltaTime * speedVal;
        
        transform.Translate(xMove, yMove, zMove);

        laTime += Time.deltaTime;
        if (laTime >= 2) {
            speedVal += 2;
            laTime = 0;
        }
                        

    }
    
    
}
