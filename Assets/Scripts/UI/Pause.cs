using UnityEngine;
using System.Collections.Generic;
public class Pause : MonoBehaviour
{
    
    public GameObject Game;
    public GameObject pauseMenuUI;
    private bool isGamePaused = false;


    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }
    public void TogglePause()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            PauseGame();

        }
        else
        {
            ResumeGame();
            Game.SetActive(true);
        }
    }

    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> activePowerUps = new List<GameObject>();
    private List<GameObject> activeAx = new List<GameObject>();
    private List<GameObject> activeBigAx = new List<GameObject>();
    private void PauseGame()
    {
        if (pauseMenuUI == null)
        {
            return;
        }

        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
        Game.SetActive(false);
        
    }

    private void ResumeGame()
    {
        if (pauseMenuUI == null)
        {
            return;
        }

        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
       
        Game.SetActive(true);
        
    }
}
