using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
public class GameManager : MonoBehaviour
{
    public int score;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject errorText;

    [SerializeField] private GameObject nameField;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private GameObject closeupCameraRig;
    [SerializeField] private Camera mainCamera;

// ENCAPSULATION
    private string _playerName;
    public string playerName
    {
        get { return _playerName;}
        set 
        {
            if (value.GetType() != typeof(string))
            {
               Debug.Log("Strings only!"); 
               return;
            }
            else if (value.Length < 1 || value.Length > 8) 
            {
                Debug.Log("Name must contain at least 1 char and no more than 8!");
                errorText.GetComponent<TMP_Text>().text = "Name must contain at least 1 char and no more than 8!";
                return;
            }
            _playerName = value;
        }

    }
    public bool gameIsActive;
    public static event Action OnGameOver;
    public static event Action OnStartGame;

    private void Awake()
    {
        gameIsActive = false;
        titlePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        scoreText.gameObject.SetActive(false);
        playerCanvas.gameObject.SetActive(false);
        Time.timeScale = 0; 
    }

    void Start()
    {
        closeupCameraRig.SetActive(true);
        mainCamera.gameObject.SetActive(false);
    }

// ABSTRACTION
    public void UpdateScore(int scoreChange)
    {
        score += scoreChange;
        scoreText.text = playerName + ": <br>" + score.ToString();
    }

    public void LoadTitleScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        gameIsActive = false;
        
        if (OnGameOver != null)
        {
            OnGameOver();
        }
        
        gameOverPanel.SetActive(true);
    }

    public void StartGame()
    {
        if (OnStartGame != null){
            OnStartGame();
        }

        playerName = nameField.GetComponent<TMP_InputField>().text;

        if (playerName != null)
        {
            gameIsActive = true;
            Time.timeScale = 1;
            titlePanel.SetActive(false);

            UpdateScore(0);

            scoreText.gameObject.SetActive(true);
            playerCanvas.gameObject.SetActive(true);

            mainCamera.gameObject.SetActive(true);
            closeupCameraRig.SetActive(false);
        }        
    }

    public void HidePlaceholder()
    {
        nameField.GetComponent<TMP_InputField>().placeholder.gameObject.SetActive(false);
    }

    public void ShowPlaceholder()
    {
        nameField.GetComponent<TMP_InputField>().placeholder.gameObject.SetActive(true);
    }

}
