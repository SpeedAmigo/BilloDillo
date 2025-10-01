using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

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
    }
}
