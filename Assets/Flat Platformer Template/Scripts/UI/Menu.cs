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
        this.gameObject.SetActive(false);
        SceneManager.LoadScene("niveau_0", LoadSceneMode.Single);
        // SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
