using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
     public GameObject mainMenu;
     public GameObject controlsMenu;
     public GameObject scoreMenu;
     
     public void PlayGame ()
       {
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
       }
   
      public void ControlMenu()
       {
           if (controlsMenu != null)
           {
               controlsMenu.SetActive(true);
               mainMenu.SetActive(false);
               
           }
       }
      
       public void ScoreMenu()
       {
           if (scoreMenu != null)
           {
               scoreMenu.SetActive(true);
               mainMenu.SetActive(false);
           }
       }
   }
