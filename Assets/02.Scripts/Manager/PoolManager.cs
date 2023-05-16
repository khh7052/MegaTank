using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public Dictionary<GameObject, List<GameObject>> poolList = new();

    // 추가
    public GameObject Push(GameObject pool)
    {
        return Push(pool, pool.transform.position, pool.transform.rotation);
    }

    public GameObject Push(GameObject pool, Vector3 pos, Quaternion rot)
    {
        // 존재안하면 Dictionary 추가
        if (!poolList.ContainsKey(pool))
            poolList.Add(pool, new List<GameObject>());

        GameObject g = Instantiate(pool, pos, rot);
        g.SetActive(true);
        poolList[pool].Add(g);

        return g;
    }

    // 활성화
    public GameObject Pop(GameObject pool)
    {
        return Pop(pool, pool.transform.position, pool.transform.rotation);
    }

    public GameObject Pop(GameObject pool, Vector3 pos, Quaternion rot)
    {
        // dictionary 존재안하면 새로 추가
        if (!poolList.ContainsKey(pool)) return Push(pool, pos, rot);

        // active 비활성화된게 있으면 활성화하고 종료
        foreach (GameObject g in poolList[pool])
        {
            if (g.activeInHierarchy == false)
            {
                g.transform.SetPositionAndRotation(pos, rot);
                g.SetActive(true);
                return g;
            }
        }

        // 만약 전부다 활성화 상태면 새로 추가
        return Push(pool, pos, rot);
    }

}
