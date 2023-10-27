using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RainManager : MonoBehaviour {
    [SerializeField] GameObject rainDropPrefab;
    [SerializeField] Vector2 spawnArea = Vector2.one * 10;

    Vector3 GetRandomRainPosition() {
        float x = Random.Range(-spawnArea.x, spawnArea.x);
        float z = Random.Range(-spawnArea.y, spawnArea.y);

        Vector3 position = transform.position;
        position.x += x;
        position.z += z;
        return position;
    }

    void spawnRain() {
        GameObject rainDropGameObject = Instantiate(rainDropPrefab, GetRandomRainPosition(), Quaternion.identity);
        Destroy(rainDropGameObject, 60f);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Vector3 gizmoSize = new Vector3(spawnArea.x, 1, spawnArea.y);
        Gizmos.DrawWireCube(transform.position, gizmoSize * 2);
    }

    void Start() {
        InvokeRepeating("spawnRain", 0.0f, 0.2f);
        spawnRain();
    }

    // Update is called once per frame
    void FixedUpdate() {
        //float time = Time.deltaTime;
    }
}
