using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour {

    [SerializeField] private BallManager manager;
    // For physics allowing the ball to collide with a triangle surface
    [SerializeField] public TriangleSurfaceV2 triangleSurface;
    //[SerializeField][Range(0.0f, 10.0f)] private float bouncyness = 1.0f;
    [SerializeField]public float radius = 3f;
    Vector3 gravity = Physics.gravity;
    public Vector3 currentVelocity = new();
    public Vector3 newVelocity = Vector3.zero;
    Vector3 acceleration = Vector3.zero;
    float mass = 2.0f;
    float newton;
    private float barycY;
    Vector3 previousPosition;
    Vector3 newPosition;

    private int triangle = -1;

    Vector3 torque; // Position (cross) Force

    // Collision between balls
    private List<RollingBall> ballsInScene = new List<RollingBall>();
    
    // Drawing the path the ball has taken

    bSpline bSpline;
    double TIME;
    bool startBspline = false;
    [SerializeField] float splineTimeInterval = 1.0f;
    [SerializeField][Min(5)] int intervals = 15;
    int controlPointsPlaced = 0;
    [SerializeField][Range(1, 200)] private int splineRes = 10;
    //private List<Vector3> bSplinePoints = new List<Vector3>();
    bool bSplineConstructed = false;

    List<Vector2> controlpoints;
    List<float> knotVector;

    [NonSerialized]public bool isOutofBounds = false; 

    void Awake() {
        previousPosition = transform.position;
        newPosition = transform.position;
        TIME = 0.0f;
        transform.localScale = new Vector3(radius * 2.00f, radius * 2.00f, radius * 2.00f);

        //VELOCITY = Vector3.ProjectOnPlane(VELOCITY, planeNormal);
    }

    void Start() {
        if (triangleSurface == null) {
            Debug.Log("Surface reference missing");
            if (manager != null) {
                manager.RemoveBall(this);
            }

            Destroy(this);
        }
    }

    public void SetManager(BallManager newManager) {
        manager = newManager;
    }
    
    void FixedUpdate() {
        //TIME += Time.deltaTime;
        if (isOutofBounds) {
            manager.RemoveBall(this);
            Destroy(this);
        }
        
        Vector3 force = new Vector3();

        // if (startBspline && TIME >= splineTimeInterval && controlPointsPlaced <= intervals) {
        //     TIME = 0;
        //     controlPointsPlaced += 1;
        //     //controlpoints.Add(new Vector2(transform.position.x, transform.position.z));
        //     //bSpline.
        // }
        
        if (SurfaceCollision()) {
            startBspline = true;
            Vector3 surfaceNormal = triangleSurface.normalVector;
            Vector3 normalForce = -Vector3.Dot(gravity, surfaceNormal) * surfaceNormal;
            force = gravity + normalForce;
            acceleration = force;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, surfaceNormal);
            //print("Collision");
            
            // Rolling over trangle edge
            if (triangleSurface.enteredTriangle) {
                triangleSurface.enteredTriangle = false;
                normalForce = (surfaceNormal + surfaceNormal).normalized;
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalForce);
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
        torque = Vector3.Cross(transform.position, force);
    }

    private bool SurfaceCollision() {
        // Using (k = C + ((S - C) . n) * n when |(S - C) . n| <= r) to calculate the collision point
        Vector3 pos = transform.position; // C
        //Vector3 baryc = triangleSurface.baryc(new Vector2(pos.x, pos.z)); // S
        Vector3 hitPos = triangleSurface.SurfaceCollision(pos, this); // S
        Vector3 normalVec = triangleSurface.normalVector; // n

        Vector3 distVec = pos - hitPos;
        float dist = distVec.magnitude;
        //print("distvec: " + hitPos + "  ballposition: " + pos);
        
        float dotProduct = Vector3.Dot(hitPos - pos, normalVec);
        // (Mathf.Abs(dotProduct)
        if ((Mathf.Abs(dotProduct)) <= radius) {
            //print("true");
            return true;
        }
        //print("false");
        return false;
    }
    
    public void BallCollision(RollingBall otherBall) {
        //print("Ball Collisioncheck works");
        Vector3 thisPos = transform.position;
        Vector3 otherPos = otherBall.transform.position;
        
        // Distance between the balls
        float distance = Vector3.Distance(thisPos, otherPos);
        
        // Check if balls are overlapping, exit if they do not
        if (distance >= radius + otherBall.radius) {
            return;
        }

        float overlap = 0.5f * (distance - radius - otherBall.radius);

        // https://www.youtube.com/watch?v=LPzyNOHY3A4
        // Displace balls
        thisPos -= overlap * (thisPos - otherPos) / 2;
        otherPos -= overlap * (otherPos - thisPos) / 2;

        transform.position = thisPos;
        otherBall.transform.position = otherPos;
        
        // https://github.com/NesquikPlus/opengl_collision/blob/master/Game.cpp
        // Normal
        Vector3 collisionNormal = (otherPos - thisPos);
        collisionNormal = Vector3.Normalize(collisionNormal);
        
        Vector3 proj1 = Vector3.Project(newVelocity, collisionNormal);
        Vector3 proj2 = Vector3.Project(otherBall.newVelocity, collisionNormal);

        float v1n = -1.0f * Vector3.Magnitude(proj1);
        float v2n = Vector3.Magnitude(proj1);
        
        float v1n2 = (v1n * (mass - otherBall.mass) + 2 * (otherBall.mass) * v2n) / (mass + otherBall.mass);
        float v2n2 = (v2n * (otherBall.mass - mass) + 2 * (mass) * v1n) / (mass + otherBall.mass);

        // Vector3 direction1 = Vector3.Normalize(-(currentVelocity - proj1));
        // Vector3 direction2 = Vector3.Normalize(-(otherBall.currentVelocity - proj2));

        //newVelocity = (newVelocity - proj1) + (v1n2 * collisionNormal);
        //otherBall.newVelocity = (newVelocity - proj2) + (v2n2 * collisionNormal);

        newVelocity = (newVelocity - 2 * collisionNormal);
        otherBall.newVelocity = (newVelocity - 2 * collisionNormal);

        //newVelocity = newVelocity + impulse;
        //otherBall.newVelocity = otherBall.newVelocity - impulse;
    }
    
    // void SetSplineControlPoint() {
    //     bSplinePoints.Add(transform.position);
    // }
    //
    // void DrawSpline()
    // {
    //     for (float x = 0; x < bSplinePoints.Count; x += 0.005f) {
    //         
    //         
    //     }
    // }
}
