using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBallLogic", menuName = "Scriptable Objects/PlayerBallLogic")]
public class PlayerBallLogic : ScriptableObject
{
    [Header("General Settings")]
    public PlayerBallLogicType ballType;
    
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
        otherObject.transform.localPosition = localOffset + new Vector3(0.2f, 0, 0);
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
