using System;
using FishNet.Object;
using UnityEngine;

public class BallHoleScript : NetworkBehaviour
{
    [SerializeField] private GameObject hedgehogVisual;
    private Collider _collider;
    private SoundPlayer _soundPlayer;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _soundPlayer = GetComponent<SoundPlayer>();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out GameBallScript gameBall))
        {
            var netObj = gameBall.GetComponent<NetworkObject>();
            
            GameplayManager.Instance.AddPlayerBall(gameBall);
            _soundPlayer.PlayRandomGlobal("BallFall");
            
            Despawn(netObj);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LockHole()
    {
        _collider.enabled = false;
        hedgehogVisual.SetActive(true);
        
        _soundPlayer.PlayRandomGlobal("HoleLock");
        
        LockHoleClients();
    }

    [ObserversRpc(BufferLast = true)]
    private void LockHoleClients()
    {
        _collider.enabled = false;
        hedgehogVisual.SetActive(true);
    }
}
