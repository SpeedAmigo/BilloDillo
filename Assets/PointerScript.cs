using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerScript : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerBall;
    
    private void OnEnable()
    {
       transform.position = playerBall.transform.position; 
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        if (IsOwner)
        {
            gameObject.SetActive(true);
            transform.position = playerBall.transform.position;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        Vector3 mousePos = MousePosition.GetMousePosition();
        Vector3 direction = mousePos - transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
