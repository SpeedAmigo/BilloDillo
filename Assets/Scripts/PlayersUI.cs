using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayersUI : NetworkBehaviour
{
    [SerializeField] private Image[] playerOneBills;
    [SerializeField] private Image[] playerTwoBills;
    
    [SerializeField] private TMP_Text playerOnePoints;
    [SerializeField] private TMP_Text playerTwoPoints;
    
    [SerializeField] private Sprite[] ballSprites;
    
    private void OnEnable()
    {
        GameplayManager.OnBallImage += AddBallImage;
        GameplayManager.OnPointsUpdate += UpdatePlayerPoints;
    }

    private void OnDisable()
    {
        GameplayManager.OnBallImage -= AddBallImage;
        GameplayManager.OnPointsUpdate -= UpdatePlayerPoints;
    }

    private void UpdatePlayerPoints(int value, int playerIndex)
    {
        if (playerIndex == 0)
        {
            playerOnePoints.text = value.ToString();
        }
        else if (playerIndex == 1)
        {
            playerTwoPoints.text = value.ToString();
        }
    }
    
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
