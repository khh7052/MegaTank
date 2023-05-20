using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{

    public void OnLoadScene()
    {
        SceneManager.LoadScene(1);
    }

    public void OnExitBtn()
    {
        Application.Quit();
    }

    public void OnOptionBtn()
    {

    }
}
