﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SincappStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject _tapToStart;
    [SerializeField] private TextMeshProUGUI _playerMoney;
    [SerializeField] private Transform _collectableSprite;
    [SerializeField] private Transform _collectableTargetTransform;

    private Animator _animator;

    #region Incrementals

    [Header("Incrementals")] 
   
    [SerializeField] private GameObject _incrementalsObj;
    [SerializeField] private Button _ageButton;
    [SerializeField] private Button _incomeButton;
    [SerializeField] private Button _speedButton;
    [SerializeField] private TextMeshProUGUI _staminaUpgradeMoney;
    [SerializeField] private TextMeshProUGUI _staminaUpgradeLevel;
    [SerializeField] private TextMeshProUGUI _speedUpgradeMoney;
    [SerializeField] private TextMeshProUGUI _speedUpgradeLevel;
    [SerializeField] private TextMeshProUGUI _incomeUpgradeMoney;
    [SerializeField] private TextMeshProUGUI _incomeUpgradeLevel;

    #endregion

    #region ProgressBar

    [Header("Progress")] [SerializeField] private Image _fillbar;
    [SerializeField] private Image _playerMarker;
    [SerializeField] private TextMeshProUGUI _levelText;
    private Transform _playerPosition;
    private Transform _finishPosition;
    private float _playerStartPos_Z;
    private float _totalDistance;

    #endregion


    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerPosition = FindObjectOfType<PlayerController>().transform;
        _finishPosition = GameObject.FindGameObjectWithTag("Finish").transform;
        _levelText.SetText($"{PersistData.Instance.CurrentLevel}");
        _playerStartPos_Z = _playerPosition.position.z;
        _totalDistance = Mathf.Abs(_playerStartPos_Z - _finishPosition.position.z);
        SetIncrementalUI();
    }

    private void OnEnable()
    {
        EventManager.OnGameStart += GameStarted;
        EventManager.OnIncrementalUpgrade += SetIncrementalUI;
        EventManager.OnGetIncome += UpdateMoneyText;
        EventManager.OnGameWin += DisableInGameUI;
        EventManager.OnGameLose += DisableInGameUI;
    }

    private void OnDisable()
    {
        EventManager.OnGameStart -= GameStarted;
        EventManager.OnIncrementalUpgrade -= SetIncrementalUI;
        EventManager.OnGetIncome -= UpdateMoneyText;
        EventManager.OnGameWin -= DisableInGameUI;
        EventManager.OnGameLose -= DisableInGameUI;
    }

    private void Update()
    {
        float fillAmount = Mathf.Clamp((-1 * _playerStartPos_Z + _playerPosition.position.z) / (_totalDistance), 0.0f,
            1.0f);
        _fillbar.fillAmount = fillAmount;
        // Vector2 playerMarkerPos = new Vector2((_fillbar.rectTransform.sizeDelta.x * fillAmount) + _fillbar.rectTransform.anchoredPosition.x, _playerMarker.rectTransform.anchoredPosition.y);
        // _playerMarker.rectTransform.anchoredPosition = playerMarkerPos;
    }

    private void GameStarted()
    {
        _tapToStart.SetActive(false);
        _incrementalsObj.SetActive(false);
    }

    private void UpdateMoneyText()
    {
        _playerMoney.SetText((PersistData.Instance.Money).ToString("f0"));
        FadeMoney();
    }

    private void SetIncrementalUI()
    {
        StartCoroutine(Sincapp.WaitAndAction(0, () =>
        {
            var persistData = PersistData.Instance;
            _playerMoney.SetText(persistData.Money.ToString("0"));

            _staminaUpgradeMoney.SetText($"${persistData.StaminaUpgradeCost}");
            _speedUpgradeMoney.SetText($"${persistData.SpeedUpgradeCost}");
            _incomeUpgradeMoney.SetText($"${persistData.IncomeUpgradeCost}");

            _incomeUpgradeLevel.SetText($"{persistData.IncomeLevel} LVL");
            _staminaUpgradeLevel.SetText($"{persistData.StaminaLevel} LVL");
            _speedUpgradeLevel.SetText($"{persistData.SpeedLevel} LVL");
        }));

        CheckIncrementalSprites();
    }

    private void CheckIncrementalSprites()
    {
        var persistData = PersistData.Instance;

        if (persistData.StaminaLevel < persistData.MaxStaminaLevel &&
            persistData.Money >= persistData.StaminaUpgradeCost)
        {
            _ageButton.interactable = true;
        }
        else
        {
            if (persistData.Money < persistData.StaminaUpgradeCost)
            {
                _staminaUpgradeMoney.SetText($"MAX LEVEL");
            }

            _ageButton.interactable = false;
        }

        if (persistData.IncomeLevel < persistData.MaxIncomeLevel &&
            persistData.Money >= persistData.IncomeUpgradeCost)
        {
            _incomeButton.interactable = true;
        }
        else
        {
            if (persistData.Money < persistData.IncomeUpgradeCost)
            {
                _incomeUpgradeMoney.SetText($"MAX LEVEL");
            }

            _incomeButton.interactable = false;
        }

        if (persistData.SpeedLevel < persistData.MaxSpeedLevel &&
            persistData.Money >= persistData.SpeedUpgradeCost)
        {
            _speedButton.interactable = true;
        }
        else
        {
            if (persistData.Money < persistData.SpeedUpgradeCost)
            {
                _speedUpgradeMoney.SetText($"MAX LEVEL");
            }

            _speedButton.interactable = false;
        }
    }

    private void FadeMoney()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("MoneyFade")) return;

        _animator.Play("MoneyFade");
    }

    private void DisableInGameUI()
    {
        gameObject.SetActive(false);
    }

    private void MoneySendUi(Vector3 spawnPos, float money)
    {
        var mainCam = Camera.main;
        var a = Mathf.InverseLerp(100, 1000, money);
        var b = Mathf.Lerp(1, 15, a);

        for (int i = 0; i < (int)1; i++)
        {
            Transform moneySpawned = Instantiate(_collectableSprite, mainCam.WorldToScreenPoint(spawnPos),
                Quaternion.identity,
                transform);
            float randY = Random.Range(220, 270);
            moneySpawned.DOMove(mainCam.WorldToScreenPoint(spawnPos) + Vector3.up * 100 + new Vector3(0, randY),
                    .7f)
                .OnComplete(() =>
                {
                    moneySpawned.DOMove(_collectableTargetTransform.position, .7f).OnComplete(() =>
                    {
                        FadeMoney();
                        Destroy(moneySpawned.gameObject);
                    });
                });
        }
    }
}