using System;
using FishNet.Object;
using UnityEngine;

public class BallHoleScript : NetworkBehaviour
{
    [SerializeField] private GameObject hedgehogVisual;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out GameBallScript gameBall))
        {
            var netObj = gameBall.GetComponent<NetworkObject>();
            
            GameplayManager.Instance.AddPlayerBall(gameBall);
            
            Despawn(netObj);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LockHole()
    {
        _collider.enabled = false;
        hedgehogVisual.SetActive(true);
        
        LockHoleClients();
    }

    [ObserversRpc(BufferLast = true)]
    private void LockHoleClients()
    {
        _collider.enabled = false;
        hedgehogVisual.SetActive(true);
    }
}
