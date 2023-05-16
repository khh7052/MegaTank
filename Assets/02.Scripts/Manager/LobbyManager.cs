using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{

    public void OnLoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnExitBtn()
    {
        Application.Quit();
    }

    public void OnOptionBtn()
    {

    }
}
