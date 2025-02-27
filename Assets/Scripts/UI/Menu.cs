using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;
     
    public void PlayGame ()
    {
        SceneManager.LoadScene(0);
    }
   
    public void ControlMenu()
    {
        if (controlsMenu != null)
        {
            controlsMenu.SetActive(true);
            mainMenu.SetActive(false);
               
        }
    }
    
}
