using System;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBallScript : NetworkBehaviour
{
    private Rigidbody _body;
    private InputSystem_Actions _inputSystem;
    
    private bool _haveShot = false;
    [AllowMutableSyncType] private SyncVar<bool> _isMoving = new();
    
    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _inputSystem = new InputSystem_Actions();
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
        
        ApplyOwnership(IsOwner);
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        ApplyOwnership(IsOwner);
    }

    private void ApplyOwnership(bool owner)
    {
        _inputSystem.Player.Jump.performed -= OnSpacePressed;

        if (owner)
        {
            _inputSystem.Enable();
            _inputSystem.Player.Jump.performed += OnSpacePressed;
        }
        else
        {
            _inputSystem.Disable();
        }
    }
    
    private void OnDestroy()
    {
        if (_inputSystem != null)
        {
            _inputSystem.Player.Jump.performed -= OnSpacePressed;
            _inputSystem.Disable();
        }
    }
    
    private void OnSpacePressed(InputAction.CallbackContext context)
    {
        if (IsOwner && context.performed)
        {
            if (_haveShot) return;
            
            Vector3 mousePos = GetMousePosition();
            Vector3 direction = mousePos - transform.position;
            
            ShootBall(direction);
            
            Debug.Log("Client pressed space");
            OwnerTestRpcServer();
            _haveShot = true;
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void ShootBall(Vector3 direction)
    {
        _body.AddForce(-direction.normalized * 5f, ForceMode.Impulse);
        
        Invoke(nameof(MoveDelay), 0.5f);
    }

    [Server]
    private void MoveDelay()
    {
        _isMoving.Value = true;
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        if (_haveShot && _isMoving.Value && _body.linearVelocity.magnitude <= 0.01f)
        {
            GameplayManager.Instance.NextTurn(NetworkObject);
            Debug.Log("Ball stopped, next turn");
            _haveShot = false;
            _isMoving.Value = false;
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = 10f; // Distance from the camera
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    [ServerRpc(RequireOwnership = true)]
    private void OwnerTestRpcServer()
    {
        OwnerTestRpcClient();
    }
    
    [ObserversRpc(RunLocally = true)]
    private void OwnerTestRpcClient()
    {
        Debug.Log("Client received owner test");
    }
}
