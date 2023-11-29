using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] public TriangleSurfaceV2 triangleSurface;

    private List<RollingBall> balls = new List<RollingBall>();

    [SerializeField] [Range(0.5f, 50.0f)] private float size = 10;

    private float timer = 0;
    
    void Start()
    {
        SpawnBall();
        SpawnBall();
        SpawnBall();
    }
    
    void FixedUpdate() {
        // Check for collisions between balls
        for (int i = 0; i < balls.Count; i++) {
            for (int j = i + 1; j < balls.Count; j++) {
                balls[i].BallCollision(balls[j]);
            }
        }

        if (Input.GetKey(KeyCode.Q)) {
            timer += Time.deltaTime;
            
        }
        
        
        if (timer > 1) {
            SpawnBall();
            timer = 0.0f;
        }
        
    }

    void SpawnBall()
    {
        GameObject ballGameObject = Instantiate(ballPrefab, GetRandomBallPosition(), Quaternion.identity);
        ballGameObject.GetComponent<RollingBall>().triangleSurface = triangleSurface;
        balls.Add(ballGameObject.GetComponent<RollingBall>());
        balls[balls.Count - 1].SetManager(this);
    }

    public void RemoveBall(RollingBall ballToRemove) {
        balls.Remove(ballToRemove);
    }
    
    Vector3 GetRandomBallPosition() {
        float random = Random.Range(-size, size);
        Vector3 position = transform.position;
        position.x += random;
        position.z += random;
        return position;
    }
    
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 gizmoSize = new Vector3(size, 1, size);
        Gizmos.DrawWireCube(transform.position, gizmoSize * 2);
    }
}
