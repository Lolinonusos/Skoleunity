using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {
    public int pointsAwarded = 10;

    [SerializeField] private AnimationCurve scaleCurve;
    private float timer = 1.00f;
    private Vector3 scale;
    private MeshFilter mesh;
    
    
    [SerializeField][Range(0.0f, 1000.0f)] float bumpForce = 500f;
    
    [SerializeField] UIScript UI;

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == "DaBall")
        {
            Vector3 bounceDirection = collision.GetContact(0).normal;
            collision.gameObject.GetComponent<Rigidbody>().AddForce(bounceDirection * bumpForce, ForceMode.Impulse);
            timer = 0.00f;
            UI.UpdateScore(pointsAwarded);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>();
        scale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mesh.transform.localScale = new Vector3(scale.x * scaleCurve.Evaluate(timer), scale.y, scale.z * scaleCurve.Evaluate(timer));
        timer += Time.deltaTime;

    }
}
