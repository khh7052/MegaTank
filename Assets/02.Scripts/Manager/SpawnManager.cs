using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnData
{
    public List<Unit> units;
}

public class SpawnManager : Singleton<SpawnManager>
{
    public List<Unit> spawnUnitList;
    [Range(200f, 500f)]
    public float spawnStartRange = 500f;
    public float spawnEndRange = 600f;
    public float spawnRate = 100f;
    public int spawnIdx = 0;
    Vector3 spawnPoint;

    public List<Unit> GetSpawnData()
    {
        int money = GameManager.Instance.currentSpawnMoney;
        List<Unit> units = new();

        while(money > 0)
        {
            int rand = Random.Range(0, spawnUnitList.Count);
            Unit unit = spawnUnitList[rand];

            if (unit.spawnMoney > money)
            {
                unit = spawnUnitList[0];
            }


            money -= unit.spawnMoney;
            units.Add(unit);
        }

        return units;
    }

    public void Spawn(List<Unit> units)
    {
        GameManager.Instance.CountReset(units.Count);

        foreach (var unit in units)
        {
            SpawnPointUpdate();
            PoolManager.Instance.Pop(unit.gameObject, spawnPoint, Quaternion.identity);
        }
    }

    public void Spawn()
    {
        Spawn(GetSpawnData());
    }

    public void SpawnPointUpdate()
    {
        float rand = Random.Range(spawnStartRange, spawnEndRange);
        float angle = Random.Range(0, 360);

        spawnPoint.x = Mathf.Cos(angle) * rand;
        spawnPoint.z = Mathf.Sin(angle) * rand;
        spawnPoint.y = 100;
        if(Physics.Raycast(spawnPoint, Vector3.down, out RaycastHit hit, 1000f, GameManager.Instance.terrainLayerMask))
        {
            spawnPoint.y = hit.point.y;
        }
    }
}
