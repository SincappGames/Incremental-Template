using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseInteractableController : MonoBehaviour, IDamageable, IInteractable
{
    protected bool _isHit;

    public virtual void TakeBulletDamage(float damageAmount, BaseBulletController bullet)
    {
    }

    public virtual void InteractPlayer(Transform playerTransform)
    {
        gameObject.layer = 0;
        _isHit = true;
    }
}