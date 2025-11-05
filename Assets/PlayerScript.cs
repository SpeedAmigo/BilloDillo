using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerScript : NetworkBehaviour
{
    [AllowMutableSyncType] public SyncVar<BallType> ballType;
    [AllowMutableSyncType] public SyncVar<int> collectedBalls;
    
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
        GameplayManager.Instance.AddPlayerConnection(conn, this);
    }
}

public enum BallType
{
    None,
    Half,
    Full,
    Black
}
