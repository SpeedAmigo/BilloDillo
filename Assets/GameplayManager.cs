using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{ 
    public static GameplayManager Instance;

    [SerializeField] private NetworkObject playerBall;
    [SerializeField] private NetworkObject pointer;
    [SerializeField] private int numberOfPlayers;
    
    private List<NetworkConnection> _playerConnections = new();
    public int _currentPlayerIndex = 0;
    
    public List<PlayerScript> players = new();
    
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
    public void AddPlayerConnection(NetworkConnection conn, PlayerScript player)
    {
        if (!_playerConnections.Contains(conn))
        {
            _playerConnections.Add(conn);
        }
        
        players.Add(player);

        numberOfPlayers = _playerConnections.Count;
        
        if (_playerConnections.Count == 1)
        {
            playerBall.GiveOwnership(_playerConnections[0]);
            pointer.GiveOwnership(_playerConnections[0]);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerBall(BallType ballType)
    {
        if (players.Count < 2)
        {
            Debug.LogWarning("Not enough players connected to assign ball types.");
            return;
        }
        
        int otherPlayerIndex = (_currentPlayerIndex == 0) ? 1 : 0;
        
        var current = players[_currentPlayerIndex];
        var other = players[otherPlayerIndex];
        
        if (current.ballType.Value == BallType.None && other.ballType.Value == BallType.None)
        {
            current.ballType.Value = ballType;
            current.collectedBalls.Value++;
            
            other.ballType.Value = (ballType == BallType.Full) ? BallType.Half : BallType.Full;
        }
        else
        {
            if (players[_currentPlayerIndex].ballType.Value == ballType)
            {
                players[_currentPlayerIndex].collectedBalls.Value++;
            }
            else
            {
                players[otherPlayerIndex].collectedBalls.Value++;
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void NextTurn()
    {
        Debug.Log("NextTurn");
        
        if (_playerConnections.Count == 0) return;
        
        _currentPlayerIndex = (_currentPlayerIndex + 1)% _playerConnections.Count;
        playerBall.GiveOwnership(_playerConnections[_currentPlayerIndex]);
        pointer.GiveOwnership(_playerConnections[_currentPlayerIndex]);
    }
}
