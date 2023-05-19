using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : RangedUnit
{
    #region ��� ����
    // �յ� �̵�
    private float move;

    // �¿� �̵�
    private float rotate;

    // �ͷ� ȸ��
    [Header("Tank")]
    public float turretRotSpeed = 4;
    public float turretWheelSpeed = 2;
    public GameObject turret;
    public float turretMinAngle = -10;
    public float turretMaxAngle = 20;

    // ���� ȸ��
    public GameObject gunBase;

    public AudioSource bodyAudio;

    public float FireTime
    {
        get { return RestAttackTime / attackRate; }
    }

    #endregion


    // �̵� ���� ������Ʈ
    public void VolumeUpdate(float v)
    {
        bodyAudio.volume = v;
    }

    public void TankMove(float vertical, float horizon)
    {
        // ���� ����
        move = moveSpeed * Time.deltaTime;

        // agent.destination = transform.position + moveSpeed * vertical * transform.forward;
        // agent.SetDestination(transform.position + move * vertical * transform.forward);
        agent.Move(move * vertical * transform.forward);
        // transform.Translate(move * vertical * Vector3.forward);

        // �¿� �̵�
        rotate = rotateSpeed * Time.deltaTime;
        if (vertical < 0) horizon *= -1;
        transform.RotateAround(transform.position + (horizon * 0.1f * Vector3.right), Vector3.up, rotate * horizon);
    }

    public void TurretMove(float x, float wheel)
    {
        rotate = turretRotSpeed * Time.deltaTime;
        // �ͷ� �¿� �̵�
        gunBase.transform.Rotate(x * rotate * Vector3.up);

        // ���� ���� ȸ��
        turret.transform.Rotate(wheel * turretWheelSpeed * Vector3.right);

        TurretClamp();
    }

    public void TurretClamp()
    {
        Vector3 ang = turret.transform.eulerAngles;
        if (ang.x > 180) ang.x -= 360;

        ang.x = Mathf.Clamp(ang.x, turretMinAngle, turretMaxAngle);
        turret.transform.eulerAngles = ang;
    }
    
}
