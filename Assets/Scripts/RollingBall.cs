using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour {

    [SerializeField] TriangleSurface triangleSurface;

    float RADIUS = 3f;
    
    Vector3 GRAVITY = new Vector3(0.0f, -9.8f, 0.0f);

    Vector3 VELOCITY;
    Vector3 ACCELERATION;
    float MASS = 3.0f;
    float KG; 
    float NEWTON;
    double TIME;
    
    // Start is called before the first frame update
    void Awake() {
        TIME = 0.0f;
        transform.localScale = new Vector3(RADIUS * 2.00f, RADIUS * 2.00f, RADIUS * 2.00f);
        
        //VELOCITY = Vector3.ProjectOnPlane(VELOCITY, planeNormal);
    }

    // Update is called once per frame
    void FixedUpdate() {
        TIME += Time.deltaTime;

        
        //Vector3 newPosition = new Vector3();
        //Vector3 previousPosition = new Vector3();
        Vector3 previousPosition = transform.position;
        
        transform.position = previousPosition + ApplyForces() * Time.fixedDeltaTime;
        //transform.position += ApplyForces();
    }

    Vector3 ApplyForces() {

        ACCELERATION = GRAVITY;
        
        Vector3 newVelocity = new Vector3();
        newVelocity = VELOCITY + ACCELERATION * Time.deltaTime;
        VELOCITY = newVelocity;

        float distance = Vector3.Distance(transform.position, triangleSurface.baryc(new Vector2(transform.position.x, transform.position.z)));
        if (distance < RADIUS) {
            print("cumming!!!!!");
        }
        
        return VELOCITY;
    }
    
    void SetVelocity() {
        
    }
}
