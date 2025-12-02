using TMPro;
using UnityEngine;

public class IPVisualScript : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    
    private void Start()
    {
        text.text = ConnectionManager.GetLocalIPAddress();
    }
}
