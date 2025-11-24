using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{
    public static event Action<int, int> OnBallImage;
    public static event Action<bool> OnGameOver;
    
    public static GameplayManager Instance;

    [SerializeField] private NetworkObject playerBall;
    [SerializeField] private NetworkObject pointer;
    [SerializeField] private int numberOfPlayers;
    
    private List<NetworkConnection> _playerConnections = new();
    public int _currentPlayerIndex = 0;
    
    public List<PlayerScript> players = new();

    public List<Rigidbody> gameBalls = new();
    
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
    public void AddPlayerBall(GameBallScript ball)
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
            current.ballType.Value = ball.ballType;
            current.collectedBalls.Value++;
            
            other.ballType.Value = (ball.ballType == BallType.Full) ? BallType.Half : BallType.Full;

            if (current.ballType.Value == ball.ballType)
            {
                AddBallImageObservers(ball.ballIndex, _currentPlayerIndex);
                //OnBallImage?.Invoke(ball.ballImage, _currentPlayerIndex);
            }
            else
            {
                AddBallImageObservers(ball.ballIndex, otherPlayerIndex);
                //OnBallImage?.Invoke(ball.ballImage, otherPlayerIndex);
            }
        }
        else
        {
            if (players[_currentPlayerIndex].ballType.Value == ball.ballType)
            {
                players[_currentPlayerIndex].collectedBalls.Value++;
                AddBallImageObservers(ball.ballIndex, _currentPlayerIndex);
                //OnBallImage?.Invoke(ball.ballImage, _currentPlayerIndex);
            }
            else
            {
                players[otherPlayerIndex].collectedBalls.Value++;
                AddBallImageObservers(ball.ballIndex, otherPlayerIndex);
                //OnBallImage?.Invoke(ball.ballImage, otherPlayerIndex);
            }
        }
        
        if (ball.ballType == BallType.Black)
        {
            if (CheckIfPlayerCanShootBlack(current))
            {
                GameOver(true);
            }
            else
            {
                GameOver(false);
            }
        }
    }

    [ObserversRpc(BufferLast = true)]
    private void AddBallImageObservers(int ballIndex, int playerIndex)
    {
        OnBallImage?.Invoke(ballIndex, playerIndex);
    }

    [Server]
    private void GameOver(bool won)
    {
        if (_playerConnections.Count == 0) return;
        
        int otherPlayerIndex = (_currentPlayerIndex == 0) ? 1 : 0;
        NetworkConnection winnerConn = won ? _playerConnections[_currentPlayerIndex] : _playerConnections[otherPlayerIndex];

        foreach (var conn in _playerConnections)
        {
            bool isWinner = (conn == winnerConn);
            TargetShowGameOver(conn, isWinner);
        }
    }

    [TargetRpc]
    private void TargetShowGameOver(NetworkConnection conn, bool won)
    {
        OnGameOver?.Invoke(won);
    }
    
    [Server]
    private bool CheckIfPlayerCanShootBlack(PlayerScript player)
    {
        int maxBalls = 7;
        
        if (player.collectedBalls.Value < maxBalls)
        {
            return player.canShootBlackBall.Value = false;
        }
        else
        {
            return player.canShootBlackBall.Value = true;
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
