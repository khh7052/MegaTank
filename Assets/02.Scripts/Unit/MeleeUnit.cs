using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    public void MeleeAttack()
    {
        Target.TakeDamage(attackDamage, this);
    }

}
