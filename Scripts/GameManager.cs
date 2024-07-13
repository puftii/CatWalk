using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void OnEnable()
    {
        EventManager.PlayerDied += EndGame;
    }

    void OnDisable()
    {
        EventManager.PlayerDied -= EndGame;
    }

    void Start()
    {
        EventManager.PlayerDied += EndGame;
    }

    void EndGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
