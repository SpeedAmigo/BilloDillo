using System;
using System.Collections;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Jobs;

public class PlayerBallScript : NetworkBehaviour
{
    public PlayerBallLogic playerBallLogic;
    
    [SerializeField] private float defaultMass;
    [SerializeField] private float snailMass;
    
    [Header("Pufferfish Settings")] 
    [Range(0,1)] public float scaleUpBy;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private float explosionRadius = 3f;
    
    [Header("Rabbit Settings")]
    [SerializeField] private float rabbitPhaseTime = 3f;
    
    [AllowMutableSyncType] private SyncVar<bool> _isMoving = new();

    private Rigidbody _body;
    
    private bool _haveShot = false;
    private float _shootForce;
    private int _defaultLayer;
    private int _ghostLayer;
    
    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _defaultLayer = LayerMask.NameToLayer("Default");
        _ghostLayer = LayerMask.NameToLayer("RabbitGhost");
    }
    
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        if (!IsOwner)
        {
            enabled = false;
        }
        else
        {
            enabled = true;
        }
    }
    
    public void ShootBall(Vector3 direction, float force)
    {
        if (IsOwner)
        {
            if (_haveShot) return;
            
            ShootBallServer(direction, force);

            _haveShot = true;
        }
    }
    
    [ServerRpc(RequireOwnership = true)]
    private void ShootBallServer(Vector3 direction, float force)
    {
        _body.mass = defaultMass;
        _body.AddForce(-direction.normalized * (force), ForceMode.Impulse);

        switch (playerBallLogic.ballType)
        {
            case PlayerBallLogicType.Rabbit:
            {
                StartCoroutine(RabbitPhaseCoroutine());
                break;
            }
            case PlayerBallLogicType.Snail:
            {
                _body.mass = snailMass;
                break;
            }
        }
        
        StartCoroutine(MoveDelayCoroutine());
    }
    
    private void Update()
    {
        if (!IsOwner) return;
        
        //Debug.Log($"{_haveShot} - true, {_isMoving.Value} - true, {_body.IsSleeping()} - true");
        
        if (_haveShot && _isMoving.Value && _body.IsSleeping())
        {
            _haveShot = false;
            SetIsMoving(false);
            GameplayManager.Instance.NextTurn();
        }
    }
    
    #region Helpers
    
    [Server]
    private IEnumerator MoveDelayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        MoveDelay();
    }

    [Server]
    private void MoveDelay()
    {
        _isMoving.Value = true;
    }
    
    [ServerRpc(RequireOwnership = true)]
    private void SetIsMoving(bool value)
    {
        _isMoving.Value = value;
    }
    
    #endregion

    #region playerBallTypeMethods

    // method for rabbit
    private IEnumerator RabbitPhaseCoroutine()
    {
        gameObject.layer = _ghostLayer; // phase through everything except walls

        yield return new WaitForSeconds(rabbitPhaseTime);

        gameObject.layer = _defaultLayer; // restore normal collisions
    }
    
    private void PufferfishCollision() 
    {
        if (playerBallLogic.ballType != PlayerBallLogicType.Pufferfish) return;
        
        transform.localScale *= (1 + scaleUpBy);
        
        DoExplosion();
    }
    
    private void DoExplosion()
    {
        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.attachedRigidbody;

            if (rb != null && rb != _body)
            {
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 0, ForceMode.Impulse);
            }
        }
    }

    #endregion
    
    private void OnCollisionEnter(Collision collision)
    {
        switch (playerBallLogic.ballType)
        {
            case PlayerBallLogicType.Hedgehog:
            {
                Vector3 hitPoint = collision.contacts[0].point;
                playerBallLogic.HedgehogCollision(gameObject, collision.gameObject, hitPoint);
                break;
            }
            case PlayerBallLogicType.Pufferfish:
            {
                PufferfishCollision();
                break;
            }
        }
    }
}
