using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lofelt.NiceVibrations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EndGameObstacleController : BaseInteractableController
{
    public float EndGameObstacleNumber;
    [SerializeField] private Transform _moneyTransform;
    [SerializeField] private GameObject _endGameObstacle;
    private TextMeshPro _numberText;
    private float _startScaleX;

    private void Start()
    {
        _numberText = GetComponentInChildren<TextMeshPro>();
        _numberText.SetText($"{EndGameObstacleNumber}");
        _startScaleX = transform.localScale.x;
    }

    public override void TakeBulletDamage(float damageAmount, BulletController bullet)
    {
        base.TakeBulletDamage(damageAmount, bullet);
        
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
        transform.DOKill();
        transform.DOScale(_startScaleX + .05f, .07f).OnComplete(() => { transform.DOScale(_startScaleX, .07f); });
        EndGameObstacleNumber -= damageAmount;

        if (EndGameObstacleNumber <= 0)
        {
            gameObject.layer = 0;
            _numberText.gameObject.SetActive(false);
            _endGameObstacle.transform.DOKill();
            _moneyTransform.DOLocalMoveY(transform.position.y + 0.68f, .25f).SetEase(Ease.Linear);
            Destroy(_endGameObstacle);
        }

        _numberText.SetText($"{Mathf.Ceil(EndGameObstacleNumber)}");
    }

    public override void InteractPlayer(Transform playerTransform)
    {
        if (_isHit) return;

        base.InteractPlayer(playerTransform);
        
        var playerController = playerTransform.GetComponent<PlayerController>();
        playerController.Die(0);
        Debug.Log("v");
    }
}