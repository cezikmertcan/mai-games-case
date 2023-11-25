using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject EndGamePanel;
    public TMP_Text LevelText;
    public TMP_Text TitleText;
    public TMP_Text ButtonText;
    private void Awake()
    {
        C.uiManager = this;
    }

    private void Start()
    {
        LevelText.text = "Level " + C.Level.ToString();
    }

    public void OpenEndGamePanel()
    {
        EndGamePanel.SetActive(true);
        if (C.gameManager.LOST)
        {
            TitleText.text = "YOU LOST";
            ButtonText.text = "Try Again";
        }
        else
        {
            TitleText.text = "YOU WIN";
            ButtonText.text = "Next Level";
        }
    }

    public void OnNextLevelButtonClick()
    {

        if (C.gameManager.LOST)
        {

        }
        else
        {
            C.Level++;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}
