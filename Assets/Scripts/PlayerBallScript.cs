using System;
using System.Collections;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerBallScript : NetworkBehaviour
{
    public PlayerBallLogic playerBallLogic;
    
    private Rigidbody _body;
    
    private bool _haveShot = false;
    [AllowMutableSyncType] private SyncVar<bool> _isMoving = new();

    private float _shootForce;
    
    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
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
        _body.AddForce(-direction.normalized * (force), ForceMode.Impulse);
        
        Debug.Log($"direction {direction}, force {force}");
        
        StartCoroutine(MoveDelayCoroutine());
    }

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
                playerBallLogic.PufferfishCollision(gameObject);
                break;
            }
        }
    }
}
