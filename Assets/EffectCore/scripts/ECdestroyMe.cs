using UnityEngine;
using System.Collections;

public class ECdestroyMe : MonoBehaviour{

    float timer;
    public float deathtimer = 10;

	void Update ()
    {
        timer += Time.deltaTime;

        if(timer >= deathtimer)
        {
            Destroy(gameObject);
        }
	}
}
