using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class BallPickerScript : NetworkBehaviour
{
    [SerializeField] private GameObject[] ballVisuals;
    
    [SerializeField] private PlayerBallScript playerBall;
    
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

    public void ChangeVisual(int index)
    {
        ChangeVisualServer(index);
    }

    public void ChangeLogic(PlayerBallLogic playerBallLogic)
    {
        ChangeLogicServer(playerBallLogic);
    }

    [ServerRpc(RequireOwnership = true)]
    private void ChangeVisualServer(int index)
    {
        for (int i = 0; i < ballVisuals.Length; i++)
        {
            ballVisuals[i].SetActive(i == index);
        }
        
        ChangeVisualClient(index);
    }

    [ServerRpc(RequireOwnership = true)]
    private void ChangeLogicServer(PlayerBallLogic playerBallLogic)
    {
        playerBall.playerBallLogic = playerBallLogic;
        
        ChangeLogicClient(playerBallLogic);
    }

    [ObserversRpc(BufferLast = true)]
    private void ChangeVisualClient(int index)
    {
        for (int i = 0; i < ballVisuals.Length; i++)
        {
            ballVisuals[i].SetActive(i == index);
        }
    }

    [ObserversRpc(BufferLast = true)]
    private void ChangeLogicClient(PlayerBallLogic playerBallLogic)
    {
        playerBall.playerBallLogic = playerBallLogic;
    }
}
