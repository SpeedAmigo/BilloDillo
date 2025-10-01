using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{ 
    public static GameplayManager Instance;

    [SerializeField] private NetworkObject playerBall;
    [SerializeField] private int numberOfPlayers;
    
    private List<NetworkConnection> _playerConnections = new();
    private int _currentPlayerIndex = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerConnection(NetworkConnection conn)
    {
        if (!_playerConnections.Contains(conn))
        {
            _playerConnections.Add(conn);
        }
        
        numberOfPlayers = _playerConnections.Count;
        ShowObserverDebug(numberOfPlayers);
        
        if (_playerConnections.Count == 1)
        {
            //playerBall.AssignOwnership(_playerConnections[0]);
            playerBall.GiveOwnership(_playerConnections[0]);
            CurrentTurnOfPlayer(_playerConnections[0]);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NextTurn(NetworkObject playerBall)
    {
        if (_playerConnections.Count == 0) return;
        
        _currentPlayerIndex = (_currentPlayerIndex + 1)% _playerConnections.Count;
        //playerBall.AssignOwnership(_playerConnections[_currentPlayerIndex]);
        playerBall.GiveOwnership(_playerConnections[_currentPlayerIndex]);
        CurrentTurnOfPlayer(_playerConnections[_currentPlayerIndex]);
    }

    [ObserversRpc]
    private void ShowObserverDebug(int numberOfPlayers)
    {
        Debug.Log(numberOfPlayers);
    }

    [ObserversRpc]
    private void CurrentTurnOfPlayer(NetworkConnection conn)
    {
        Debug.Log(conn);
    }
}
