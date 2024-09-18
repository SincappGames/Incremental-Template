using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ElephantSDK;
using Lofelt.NiceVibrations;
using SincappStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class InGameUIManager : MonoSingleton<InGameUIManager>
{
    [SerializeField] private GameObject _tapToStart;
    [SerializeField] private TextMeshProUGUI _playerMoney;
    [SerializeField] private Transform _collectableSprite;
    [SerializeField] private Transform _collectableTargetTransform;
    [SerializeField] private Material _greyMat;
    [SerializeField] private Slider _vibrationSlider;
    [SerializeField] private GameObject _moneyImage;
    [SerializeField] private GameObject _settingsImage;
    [SerializeField] private GameObject _rewardedCanvas;
    [SerializeField] private TextMeshProUGUI _levelText;
    private Animator _animator;

    #region Incrementals

    [Header("Incrementals")] 
   
    public GameObject incrementalsObj;
    [SerializeField] private Button _fireRateButton;
    [SerializeField] private Button _rangeButton;
    [SerializeField] private Button _incomeButton;
    [SerializeField] private TextMeshProUGUI _fireRateUpgradeMoney;
    [SerializeField] private TextMeshProUGUI _fireRateUpgradeLevel;
    [SerializeField] private TextMeshProUGUI _rangeUpgradeMoney;
    [SerializeField] private TextMeshProUGUI _rangeUpgradeLevel;
    [SerializeField] private TextMeshProUGUI _incomeUpgradeMoney;
    [SerializeField] private TextMeshProUGUI _incomeUpgradeLevel;

    #endregion

    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _levelText.SetText($"LEVEL {PersistData.Instance.CurrentLevel}");
        var isHapticOn = PlayerPrefsX.GetBool("HapticMode",true);
        _vibrationSlider.value = isHapticOn ? 1 : 0;
        HapticController.hapticsEnabled = isHapticOn;
        SetIncrementalUI();
    }

    private void OnEnable()
    {
        EventManager.OnGameStart += GameStarted;
        EventManager.OnIncrementalUpgrade += SetIncrementalUI;
        EventManager.OnMoneyChange += UpdateMoneyText;
        EventManager.OnGameWin += DisableInGameUI;
        EventManager.OnGameLose += DisableInGameUI;
    }

    private void OnDisable()
    {
        EventManager.OnGameStart -= GameStarted;
        EventManager.OnIncrementalUpgrade -= SetIncrementalUI;
        EventManager.OnMoneyChange -= UpdateMoneyText;
        EventManager.OnGameWin -= DisableInGameUI;
        EventManager.OnGameLose -= DisableInGameUI;
    }

    private void GameStarted()
    {
        _tapToStart.SetActive(false);
        incrementalsObj.SetActive(false);
    }

    private void UpdateMoneyText()
    {
        _playerMoney.SetText((PersistData.Instance.Money).ToString("f0"));
        FadeMoney();
        SetIncrementalUI();
    }

    public void GameUIStatus(bool isOpen)
    {
        if (isOpen)
        {
            _levelText.transform.parent.gameObject.SetActive(true);
            _settingsImage.SetActive(true);
            _moneyImage.SetActive(true);
            _rewardedCanvas.SetActive(true);
        }
        else
        {
            _levelText.transform.parent.gameObject.SetActive(false);
            _settingsImage.SetActive(false);
            _moneyImage.SetActive(false);
            _rewardedCanvas.SetActive(false);
        }
    }
    private void SetIncrementalUI()
    {
        var persistData = PersistData.Instance;
        _playerMoney.SetText(Sincapp.AbbreviateNumber(persistData.Money));
       
        _fireRateUpgradeMoney.SetText( $"{Sincapp.AbbreviateNumber(persistData.FireRateUpgradeCost)}$");
        _rangeUpgradeMoney.SetText($"{Sincapp.AbbreviateNumber(persistData.RangeUpgradeCost)}$");
        _incomeUpgradeMoney.SetText($"{Sincapp.AbbreviateNumber(persistData.IncomeUpgradeCost)}$");

        _fireRateUpgradeLevel.SetText($"{persistData.FireRateLevel}");
        _rangeUpgradeLevel.SetText($"{persistData.RangeLevel}");
        _incomeUpgradeLevel.SetText($"{persistData.IncomeLevel}");
            
        CheckIncrementalSprites();
    }

    private void CheckIncrementalSprites()
    {
        var persistData = PersistData.Instance;

        if (persistData.FireRateLevel < persistData.MaxFireRateLevel &&
            persistData.Money >= persistData.FireRateUpgradeCost)
        {
            _fireRateButton.interactable = true;
            _fireRateButton.image.material = null;
        }
        else
        {
            if (persistData.FireRateLevel >= persistData.MaxFireRateLevel)
            {
                _fireRateUpgradeMoney.SetText($"MAX LEVEL");
            }

            _fireRateButton.interactable = false;
            _fireRateButton.image.material = _greyMat;
        }

        if (persistData.RangeLevel < persistData.MaxRangeLevel &&
            persistData.Money >= persistData.RangeUpgradeCost)
        {
            _rangeButton.interactable = true;
            _rangeButton.image.material = null;
        }
        else
        {
            if (persistData.RangeLevel >= persistData.MaxRangeLevel)
            {
                _rangeUpgradeMoney.SetText($"MAX LEVEL");
            }

            _rangeButton.interactable = false;
            _rangeButton.image.material = _greyMat;
        }
        
        if (persistData.IncomeLevel < persistData.MaxIncomeLevel &&
            persistData.Money >= persistData.IncomeUpgradeCost)
        {
            _incomeButton.interactable = true;
            _incomeButton.image.material = null;
        }
        else
        {
            if (persistData.IncomeLevel >= persistData.MaxIncomeLevel)
            {
                _incomeUpgradeMoney.SetText($"MAX LEVEL");
            }

            _incomeButton.interactable = false;
            _incomeButton.image.material = _greyMat;
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
    
    public void SetVibration()
    {
        _vibrationSlider.value = _vibrationSlider.value == 1 ? 0 : 1;
        bool isHapticOn = _vibrationSlider.value == 1;
        HapticController.hapticsEnabled = isHapticOn;
        PlayerPrefsX.SetBool("HapticMode", isHapticOn);
    }

    public void DirectPrivacy()
    {
        Elephant.ShowSettingsView();
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