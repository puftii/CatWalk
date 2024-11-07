using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject EndScreen;
    public GameObject PauseScreen;
    public KeyCode EscapeButton;
    static public bool GamePaused;
    private ThirdPersonController _player;

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
        _player = FindFirstObjectByType<ThirdPersonController>();
        PauseGame(PauseScreen.activeSelf);

    }


    private void Update()
    {
        if (Input.GetKeyDown(EscapeButton))
        {
            PauseGame(!GamePaused);
        }
    }

    public void EndGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Died");
        //EndScreen = GameObject.Find("EndScreen");
        if (EndScreen != null)
        {
            EndScreen.SetActive(true);
        }
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }


    public void Quit()
    {
        Application.Quit();
    }



    public void PauseGame(bool state)
    {
        if (state)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0.0f;
            GamePaused = true;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1.0f;
            GamePaused = false;
        }
        _player.enabled = !GamePaused;
        PauseScreen.SetActive(GamePaused);
    }
}
