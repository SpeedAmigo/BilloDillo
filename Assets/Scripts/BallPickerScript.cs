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

    public void ChangeBall(PlayerBallLogic playerBallLogic)
    {
        ChangeBallServer(playerBallLogic);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeBallServer(PlayerBallLogic playerBallLogic)
    {
        if (GameplayManager.Instance.currentPlayer.points.Value < playerBallLogic.price)
        {
            Debug.Log("Not enough points");
            return;
        }
        
        //GameplayManager.Instance.currentPlayer.PayPrice(playerBallLogic.price);
        GameplayManager.Instance.PlayerPayCost(playerBallLogic.price);
        
        ChangeVisualServer(playerBallLogic.index);
        ChangeLogicServer(playerBallLogic);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ChangeVisualServer(int index)
    {
        for (int i = 0; i < ballVisuals.Length; i++)
        {
            ballVisuals[i].SetActive(i == index);
        }
        
        ChangeVisualClient(index);
    }
    
    [ServerRpc(RequireOwnership = false)]
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
