using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerScript : NetworkBehaviour
{
    [SerializeField] private HandMovementScript handMovementScript;
    [SerializeField] private NetworkObject playerBall;
    [SerializeField] private float forceMultiplier = 20;
    private void OnEnable()
    {
       transform.position = playerBall.transform.position;

       ForceSliderScript.OnHandleRelease += PerformShoot;
    }

    private void OnDisable()
    {
        ForceSliderScript.OnHandleRelease -= PerformShoot;
    }

    private void PerformShoot(float force)
    {
        PlayerBallScript ball = playerBall.GetComponent<PlayerBallScript>();
        
        Vector3 direction = transform.forward;
        float forceMagnitude = force * forceMultiplier;
        
        ball.ShootBall(direction, forceMagnitude);
    }

    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        if (IsOwner)
        {
            enabled = true;
            handMovementScript.enabled = true;
            //gameObject.SetActive(true);
            transform.position = playerBall.transform.position;
        }
        else
        {
            enabled = false;
            handMovementScript.enabled = false;
            //gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (!Mouse.current.rightButton.isPressed) return;
            
        Vector3 mousePos = MousePosition.GetMousePosition();
        Vector3 direction = mousePos - transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
