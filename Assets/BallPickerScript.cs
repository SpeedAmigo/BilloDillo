using FishNet.Object;
using UnityEngine;

public class BallPickerScript : NetworkBehaviour
{
    [SerializeField] private GameObject[] ballVisuals;

    public void ChangeVisual(int index)
    {
        for (int i = 0; i < ballVisuals.Length; i++)
        {
            ballVisuals[i].SetActive(i == index);
        }
    }
}
