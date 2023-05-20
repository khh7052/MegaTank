using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TankAIController : AIController
{
    public Tank myTank;
    private float turretHorizonRot = 1;
    private float turretVerticalRot = 1;
    private bool attackOn = false;

    void Update()
    {
        if (GameManager.Instance.State != GameState.STARTBATTLE) return;
        TurretLookAtTarget();
        MoveVolumeUpdate();
    }

    public override void Attack()
    {
        if (!attackOn) return;
        base.Attack();
    }

    // 이동 볼륨 업데이트
    void MoveVolumeUpdate()
    {
        float volume;
        if (agent.isStopped)
        {
            volume = 0;
            print(gameObject.name);
        }
        else volume = agent.velocity.magnitude / agent.speed;
        // print(agent.velocity.magnitude + " " + agent.speed);
        
        myTank.VolumeUpdate(volume);
    }


    void TurretLookAtTarget()
    {
        if(state == AIState.CHASE || state == AIState.ATTACK || state == AIState.RETREAT)
        {
            Vector3 direction = myTank.transform.forward;
            Vector3 targetDir = target.transform.position - transform.position;
            float angle = Vector3.SignedAngle(targetDir, direction, Vector3.up);
            if (angle < -5.0F) turretHorizonRot = 1;
            else if (angle > 5.0F) turretHorizonRot = -1;
            else turretHorizonRot = 0;
            myTank.TankMove(0, turretHorizonRot);

            direction = myTank.turret.transform.forward;
            angle = Vector3.SignedAngle(targetDir, direction, Vector3.up);
            attackOn = -10.0F <= angle && angle <= 10.0F;
            
            if (angle < -1.0F) turretHorizonRot = 1;
            else if (angle > 1.0F) turretHorizonRot = -1;
            else turretHorizonRot = 0;

            targetDir = targetDir.normalized;
            if (targetDir.y > direction.y) turretVerticalRot = -1;
            else if (targetDir.y < direction.y) turretVerticalRot = 1;
            else turretVerticalRot = 0;

            myTank.TurretMove(turretHorizonRot, turretVerticalRot);
            
        }
    }
}
