using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuControl : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pauseMenu;
    public PlayerControl player;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    void Pause()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        player.enabled = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        player.enabled = true;
        Time.timeScale = 1;
    }
}
