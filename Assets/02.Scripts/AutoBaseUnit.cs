using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBaseUnit : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.baseUnit = GetComponent<Unit>();
    }
}
