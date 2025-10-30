using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            RegisterConnection(Owner);
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterConnection(NetworkConnection conn)
    {
        GameplayManager.Instance.AddPlayerConnection(conn);
        PlayerId(conn);
    }

    [ObserversRpc(RunLocally = true)]
    private void PlayerId(NetworkConnection conn)
    {
        Debug.Log($"player id {conn}");
    }
}
