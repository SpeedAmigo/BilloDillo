using System;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBallScript : NetworkBehaviour
{
    private Rigidbody _body;
    private InputSystem_Actions _inputSystem;
    
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
            ShootBall();
            GameplayManager.Instance.NextTurn(NetworkObject);
            Debug.Log("Client pressed space");
            OwnerTestRpcServer();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void ShootBall()
    {
        Vector3 mousePos = GetMousePosition();
        Vector3 direction = mousePos - transform.position;
            
        _body.AddForce(-direction.normalized * 500f);
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
