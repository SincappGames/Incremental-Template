using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] private Transform _shootPosTransform;
    [SerializeField] private BulletController _bullet;
    private float _range;
    private float _fireRate;
    private float _collectedGateFireRateAmount;
    private float _collectedGateRangeAmount;

    public void StartShooting()
    {
        _range = PersistData.Instance.Range;
        _fireRate = PersistData.Instance.FireRate;
        StartCoroutine(StartShoot());
    }

    public void CalculateSkills(GateGroupController.SkillTypes skillType, float skillAmount)
    {
        if (skillType == GateGroupController.SkillTypes.Range)
        {
            _collectedGateRangeAmount += skillAmount;

            var a = Mathf.InverseLerp(0, RemoteController.Instance.RangeGateCollectLerpMax,
                _collectedGateRangeAmount);
            _range = Mathf.Lerp(_range, 25, a);
        }
        else if (skillType == GateGroupController.SkillTypes.FireRate)
        {
            _collectedGateFireRateAmount += skillAmount;

            var a = Mathf.InverseLerp(0, RemoteController.Instance.FireRateGateCollectLerpMax,
                _collectedGateFireRateAmount);
            _fireRate = Mathf.Lerp(_fireRate, .15f, a);
        }
    }

    private IEnumerator StartShoot()
    {
        while (PlayerController.PlayerState == PlayerController.PlayerStates.Run
               || PlayerController.PlayerState == PlayerController.PlayerStates.OnFinishWall)
        {
            InstantiateBullet().Shoot(1,_range, transform);
            yield return new WaitForSeconds(_fireRate);
        }
    }

    private BulletController InstantiateBullet()
    {
        var bullet = Instantiate(_bullet, _shootPosTransform.position, Quaternion.identity);
        return bullet;
    }
}