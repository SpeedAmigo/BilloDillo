using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForceSliderScript : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public static bool IsDraggingHandle = false;
    public static event Action<float> OnHandleRelease;
    
    [SerializeField] private float returnSpeed;
    [SerializeField] private Slider _slider;

    private bool _returnComplete;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }
    
    public void HandleRelease()
    {
        var force = _slider.value;
        OnHandleRelease?.Invoke(force);
        _returnComplete = false;
    }

    private void Update()
    {
        if (!_returnComplete)
        {
            _slider.value -= returnSpeed * Time.deltaTime;

            if (_slider.value <= 0)
            {
                _returnComplete = true;
                _slider.value = 0;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraggingHandle = false;
        HandleRelease();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDraggingHandle = true;
    }
}
