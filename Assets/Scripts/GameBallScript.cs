using FishNet.Object;
using UnityEngine;

public class GameBallScript : NetworkBehaviour
{
    public BallType ballType;
    public int ballIndex;
    public int ballPoints;
    
    private SoundPlayer _soundPlayer;

    private void Awake()
    {
        _soundPlayer = GetComponent<SoundPlayer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _soundPlayer.PlayRandomGlobal("BallCollision");
        }
    }
}
