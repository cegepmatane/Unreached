using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameOver : MonoBehaviour
{
    private bool activateFadeOut = false;
    private bool activateFadeRestart = false;
    private bool activateFadeMainMenu = false;

    [SerializeField]
    private GameObject m_BlackoutPanel;
    [SerializeField]
    private GameObject m_RestartButton;
    [SerializeField]
    private GameObject m_MainMenuButton;

    private void Update()
    {
        // if the player presses enter, restart the level
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RestartLevel();
        }
        if (activateFadeOut)
            m_BlackoutPanel.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, m_BlackoutPanel.GetComponent<UnityEngine.UI.Image>().color.a + 0.01f);

        if (activateFadeRestart)
            m_RestartButton.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, m_RestartButton.GetComponent<UnityEngine.UI.Image>().color.a + 0.01f);

        if (activateFadeMainMenu)
            m_MainMenuButton.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, m_MainMenuButton.GetComponent<UnityEngine.UI.Image>().color.a + 0.01f);
    }
   
    private void OnEnable()
    {
        Time.timeScale = 0.2f;
        StartCoroutine(FadeOut());
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.5f);
        activateFadeOut = true;
        StartCoroutine(WaitRestart());
    }

    IEnumerator WaitRestart()
    {
        yield return new WaitForSeconds(0.5f);
        m_RestartButton.SetActive(true);
        activateFadeRestart = true;
        StartCoroutine(WaitMainMenu());
    }

    IEnumerator WaitMainMenu()
    {
        yield return new WaitForSeconds(0.5f);
        activateFadeMainMenu = true;
        m_MainMenuButton.SetActive(true);
    }

    public void RestartLevel()
    {
        Debug.Log("Restart");
        this.gameObject.SetActive(false);
        activateFadeOut = false;
        activateFadeRestart = false;
        activateFadeMainMenu = false;
        SceneManager.LoadScene("level_0", LoadSceneMode.Single);
    }

    public void MainMenu()
    {
        Debug.Log("MainMenu");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
