using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int score;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject nameField;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private GameObject closeupCameraRig;
    [SerializeField] private Camera mainCamera;

    private string playerName;

    public bool gameIsActive;

    private float closeupRotateSpeed = .25f;

    private void Awake()
    {
        gameIsActive = false;
        titlePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        scoreText.gameObject.SetActive(false);
        playerCanvas.gameObject.SetActive(false);
        Time.timeScale = 0; 
    }

    // Start is called before the first frame update
    void Start()
    {
        closeupCameraRig.SetActive(true);
        mainCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsActive)
        {
            closeupCameraRig.transform.Rotate(Vector3.up * closeupRotateSpeed);
        }
    }

    public void UpdateScore(int scoreChange)
    {
        score += scoreChange;
        scoreText.text = playerName + ": <br>" + score.ToString();
    }

    public void TitleScreen()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        gameIsActive = false;
        gameOverPanel.SetActive(true);
    }

    public void StartGame()
    {
        gameIsActive = true;
        Time.timeScale = 1;
        titlePanel.SetActive(false);

        playerName = nameField.GetComponent<TMP_InputField>().text;
        UpdateScore(0);

        scoreText.gameObject.SetActive(true);
        playerCanvas.gameObject.SetActive(true);

        mainCamera.gameObject.SetActive(true);
        closeupCameraRig.SetActive(false);
        
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
