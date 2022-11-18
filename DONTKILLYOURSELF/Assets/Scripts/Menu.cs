using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private bool paused;
    [SerializeField] GameObject pauseMenu;

    void Awake()
    {
        paused = false;
    }

    void Update()
    {
        if(pauseMenu != null && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
            Pause();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        GlobalVars.setScore(0);
        GlobalVars.setTime(0);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        if(paused == true && pauseMenu != null)
        {
            paused = false;
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }

    public void Pause()
    {
        if(pauseMenu != null)
        {
            if(paused == false)
            {
                paused = true;
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
            }
            else
            {
                paused = false;
                Time.timeScale = 1f;
                pauseMenu.SetActive(false);
            }
        }
    }
}
