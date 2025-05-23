using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance { 
        get
        {
            if(instance == null) instance = FindObjectOfType<T>();
            if(instance == null) instance = new GameObject().AddComponent<T>();
            /*
            else
            {
                if(instance.transform.parent != null) DontDestroyOnLoad(instance.transform.parent);
                else DontDestroyOnLoad(instance.gameObject);
            }
            */
            return instance;
        }
        set { instance = value; }
    }

}
