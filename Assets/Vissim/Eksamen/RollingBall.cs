using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class RollingBall : MonoBehaviour {

    // For physics allowing the ball to collide with a triangle surface
    [SerializeField] TriangleSurfaceV2 triangleSurface;
    //[SerializeField][Range(0.0f, 10.0f)] private float bouncyness = 1.0f;
    [SerializeField]public float radius = 3f;
    Vector3 gravity = Physics.gravity;
    public Vector3 currentVelocity = new();
    public Vector3 newVelocity = Vector3.zero;
    Vector3 acceleration = Vector3.zero;
    float mass = 2.0f;
    float KG; 
    float newton;
    double TIME;
    private float barycY;
    Vector3 previousPosition;
    Vector3 newPosition;

    private int triangle = -1;
    
    // Collision between balls
    private List<RollingBall> ballsInScene = new List<RollingBall>();
    
    // Drawing the path the ball has taken
    [SerializeField][Range(2, 50)]private int splineRes = 10;
    private List<Vector3> bSplinePoints = new List<Vector3>();

    void Awake() {
        previousPosition = transform.position;
        newPosition = transform.position;
        TIME = 0.0f;
        transform.localScale = new Vector3(radius * 2.00f, radius * 2.00f, radius * 2.00f);
        
        //VELOCITY = Vector3.ProjectOnPlane(VELOCITY, planeNormal);
    }
    
    void FixedUpdate() {
        TIME += Time.deltaTime;

        if (SurfaceCollision())
        {
            Vector3 surfaceNormal = triangleSurface.normalVector;
            Vector3 normalForce = -Vector3.Dot(gravity, surfaceNormal) * surfaceNormal;
            Vector3 force = gravity + normalForce;
            acceleration = force;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, surfaceNormal);
            SetSplineControlPoint();
            
            if (triangleSurface.enteredTriangle)
            {
                triangleSurface.enteredTriangle = false;
                normalForce = (surfaceNormal + surfaceNormal).normalized;
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalForce);
                //print("TIME: " + TIME);
                //print("NORMAL: " + triangleSurface.normalVector);
                //print("ACCELERATION: " + acceleration + "  " + acceleration.magnitude);
                //print("NORMAL: " + reflectionNormal);
                //print("VELOCITY: " + newVelocity + "  " + newVelocity.magnitude);
                //print("POSITION: " + newPosition);
                TIME = 0;
            }
        }
        else {
            // Constantly applies gravity
            acceleration = gravity * mass;
        }
        
        Debug.DrawRay(transform.position, acceleration, Color.green);
        newVelocity = currentVelocity + acceleration * Time.fixedDeltaTime;
        currentVelocity = newVelocity;

        Debug.DrawRay(transform.position, newVelocity, Color.blue);

        newPosition = transform.position + newVelocity * Time.fixedDeltaTime;
   
        transform.position = newPosition;
    }

    private bool SurfaceCollision() {
        // Using (k = C + ((S - C) . n) * n when |(S - C) . n| <= r) to calculate the collision point
        Vector3 pos = transform.position; // C
        Vector3 baryc = triangleSurface.baryc(new Vector2(pos.x, pos.z)); // S
        Vector3 normalVec = triangleSurface.normalVector; // n

        float dotProduct = Vector3.Dot(baryc - pos, normalVec);
        
        if (Mathf.Abs(dotProduct) <= radius) {
            barycY = baryc.y;
            Vector3 collisionPos = pos + dotProduct * normalVec;
            print("CBT");
            return true;
        }
        return false;
    }
    
    public void BallCollision(RollingBall otherBall) {
        Vector3 direction = transform.position - otherBall.transform.position;

        Vector3 impulse = new Vector3();
        
        newVelocity = newVelocity + impulse;
        otherBall.newVelocity = otherBall.newVelocity - impulse;
    }
    
    void SetSplineControlPoint() {
        bSplinePoints.Add(transform.position);
    }

    void DrawSpline()
    {
        for (float x = 0; x < bSplinePoints.Count; x += 0.005f) {
            
            
        }
    }
}