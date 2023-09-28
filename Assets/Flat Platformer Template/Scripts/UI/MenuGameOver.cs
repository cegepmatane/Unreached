using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameOver : MonoBehaviour
{
    private void Update()
    {
        // if the player presses enter, restart the level
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RestartLevel();
        }
    }
   
    private void OnEnable()
    {
        Time.timeScale = 0.2f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void RestartLevel()
    {
        Debug.Log("Restart");
        this.gameObject.SetActive(false);
        SceneManager.LoadScene("niveau_01", LoadSceneMode.Single);
    }
}
