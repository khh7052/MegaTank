using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : Unit
{
    [Header("Attack")]
    public GameObject bullet;
    public float bulletPower = 1000;
    public GameObject muzzle;
    public Transform spPoint;

    public virtual void MakeBullet()
    {
        PoolManager.Instance.Pop(muzzle, spPoint.position, spPoint.rotation); // muzzle
        Bullet b = PoolManager.Instance.Pop(bullet, spPoint.position, spPoint.rotation).GetComponent<Bullet>();

        b.owner = this;
        b.damage = attackDamage;
        b.power = bulletPower;

        Rigidbody rb = b.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(spPoint.forward * bulletPower, ForceMode.Impulse);
        if (audioSoruce.isPlaying) audioSoruce.Stop();
        audioSoruce.Play();
        //audioSoruce.PlayOneShot(attackSound);
    }
}
