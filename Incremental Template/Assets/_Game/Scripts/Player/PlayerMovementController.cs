using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [InlineEditor(Expanded = true)] [SerializeField] private PlayerSetting _playerSettings;
    private SwerveInputSystem _swerveInputSystem;
    public float SpeedMultiplier = 1;
    private float _speed;
    private Tween _speedTween;

    private void Awake()
    {
        _swerveInputSystem = FindObjectOfType<SwerveInputSystem>();
        _speed = _playerSettings.MovementSpeed;
    }

    private void Update()
    {
        if (PlayerController.PlayerState == PlayerController.PlayerStates.Run || PlayerController.PlayerState == PlayerController.PlayerStates.OnFinishWall)
        {
            SwerveMovement();
            ClampPosition();
        }
    }

    private void SwerveMovement()
    {
        float swerveAmount = Time.deltaTime * _playerSettings.SwerveSpeed * _swerveInputSystem.MoveFactorX;
        transform.Translate(swerveAmount, 0, (_speed * Time.deltaTime * SpeedMultiplier), Space.World);
    }

    private void ClampPosition()
    {
        Vector3 transformPosition = transform.position;
        transformPosition.x = Mathf.Clamp(transformPosition.x, -_playerSettings.MaxSwerveAmount,
            _playerSettings.MaxSwerveAmount);
        transform.position = transformPosition;
    }

    public void SetSpeedBoost(float speedBoost) => SpeedMultiplier = speedBoost;

    public void BounceBackGate()
    {
        var tempSpeed = _playerSettings.MovementSpeed;
        _speed *= -2;
        _speedTween.Kill();

        _speedTween = DOTween.To(() => _speed, x => _speed = x, 1, 1f).OnComplete(() =>
        {
            _speedTween = DOTween.To(() => _speed, x => _speed = x, tempSpeed, 1f).SetEase(Ease.Linear);
        }).SetEase(Ease.Linear);
    }
}