using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	public void StartPinball()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

	}

	public void StartBallSim()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
		
	}

	public void CloseApp()
	{
		Application.Quit();
	}

}
