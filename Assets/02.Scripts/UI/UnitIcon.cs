using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitIcon : MonoBehaviour
{
    public Unit unit;

    public void PassInfomation()
    {
        BuildingManager.Instance.StartSpawn(unit);
        if (BuildingManager.Instance.selectUnitObject == null) return;
        Unit selectUnit = BuildingManager.Instance.selectUnitObject.GetComponent<Unit>();
        if(selectUnit != null) UIManager.Instance.InfomationUIUpdate(selectUnit);
    }
}
