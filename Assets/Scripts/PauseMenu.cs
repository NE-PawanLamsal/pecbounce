using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
   public static bool Paused = false;

   [SerializeField] GameObject pauseMenuUI;

    void Start()
    {
       pauseMenuUI.SetActive(false);
    }
	
	public void Pause()
	{
		pauseMenuUI.SetActive(true);
		Time.timeScale = 0f;
		Paused = true;
	}
	
	public void Resume()
	{
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		Paused = false;
	}
	
	public void LoadMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("Start Screen");
	}
	public void Quit()
	{
		Application.Quit();
	}
}
