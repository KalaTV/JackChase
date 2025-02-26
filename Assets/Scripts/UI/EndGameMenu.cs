using UnityEngine;
using UnityEngine.SceneManagement;
public class EndGameMenu : MonoBehaviour
{
  
    
    public void PlayGame ()
    {
        SceneManager.LoadScene(1);
    }

    public void BackMenu()
    {
        SceneManager.LoadScene(0);
    }
}

