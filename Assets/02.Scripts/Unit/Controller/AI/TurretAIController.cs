using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretAIController : AIController
{
    private float turretRot = 1;
    private bool attackOn = false;
    private RangedUnit rangedUnit;
    void Update()
    {
        TurretLookAtTarget();
    }

    public override void ComponentInit()
    {
        base.ComponentInit();
        rangedUnit = owner as RangedUnit;
    }

    public override void Attack()
    {
        if (!attackOn) return;
        base.Attack();
    }

    public override void TargetUpdate()
    {
        if (target)
        {
            if (TargetInAttackDistance && IsVisible(transform.position, target.transform.position)) return;
        }

        // 시야범위 내의 유닛들을 불러옴
        Collider[] chaseColliders = Physics.OverlapSphere(transform.position, owner.attackDistance, GameManager.Instance.unitLayerMask);

        if (chaseColliders.Length > 0)
        {
            foreach (var coll in chaseColliders)
            {
                Unit unit = coll.GetComponentInParent<Unit>();
                if (unit == null) continue;
                if (unit.team == owner.team) continue; // 같은팀이면 넘김

                isTargetVisible = IsVisible(transform.position, coll.transform.position);

                // 시야에 안가리면 타겟으로 설정하고 종료
                if (isTargetVisible)
                {
                    Target = unit;
                    return;
                }
            }
        }
        
        chaseColliders = Physics.OverlapSphere(transform.position, owner.lookDistance, GameManager.Instance.unitLayerMask);

        if (chaseColliders.Length > 0)
        {
            foreach (var coll in chaseColliders)
            {
                Unit unit = coll.GetComponentInParent<Unit>();
                if (unit == null) continue;
                if (unit.team == owner.team) continue; // 같은팀이면 넘김

                isTargetVisible = IsVisible(transform.position, coll.transform.position);

                // 시야에 안가리면 타겟으로 설정하고 종료
                if (isTargetVisible)
                {
                    Target = unit;
                    return;
                }
            }
        }

        if (target) isTargetVisible = IsVisible(transform.position, target.transform.position);
    }

    void TurretLookAtTarget()
    {
        if (target == null) return;

        Vector3 direction = transform.forward;
        Vector3 targetDir = target.transform.position - transform.position;
        rangedUnit.spPoint.rotation = Quaternion.RotateTowards(rangedUnit.spPoint.rotation, Quaternion.LookRotation(targetDir), owner.rotateSpeed * Time.deltaTime);
        float angle = Vector3.SignedAngle(targetDir, direction, Vector3.up);
        attackOn = -10.0F <= angle && angle <= 10.0F;
        
        if (angle < -1.0F) turretRot = 1;
        else if (angle > 1.0F) turretRot = -1;
        else turretRot = 0;

        transform.Rotate(0, turretRot * owner.rotateSpeed * Time.deltaTime, 0);
    }
}
