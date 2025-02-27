using UnityEngine;
using UnityEngine.SceneManagement;
public class DefaultMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject activeMenu;
    
    
    public void PlayGame ()
    {
        SceneManager.LoadScene(0);
    }

    public void BackMenu()
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
            activeMenu.SetActive(false);
        }
    }
}
