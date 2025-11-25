using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class HandMovementScript : NetworkBehaviour
{
    [SerializeField] private Slider slider;
    
    [SerializeField] private float moveBackMultiplier;

    private float _originalOffset;
    
    private void Start()
    {
        _originalOffset = transform.localPosition.z;
    }
    
    private void Update()
    {
        float targetZ = _originalOffset + (slider.value * moveBackMultiplier);
        transform.localPosition = new Vector3(
            transform.localPosition.x,
            transform.localPosition.y, 
            targetZ
            );
    }
}
