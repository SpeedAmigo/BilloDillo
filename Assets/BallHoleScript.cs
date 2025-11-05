using System;
using FishNet.Object;
using UnityEngine;

public class BallHoleScript : NetworkBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out GameBallScript gameBall))
        {
            var netObj = gameBall.GetComponent<NetworkObject>();
            
            GameplayManager.Instance.AddPlayerBall(gameBall.ballType);
            
            Despawn(netObj);
        }
    }
}
