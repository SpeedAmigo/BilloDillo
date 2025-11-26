using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBallLogic", menuName = "Scriptable Objects/PlayerBallLogic")]
public class PlayerBallLogic : ScriptableObject
{
    public float speed;

    public float mass;
    public float linearDamping;
    public float angularDamping;
}
