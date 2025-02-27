using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class DefaultMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject activeMenu;
    
    
    public void PlayGame ()
    {
        SceneManager.LoadScene(1);
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
