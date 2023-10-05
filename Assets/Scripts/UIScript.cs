using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour
{
    private float currentTime;
    private bool startTimer = false;

    [SerializeField] TMP_Text timerText;
    
    private int score;
    [SerializeField] TMP_Text scoreText;
    
    // Start is called before the first frame update
    void Start() {
        score = 0;
        currentTime = 0.0f;
        timerText.text = "Time: " + currentTime.ToString("f2");
        scoreText.text = "Score: " + score.ToString("f0");
        startTimer = true;
    }

    // Update is called once per frame
    void Update() {
        if (startTimer) {
            currentTime += Time.deltaTime;
            timerText.text = "Time: " + currentTime.ToString("f2");
        }
    }

    public void UpdateScore(int add) {
        score += add;
        scoreText.text = "Score: " + score.ToString("f0");
    }
}
