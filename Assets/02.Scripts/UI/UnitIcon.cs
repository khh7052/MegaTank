using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    public Texture unitImage;
    public Unit unit;

    public void PassInfomation()
    {
        UIManager.Instance.InfomationUIUpdate(unit);
        BuildingManager.Instance.StartSpawn(unit);
    }
}
