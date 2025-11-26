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
    
    public void HedgehogCollision(GameObject thisObject, GameObject otherObject, Vector3 hitPoint)
    {
        if (ballType != PlayerBallLogicType.Hedgehog) return;
        
        if (thisObject.transform.childCount >= 6) return;
        
        var rb = otherObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        
        otherObject.transform.SetParent(thisObject.transform);
        
        Vector3 localOffset = thisObject.transform.InverseTransformPoint(hitPoint);
        otherObject.transform.localPosition = localOffset;
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
