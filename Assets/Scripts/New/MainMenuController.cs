using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    #region Buttons
    public void HowToPlayGame()
    {
        Debug.Log("Starting Tutorial");
        SceneManager.LoadScene("Tutorial");
    }

    public void StartSinglePlayerGame()
    {
        Debug.Log("Starting Singleplayer Game");
    }

    public void StartMultiPlayerLocalGame()
    {
        Debug.Log("Starting Multiplayer Local Game");
        SceneManager.LoadScene("RoyalGameOfUr");
    }

    public void StartMultiPlayerRemoteGame()
    {
        Debug.Log("Starting Multiplayer Remote Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}
