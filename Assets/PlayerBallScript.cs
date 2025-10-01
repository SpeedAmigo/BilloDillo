using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBallScript : NetworkBehaviour
{
    private InputSystem_Actions _inputSystem;
    
    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        { 
            enabled = false;
        }
    }
    
    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Jump.performed += OnSpacePressed;
    }

    private void OnDisable()
    {
        _inputSystem.Disable();
        _inputSystem.Player.Jump.performed -= OnSpacePressed;
    }
    
    private void OnSpacePressed(InputAction.CallbackContext context)
    {
        if (IsOwner && context.performed)
        {
            GameplayManager.Instance.NextTurn(NetworkObject);
            Debug.Log("Client pressed space");
        }
    }
}
