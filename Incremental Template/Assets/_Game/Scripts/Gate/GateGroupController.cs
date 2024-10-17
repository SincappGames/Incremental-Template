using System.Globalization;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class GateGroupController : BaseInteractableController, IDataCollectable
{
    public enum SkillTypes
    {
        FireRate,
        Range,
    }
    
    #region Gate 1 Attributes
    
    [TabGroup("Gate 1")]
    [LabelText("Skill Type"),GUIColor(.5f,1f,.5f)]
    [SerializeField] private SkillTypes _firstGateSkillType;

    [TabGroup("Gate 1")]
    [LabelText("Skill Amount")]
    [SerializeField] private float _firstGateSkillAmount;

    [TabGroup("Gate 1")]
    [LabelText("Power Amount")]
    [SerializeField] private float _firstGatePowerAmount;
    
    [TabGroup("Gate 1")]
    [LabelText("Destructible Type")]
    [SerializeField] private DestructibleBaseSO _firstGateDestructibleType;
    
    [TabGroup("Gate 1")]
    [LabelText("Lock Amount")]
    [ShowIf("_firstGateDestructibleType", null)]
    [SerializeField] private float _firstGateLockAmount;
    
    private GateController _firstGateController;
    
    #endregion

    #region Gate 2 Attributes
    
    [TabGroup("Gate 2")]
    [LabelText("Skill Type"),GUIColor(.4f,1f,.4f)]
    [SerializeField] private SkillTypes _secondGateSkillType;

    [TabGroup("Gate 2")]
    [LabelText("Skill Amount")]
    [SerializeField] private float _secondGateSkillAmount;

    [TabGroup("Gate 2")]
    [LabelText("Power Amount")]
    [SerializeField] private float _secondGatePowerAmount;
    
    [TabGroup("Gate 2")]
    [LabelText("Destructible Type")]
    [SerializeField] private DestructibleBaseSO _secondGateDestructibleType;
    
    [TabGroup("Gate 2")]
    [LabelText("Lock Amount")]
    [ShowIf("_secondGateDestructibleType", null)]
    [SerializeField] private float _secondGateLockAmount;
    
    private GateController _secondGateController;
    
    #endregion

    private bool _isSingleGate;
    private InteractableMovementController _interactableMovementController;
    private GateController[] _gateControllers;

    private void Start()
    {
        _interactableMovementController = GetComponent<InteractableMovementController>();
        _gateControllers = GetComponentsInChildren<GateController>();
        _firstGateController = _gateControllers[0];
        _secondGateController = _gateControllers.Length < 2 ? null : _gateControllers[1];
        
        if (_gateControllers.Length == 1)
        {
            _isSingleGate = true;
        }

        if (_firstGateController != null)
        {
            _firstGateController.InitGate(_firstGateSkillType, _firstGateSkillAmount, _firstGatePowerAmount,_firstGateDestructibleType, _firstGateLockAmount);
        }

        if (_secondGateController != null)
        {
            _secondGateController.InitGate(_secondGateSkillType, _secondGateSkillAmount, _secondGatePowerAmount,_secondGateDestructibleType, _secondGateLockAmount);
        }
    }

    private GateController FindInteractedGate(float bulletXPos)
    {
        if (!_isSingleGate)
        {
            if (bulletXPos < 0)
            {
                return _firstGateController;
            }
           
            return _secondGateController;
        }

        return _firstGateController;
    }
    
    public override void TakeBulletDamage(float damageAmount, BaseBulletController bullet)
    {
        base.TakeBulletDamage(damageAmount, bullet);
        
        GateController hittedGate = FindInteractedGate(bullet.transform.position.x);
        hittedGate.IncreaseSkillAmountOnBulletHit(bullet.transform.position);
        _interactableMovementController.Move();
        ParticleManager.Instance.GateParticle(bullet.transform.position);
    }
    
    public override void InteractPlayer(Transform playerTransform)
    {
        base.InteractPlayer(playerTransform);
        
        GateController hittedGate = FindInteractedGate(playerTransform.position.x);
        GateController otherGate = FindInteractedGate(-playerTransform.position.x);

        hittedGate.UseSkill();
        transform.DOKill();
        otherGate.SetGateGrey();
        
        foreach (GateController gateController in _gateControllers)
        {
            gateController.DestroyGate();
        }
    }
    
    private void DeactiveGates()
    {
        gameObject.layer = 0;

        foreach (GateController gateController in _gateControllers)
        {
            gateController.SetGateGrey();
        }
    }

    private void Update()
    {
        if (gameObject.layer == 0) return;
        
        if (LevelManager.Instance.PlayerController.transform.position.z  > transform.position.z + .4f)
        {
            DeactiveGates();
        }
    }

    public InteractableData GetInteractableData()
    {
        InteractableData data = new InteractableData();

        LevelDataHandler.AddProperty(data, ("_firstGateSkillType", _firstGateSkillType.ToString()),
            ("_firstGateSkillAmount", _firstGateSkillAmount.ToString(CultureInfo.InvariantCulture)),
            ("_firstGatePowerAmount", _firstGatePowerAmount.ToString(CultureInfo.InvariantCulture)),
            ("_secondGateSkillType", _secondGateSkillType.ToString()),
            ("_secondGateSkillAmount", _secondGateSkillAmount.ToString(CultureInfo.InvariantCulture)),
            ("_secondGatePowerAmount", _secondGatePowerAmount.ToString(CultureInfo.InvariantCulture)),
            ("_firstGateLockAmount", _firstGateLockAmount.ToString(CultureInfo.InvariantCulture)),
            ("_secondGateLockAmount", _secondGateLockAmount.ToString(CultureInfo.InvariantCulture)));
        
        LevelDataHandler.AddProperty(data,
            ("_firstGateDestructibleType",_firstGateDestructibleType),
            ("_secondGateDestructibleType",_secondGateDestructibleType));
        
        LevelDataHandler.AddTransformValues(data, transform.position,transform.rotation);
        return data;
    }

    public GameObject GetGameObjectReference()
    {
        return gameObject;
    }
}