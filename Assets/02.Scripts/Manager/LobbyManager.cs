using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : Singleton<LobbyManager>
{

    public void OnLoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void OnExitBtn()
    {
        Application.Quit();
    }

}
