using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour {

    [SerializeField] TriangleSurface triangleSurface;

    [SerializeField][Range(0.0f, 10.0f)] private float bouncyness = 1.0f;
    
    float radius = 3f;

    Vector3 gravity = Physics.gravity;

    Vector3 currentVelocity;
    Vector3 newVelocity;
    Vector3 acceleration;
    float mass = 3.0f;
    float KG; 
    float newton;
    double TIME;
    
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

        
        
        //Vector3 newPosition = new Vector3();
        //Vector3 previousPosition = new Vector3();
        //previousPosition = transform.position;

        //newton = gravity * mass
        
        if (CheckCollision()) {
            Vector3 normalForce = -Vector3.Dot(triangleSurface.normalVector, gravity * mass) * triangleSurface.normalVector;
            acceleration = (normalForce + gravity) / mass;
            Vector3 Vnormal = Vector3.Dot(currentVelocity, triangleSurface.normalVector) * triangleSurface.normalVector; 
            newVelocity = currentVelocity - 2*Vnormal;
        }
        else {
            acceleration = gravity;
            newVelocity = currentVelocity + acceleration * Time.fixedDeltaTime;
            currentVelocity = newVelocity;
        }
        
        newPosition = previousPosition + newVelocity * Time.fixedDeltaTime;
        previousPosition = newPosition;
        transform.position = newPosition;
    }

    Vector3 ApplyForces() {

        

        
        
        return newVelocity;
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
            Vector3 collisionPos = pos + dotProduct * normalVec;
            //Debug.Log("Absolute value: " + Mathf.Abs(dotProduct));// + "\nCollision point: " + collisionPos);
            return true;
        }
        return false;
    }
    
    void SetVelocity() {
        
    }
}
