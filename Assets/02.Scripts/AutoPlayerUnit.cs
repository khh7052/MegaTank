using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayerUnit : MonoBehaviour
{
    public Transform camTarget;
    private Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    private void OnEnable()
    {
        GameManager.Instance.playerUnit = unit;
        FollowCam.Instance.battleTr = camTarget;
    }
}
