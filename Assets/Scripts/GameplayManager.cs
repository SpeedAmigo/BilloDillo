using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class GameplayManager : NetworkBehaviour
{
    public static event Action<int, int> OnBallImage;
    public static event Action<bool> OnGameOver;
    public static event Action<int, int> OnPointsUpdate;
    
    public static GameplayManager Instance;
    
    [SerializeField] private NetworkObject playerBall;
    [SerializeField] private NetworkObject pointer;
    [SerializeField] private NetworkObject ballPicker;
    [SerializeField] private int numberOfPlayers;
    
    private List<NetworkConnection> _playerConnections = new();
    public int _currentPlayerIndex = 0;
    
    public List<PlayerScript> players = new();

    public PlayerScript currentPlayer;
    
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
            _currentPlayerIndex = 0;
            if (players.Count > 0)
                currentPlayer = players[0];
            
            playerBall.GiveOwnership(_playerConnections[0]);
            pointer.GiveOwnership(_playerConnections[0]);
            ballPicker.GiveOwnership(_playerConnections[0]);
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
            current.points.Value += ball.ballPoints;
            UpdatePlayerPointsObservers(current.points.Value, _currentPlayerIndex);
            
            other.ballType.Value = (ball.ballType == BallType.Full) ? BallType.Half : BallType.Full;

            if (current.ballType.Value == ball.ballType)
            {
                AddBallImageObservers(ball.ballIndex, _currentPlayerIndex);
            }
            else
            {
                AddBallImageObservers(ball.ballIndex, otherPlayerIndex);
            }
        }
        else
        {
            if (players[_currentPlayerIndex].ballType.Value == ball.ballType)
            {
                players[_currentPlayerIndex].collectedBalls.Value++;
                players[_currentPlayerIndex].points.Value += ball.ballPoints;
                UpdatePlayerPointsObservers(players[_currentPlayerIndex].points.Value, _currentPlayerIndex);
                AddBallImageObservers(ball.ballIndex, _currentPlayerIndex);
            }
            else
            {
                players[otherPlayerIndex].collectedBalls.Value++;
                players[otherPlayerIndex].points.Value += ball.ballPoints;
                UpdatePlayerPointsObservers(players[otherPlayerIndex].points.Value, otherPlayerIndex);
                AddBallImageObservers(ball.ballIndex, otherPlayerIndex);
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

    [ServerRpc(RequireOwnership = false)]
    public void PlayerPayCost(int price)
    {
        currentPlayer.points.Value -= price;
        UpdatePlayerPointsObservers(currentPlayer.points.Value, _currentPlayerIndex);
        Debug.Log("cost payed");
    }
    
    [ObserversRpc(BufferLast = true)]
    private void AddBallImageObservers(int ballIndex, int playerIndex)
    {
        OnBallImage?.Invoke(ballIndex, playerIndex);
    }

    [ObserversRpc(BufferLast = true)]
    private void UpdatePlayerPointsObservers(int value, int playerIndex)
    {
        OnPointsUpdate?.Invoke(value, playerIndex);
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
        ballPicker.GiveOwnership(_playerConnections[_currentPlayerIndex]);
        
        if (players.Count > _currentPlayerIndex)
            currentPlayer = players[_currentPlayerIndex];
    }
}
