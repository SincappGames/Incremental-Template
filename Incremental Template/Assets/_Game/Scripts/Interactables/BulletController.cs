using System;
using UnityEngine;
using UnityEngine.Pool;

public class BulletController : BaseBulletController
{
    public override void Shoot(float power, float range, Transform playerPos)
    {
        base.Shoot(power, range, playerPos);
    }

    protected override void DestroySelf()
    {
        base.DestroySelf();
    }
    
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
}