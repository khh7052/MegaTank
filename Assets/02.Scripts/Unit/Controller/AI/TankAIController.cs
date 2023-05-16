using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TankAIController : AIController
{
    #region 멤버 변수

    public Tank myTank;
    private float turretRot = 1;
    private bool attackOn = false;

    #endregion

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
        if (agent.isStopped) volume = 0;
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
            if (angle < -5.0F) turretRot = 1;
            else if (angle > 5.0F) turretRot = -1;
            else turretRot = 0;
            myTank.TankMove(0, turretRot);

            direction = myTank.turret.transform.forward;
            angle = Vector3.SignedAngle(targetDir, direction, Vector3.up);
            attackOn = -10.0F <= angle && angle <= 10.0F;
            
            if (angle < -1.0F) turretRot = 1;
            else if (angle > 1.0F) turretRot = -1;
            else turretRot = 0;
            myTank.TurretMove(turretRot, 0);
        }
    }
}
