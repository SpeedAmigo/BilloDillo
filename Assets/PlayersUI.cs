using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

public class PlayersUI : NetworkBehaviour
{
    [SerializeField] private Image[] playerOneBills;
    [SerializeField] private Image[] playerTwoBills;
    
    [SerializeField] private Sprite[] ballSprites;
    

    private void OnEnable()
    {
        GameplayManager.OnBallImage += AddBallImage;
    }

    private void OnDisable()
    {
        GameplayManager.OnBallImage -= AddBallImage;
    }

    /*[Server]
    private void AddBallImageServer(Sprite sprite, int index)
    {
        AddBallImageLogic(sprite, index);
        AddBallImageClient(sprite, index);
    }

    [ObserversRpc(BufferLast = true)]
    private void AddBallImageClient(Sprite sprite, int index)
    {
        AddBallImageLogic(sprite, index);
    }*/
    
    private void AddBallImage(int spriteIndex, int index)
    {
        Image[] target = null;
        if (index == 0) target = playerOneBills;
        else if (index == 1) target = playerTwoBills;
        else
        {
            Debug.LogWarning($"PlayersUI: invalid player index {index}");
            return;
        }

        if (target == null || target.Length == 0)
        {
            Debug.LogWarning($"PlayersUI: no image array assigned for player {index}");
            return;
        }

        foreach (var img in target)
        {
            if (img == null) continue;
            if (!img.gameObject.activeSelf)
            {
                img.sprite = ballSprites[spriteIndex];
                img.gameObject.SetActive(true);
                return;
            }
        }

        Debug.LogWarning($"PlayersUI: no available (disabled) image slot for player {index}");
    }
}
