using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBallLogic", menuName = "Scriptable Objects/PlayerBallLogic")]
public class PlayerBallLogic : ScriptableObject
{
    [Header("General Settings")]
    public PlayerBallLogicType ballType;

    [Header("Price Settings")]
    public int price;

    [Header("Index Settings")]
    public int index;
}

public enum PlayerBallLogicType
{
    Armadillo,
    Hedgehog,
    Pufferfish,
    Rabbit,
    Snail
}
