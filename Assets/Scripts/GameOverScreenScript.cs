using TMPro;
using UnityEngine;

public class GameOverScreenScript : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    
    [SerializeField] private TMP_Text gameWonText;
    [SerializeField] private TMP_Text gameLostText;
    
    private void OnEnable()
    {
        GameplayManager.OnGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        GameplayManager.OnGameOver -= ShowGameOver;
    }

    private void ShowGameOver(bool isWinner)
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);

        if (gameWonText != null)
            gameWonText.gameObject.SetActive(isWinner);

        if (gameLostText != null)
            gameLostText.gameObject.SetActive(!isWinner);
    }
}
