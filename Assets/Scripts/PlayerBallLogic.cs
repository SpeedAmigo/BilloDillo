using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBallLogic", menuName = "Scriptable Objects/PlayerBallLogic")]
public class PlayerBallLogic : ScriptableObject
{
    [Header("General Settings")]
    public PlayerBallLogicType ballType;
    
    public float speed;

    public float mass;
    public float linearDamping;
    public float angularDamping;

    [Header("Pufferfish Settings")] 
    [Range(0,1)] public float scaleUpBy;
    
    public void RabbitCollision(Collider thisObjectCollider, float time)
    {
        if (ballType != PlayerBallLogicType.Rabbit) return;
    }
    
    public void HedgehogCollision(GameObject thisObject, GameObject otherObject)
    {
        if (ballType != PlayerBallLogicType.Hedgehog) return;
        
        otherObject.transform.SetParent(thisObject.transform);
        
        var objectBody = otherObject.GetComponent<Rigidbody>();
        objectBody.linearVelocity = Vector3.zero;
        objectBody.angularVelocity = Vector3.zero;
        
        otherObject.transform.localPosition = Vector3.zero;
    }

    public void PufferfishCollision(GameObject thisObject) 
    {
        if  (ballType != PlayerBallLogicType.Pufferfish) return;
        
        thisObject.transform.localScale *= (1 + scaleUpBy);
    }

    public void SnailCollision()
    {
        if  (ballType != PlayerBallLogicType.Snail) return;
    }
}

public enum PlayerBallLogicType
{
    Armadillo,
    Hedgehog,
    Pufferfish,
    Rabbit,
    Snail
}
