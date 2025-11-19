using FishNet.Object;

public class GameBallsParentScript : NetworkBehaviour
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        //UnParentChild();
    }

    private void Awake()
    {
        UnParentChild();
    }

    [Server]
    private void UnParentChild()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            
            child.SetParent(null, true);
        }
        
        //UnparentChild_Client();
    }

    [ObserversRpc(BufferLast = true)]
    private void UnparentChild_Client()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            
            child.SetParent(null, true);
        }
    }
}
