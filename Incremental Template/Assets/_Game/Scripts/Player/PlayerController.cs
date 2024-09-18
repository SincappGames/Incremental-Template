using DG.Tweening;
using ElephantSDK;
using SincappStudio;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerStates
    {
        Idle,
        Run,
        Dead,
        OnFinishWall,
        Finish
    }

    public static PlayerStates PlayerState;
    private PlayerMovementController _playerMovementController;
    private PlayerAttackController _playerAttackController;
    
    private void Awake()
    {
        _playerAttackController = GetComponent<PlayerAttackController>();
        _playerMovementController = GetComponent<PlayerMovementController>();
    }

    private void OnEnable()
    {
        EventManager.OnGameStart += StartGame;
        EventManager.OnGateCollect += TakeSkills;
    }

    private void OnDisable()
    {
        EventManager.OnGameStart -= StartGame;
        EventManager.OnGateCollect -= TakeSkills;
    }

    private void Start()
    {
        PlayerState = PlayerStates.Idle;
    }

    private void StartGame()
    {
        PlayerState = PlayerStates.Run;
        _playerAttackController.StartShooting();
        
        var persistData = PersistData.Instance;
        persistData.Save();
        var level = persistData.CurrentLevel;
        Elephant.LevelStarted(level);
    }

    private void TakeSkills(GateGroupController.SkillTypes skillType, float skillAmount)
    {
        _playerAttackController.CalculateSkills(skillType, skillAmount);
    }
    
    public void EnterFinish()
    {
        PlayerState = PlayerStates.OnFinishWall;
        EventManager.OnFinishWall?.Invoke();
    }

    public void CollectMoney(float moneyAmount)
    {
        PersistData persistData = PersistData.Instance;
        persistData.Money += moneyAmount;
        EventManager.OnMoneyChange?.Invoke();
    }

    private void EndGameReach()
    {
        Die(0);
        PersistData.Instance.EndGameReachCount++;
    }

    public void Die(float delay)
    {
        transform.DOKill();
        PlayerState = PlayerStates.Finish;
        StartCoroutine(Sincapp.WaitAndAction(delay, () => { EventManager.OnGameWin?.Invoke(); }));
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IInteractable>()?.InteractPlayer(transform);
        
        if (other.gameObject.layer == LayerHandler.EndGameWall)
        {
            other.gameObject.layer = 0;
            EndGameReach();
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            _playerMovementController.enabled = !_playerMovementController.enabled;
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
           Die(0);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndGameReach();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Time.timeScale = Time.timeScale == 1 ? 3 : 1;
        }
    }
}