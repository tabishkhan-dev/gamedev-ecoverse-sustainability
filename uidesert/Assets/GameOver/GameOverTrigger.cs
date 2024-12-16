using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    public Animator gameOverAnimator; // Reference to the Animator
    public KeyCode triggerKey = KeyCode.G; // Test trigger key (G for Game Over)

    void Update()
    {
        // For testing: Press G to play the Game Over animation
        if (Input.GetKeyDown(triggerKey))
        {
            PlayGameOverAnimation();
        }
    }

    public void PlayGameOverAnimation()
    {
        gameOverAnimator.SetTrigger("PlayGameOve");
        Debug.Log("Game Over Animation Triggered");
    }
}
