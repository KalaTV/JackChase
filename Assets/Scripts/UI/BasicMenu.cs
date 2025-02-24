using UnityEngine;
using UnityEngine.SceneManagement;
public class BasicMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject activeMenu;
    
    
    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

