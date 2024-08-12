using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject EndScreen;
    
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
}
