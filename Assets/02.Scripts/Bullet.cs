using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : InitSystem
{
    public Unit owner;
    public Rigidbody rb;
    public GameObject impact;
    public float power = 600;
    public int damage = 10;

    private void CreateImpact(Vector3 pos, Quaternion rot)
    {
        PoolManager.Instance.Pop(impact, pos, rot);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Unit unit = collision.collider.GetComponent<Unit>();
        if(unit != null) unit.TakeDamage(damage, owner);

        CreateImpact(collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        gameObject.SetActive(false);
    }
}
