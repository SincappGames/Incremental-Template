using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : BaseInteractableController
{
    public override void TakeBulletDamage(float damageAmount, BaseBulletController bullet)
    {
        base.TakeBulletDamage(damageAmount, bullet);
    }

    public override void InteractPlayer(Transform playerTransform)
    {
        base.InteractPlayer(playerTransform);
    }
}