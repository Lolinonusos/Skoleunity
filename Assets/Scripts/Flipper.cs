using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : MonoBehaviour {
    [SerializeField] AnimationCurve animationCurve;
    float _timer = 1f;
    
    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
			_timer = 0f;
        }

        _timer += Time.deltaTime;
	    float angleRot = animationCurve.Evaluate(_timer) * 45f;
	    Vector3 localRotation = new Vector3(0, 0, angleRot);
        transform.localEulerAngles = localRotation;
    }
}
