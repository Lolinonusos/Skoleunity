using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    private List<RollingBall> balls = new List<RollingBall>();

    // Start is called before the first frame update
    void Start()
    {
        balls.Add(FindObjectOfType<RollingBall>());
    }
    
    void FixedUpdate() {
        // Check for collisions between balls
        for (int i = 0; i < balls.Count; i++) {
            for (int j = i + 1; j < balls.Count; j++) {
                float distance = Vector3.Distance(balls[i].transform.position, balls[j].transform.position); 
                if (distance <= balls[j].radius) {
                    balls[i].BallCollision(balls[j], distance);
                }    
            }
        }
    }

    void spawnBall()
    {
        
    }
}
