using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerScript : NetworkBehaviour
{
    [AllowMutableSyncType] public SyncVar<BallType> ballType;
    [AllowMutableSyncType] public SyncVar<int> collectedBalls;
    [AllowMutableSyncType] public SyncVar<bool> canShootBlackBall;
    [AllowMutableSyncType] public SyncVar<int> points;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            RegisterConnection(Owner);
            SetPoints();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void SetPoints()
    {
        points.Value = 100;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PayPrice(int price)
    {
        points.Value -= price;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AddPoints(int pointsToAdd)
    {
        points.Value += pointsToAdd;
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
