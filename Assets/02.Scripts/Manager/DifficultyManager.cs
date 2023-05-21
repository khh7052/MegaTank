using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    private int endingDay = 10;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetEndingDay(int day)
    {
        endingDay = day;
    }

    private void OnLevelWasLoaded(int level)
    {
        if(level == 1)
        {
            GameManager.Instance.endingDay = endingDay;
        }
    }

}
