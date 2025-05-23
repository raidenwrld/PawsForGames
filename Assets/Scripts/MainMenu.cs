using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
        public GameObject mainMenu;
    public GameObject optionsMenu;
   public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }


    public void OpenOptionsMenu()
    {
        mainMenu.SetActive(false); // disable main menu
        optionsMenu.SetActive(true); // show options menu
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false); // hide options menu
        mainMenu.SetActive(true); // re-enable main menu
    }


    public void QuitGame()
    {
        Application.Quit();
    }

}
