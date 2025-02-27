using UnityEngine;
using UnityEngine.SceneManagement;
public class ControlMenuInGame : MonoBehaviour
{
    
    public void PlayGame ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void BackMenu()
    {
       Time.timeScale = 1f;
            SceneManager.LoadScene(0);
    }
}

