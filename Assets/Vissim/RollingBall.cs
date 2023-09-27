using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class RollingBall : MonoBehaviour {

    [SerializeField] TriangleSurface triangleSurface;

    [SerializeField][Range(0.0f, 10.0f)] private float bouncyness = 1.0f;
    
    float radius = 3f;

    Vector3 gravity = Physics.gravity;

    Vector3 currentVelocity = new();
    Vector3 newVelocity = Vector3.zero;
    //Vector3 currentAcceleration;
    Vector3 acceleration = Vector3.zero;
    float mass = 2.0f;
    float KG; 
    float newton;
    double TIME;

    private float barycY;
    
    Vector3 previousPosition;
    Vector3 newPosition;
    
    // Start is called before the first frame update
    void Awake() {
        previousPosition = transform.position;
        newPosition = transform.position;
        TIME = 0.0f;
        transform.localScale = new Vector3(radius * 2.00f, radius * 2.00f, radius * 2.00f);
        
        //VELOCITY = Vector3.ProjectOnPlane(VELOCITY, planeNormal);
    }

    // Update is called once per frame
    void FixedUpdate() {
        TIME += Time.deltaTime;

        bool collision = false;

        if (CheckCollision())
        {
            collision = true;
            
            Vector3 surfaceNormal = triangleSurface.normalVector;
            Vector3 normalForce = -Vector3.Dot(gravity, surfaceNormal) * surfaceNormal;
            Vector3 force = gravity + normalForce;
            acceleration = force;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, surfaceNormal);
            
            if (triangleSurface.enteredTriangle)
            {
                triangleSurface.enteredTriangle = false;
                normalForce = (surfaceNormal + surfaceNormal).normalized;
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalForce);
                print("TIME: " + TIME);
                print("NORMAL: " + triangleSurface.normalVector);
                print("ACCELERATION: " + acceleration + "  " + acceleration.magnitude);
                //print("NORMAL: " + reflectionNormal);
                print("VELOCITY: " + newVelocity + "  " + newVelocity.magnitude);
                print("POSITION: " + newPosition);
                TIME = 0;
            }
        }
        else {
            collision = false;

            // Constantly applies gravity
            acceleration = gravity * mass;
        }
        
        Debug.DrawRay(transform.position, acceleration, Color.green);
        newVelocity = currentVelocity + acceleration * Time.fixedDeltaTime;
        currentVelocity = newVelocity;

        Debug.DrawRay(transform.position, newVelocity, Color.blue);

        newPosition = transform.position + newVelocity * Time.fixedDeltaTime;
        // if (collision) {
        //     transform.position = new Vector3(newPosition.x, barycY + radius, newPosition.z);
        // }
        // else {
        // }
        transform.position = newPosition;
    }

    private bool CheckCollision()
    {
        // Using (k = C + ((S - C) . n) * n when |(S - C) . n| <= r) to calculate the collision point
        Vector3 pos = transform.position; // C
        Vector3 baryc = triangleSurface.baryc(new Vector2(pos.x, pos.z)); // S
        Vector3 normalVec = triangleSurface.normalVector; // n

        float dotProduct = Vector3.Dot(baryc - pos, normalVec);
        
        if (Mathf.Abs(dotProduct) <= radius)
        {
            barycY = baryc.y;
            Vector3 collisionPos = pos + dotProduct * normalVec;
            //Debug.Log("CBT");
            return true;
        }
        return false;
    }
    
    void SetVelocity() {
        
    }
}
