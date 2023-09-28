using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    public void Play()
    {
        Debug.Log("Play");
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        this.gameObject.SetActive(false);
        // SceneManager.LoadScene("niveau_01", LoadSceneMode.Single);
    }
    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
