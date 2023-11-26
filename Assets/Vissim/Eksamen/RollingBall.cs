using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBall : MonoBehaviour {

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


    void Awake() {
        previousPosition = transform.position;
        newPosition = transform.position;
        TIME = 0.0f;
        transform.localScale = new Vector3(radius * 2.00f, radius * 2.00f, radius * 2.00f);
        
        //VELOCITY = Vector3.ProjectOnPlane(VELOCITY, planeNormal);
    }
    
    void FixedUpdate() {
        TIME += Time.deltaTime;
        Vector3 force = new Vector3();

        if (startBspline && TIME >= splineTimeInterval && controlPointsPlaced <= intervals) {
            TIME = 0;
            controlPointsPlaced += 1;
            //controlpoints.Add(new Vector2(transform.position.x, transform.position.z));
            //bSpline.
        }
        
        if (SurfaceCollision()) {
            startBspline = true;
            Vector3 surfaceNormal = triangleSurface.normalVector;
            Vector3 normalForce = -Vector3.Dot(gravity, surfaceNormal) * surfaceNormal;
            force = gravity + normalForce;
            acceleration = force;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, surfaceNormal);

            // Rolling over trangle edge
            if (triangleSurface.enteredTriangle) {
                triangleSurface.enteredTriangle = false;
                normalForce = (surfaceNormal + surfaceNormal).normalized;
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalForce);
                //print("TIME: " + TIME);
                //print("NORMAL: " + triangleSurface.normalVector);
                //print("ACCELERATION: " + acceleration + "  " + acceleration.magnitude);
                //print("NORMAL: " + reflectionNormal);
                //print("VELOCITY: " + newVelocity + "  " + newVelocity.magnitude);
                //print("POSITION: " + newPosition);
                //TIME = 0;
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
        Vector3 baryc = triangleSurface.baryc(new Vector2(pos.x, pos.z)); // S
        Vector3 normalVec = triangleSurface.normalVector; // n

        float dotProduct = Vector3.Dot(baryc - pos, normalVec);
        
        if (Mathf.Abs(dotProduct) <= radius) {
            barycY = baryc.y;
            Vector3 collisionPos = pos + dotProduct * normalVec;
            return true;
        }
        return false;
    }
    
    public void BallCollision(RollingBall otherBall, float distance) {
        //print("Ball Collisioncheck works");
        
        Vector3 direction = transform.position - otherBall.transform.position;

        Vector3 normal = direction / distance;

        Vector3 minTransDist;
        if (true)
        {
            minTransDist = direction;
        }
        
        Vector3 vel = newVelocity - otherBall.newVelocity;
        

        float impactSpeed = Vector3.Dot(vel, minTransDist);
        
        Vector3 impulse = normal;
        
        newVelocity = newVelocity + impulse;
        otherBall.newVelocity = otherBall.newVelocity - impulse;
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
