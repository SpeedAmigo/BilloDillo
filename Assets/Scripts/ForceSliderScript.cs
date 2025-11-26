using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForceSliderScript : MonoBehaviour, IEndDragHandler
{
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
        HandleRelease();
    }
}
