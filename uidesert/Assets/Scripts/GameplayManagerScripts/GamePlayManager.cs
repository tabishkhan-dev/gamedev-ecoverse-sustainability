using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to Game Menu...");
        SceneManager.LoadScene("Game menu"); // Ensure this matches your Game menu scene name
    }
}