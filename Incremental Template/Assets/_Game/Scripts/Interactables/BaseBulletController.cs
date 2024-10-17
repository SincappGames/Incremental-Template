using DG.Tweening;
using UnityEngine;

public class BaseBulletController : MonoBehaviour
{
    protected float BulletPower { get; set; } = 1;
    protected bool IsDead;
    
    public virtual void Shoot(float power,float range, Transform playerPos)
    {
        BulletPower = power;
        transform.DOMoveZ(playerPos.position.z + range, 20).SetSpeedBased().SetEase(Ease.Linear)
            .OnComplete(DestroySelf);
    }
    
    protected virtual void DestroySelf()
    {
        IsDead = true;
        transform.DOKill();
        gameObject.layer = 0;
        Destroy(gameObject);
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsDead) return;

        var damagable = other.GetComponent<IDamageable>();

        if (damagable != null)
        {
            damagable.TakeBulletDamage(BulletPower, this);
            DestroySelf();
        }
    }
}