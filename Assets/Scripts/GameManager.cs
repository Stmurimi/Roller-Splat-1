using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Only one GameManager instance throughout all scenes
    public static GameManager singleton;
    private GroundPiece[] allGroundPieces;
    public Button gameOverButton;
    public GameObject gameOverUI;

    public Text level5Text;
    public GameObject level, ball;



    void Start()
    {
        SetUpNewLevel();
    }

    void SetUpNewLevel()
    {
        allGroundPieces = FindObjectsOfType<GroundPiece>();
    }

    private void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
        }
        else if(singleton != this)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        //Each time a scene is loaded,
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        SetUpNewLevel();
    }

    public void CheckComplete()
    {
        bool isFinished = true;

        for(int i =0; i < allGroundPieces.Length; i++)
        {
            if (allGroundPieces[i].isColored == false)
            {
                isFinished = false;
                gameOverUI.gameObject.SetActive(false);
                gameOverButton.gameObject.SetActive(false);
                break;
            }
        }

        if (isFinished)
        {
            NextLevel();
        }
    }

    private void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            //End Game and restart on click
            RestartGame();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void RestartGame()
    {
        level5Text.gameObject.SetActive(false);
        level.gameObject.SetActive(false);
        ball.gameObject.SetActive(false);

        gameOverUI.gameObject.SetActive(true);
        gameOverButton.gameObject.SetActive(true);

        gameOverButton.onClick.AddListener(Restart);
        
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
