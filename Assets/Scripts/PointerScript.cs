using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerScript : NetworkBehaviour
{
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private GameObject hitCircle;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject raycastOrigin;
    [SerializeField] private HandMovementScript handMovementScript;
    [SerializeField] private NetworkObject playerBall;
    [SerializeField] private float forceMultiplier = 20;
    [SerializeField] float offset;

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
        
        soundPlayer.PlayRandomGlobal("HandHit");
    }
    
    public override void OnOwnershipClient(NetworkConnection prevOwner)
    {
        if (IsOwner)
        {
            enabled = true;
            handMovementScript.enabled = true;
            transform.position = playerBall.transform.position;
        }
        else
        {
            enabled = false;
            handMovementScript.enabled = false;
            lineRenderer.enabled = false;
            hitCircle.SetActive(false);
        }
    }

    private void FireRuntimeRaycast()
    {
        if (raycastOrigin == null) return;
        
        Ray ray = new Ray(raycastOrigin.transform.position, raycastOrigin.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 offsetHitPoint = hit.point + hit.normal * offset;
            
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, offsetHitPoint);

            if (hitCircle != null)
            {
                hitCircle.SetActive(true);
                hitCircle.transform.position = offsetHitPoint;
            }
        }
        else
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, ray.origin + ray.direction * 100f);

            if (hitCircle != null)
            {
                hitCircle.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (UIUtility.IsPointerOverUI()) return;
        //if (ForceSliderScript.IsDraggingHandle) return;

        bool mouseAvailable = Mouse.current != null && Mouse.current.rightButton != null;
        bool touchAvailable = Touchscreen.current != null && Touchscreen.current.primaryTouch != null;
        
        bool mousePressed = mouseAvailable && Mouse.current.rightButton.isPressed;
        bool touchPressed = touchAvailable && Touchscreen.current.primaryTouch.isInProgress;

        if (!mousePressed && !touchPressed) return;
        
        Vector3 inputWorldPos;

        if (mousePressed)
        {
            inputWorldPos = MousePosition.GetMousePosition();
        }
        else
        {
            // Convert touch screen position to world position
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Camera cam = Camera.main;
            if (cam == null) return;

            // Use the pointer's current depth to compute a reasonable world Z
            float z = cam.WorldToScreenPoint(transform.position).z;
            Vector3 screenPos = new Vector3(touchPos.x, touchPos.y, z);
            inputWorldPos = cam.ScreenToWorldPoint(screenPos);
        }
        
        Vector3 direction = inputWorldPos - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        FireRuntimeRaycast();
    }
}
