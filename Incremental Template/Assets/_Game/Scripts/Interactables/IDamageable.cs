using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeBulletDamage(float damageAmount, BaseBulletController bullet);
}