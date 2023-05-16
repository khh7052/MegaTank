using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : InitSystem
{
    [HideInInspector] public Unit owner;

    public override void ComponentInit()
    {
        owner = GetComponent<Unit>();
    }
}
