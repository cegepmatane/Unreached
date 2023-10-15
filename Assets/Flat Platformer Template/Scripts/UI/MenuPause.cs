using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{   
    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    
    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    public void Resume()
    {
        Debug.Log("Resume");
        this.gameObject.SetActive(false);
    }

    public void MainMenu()
    {
        Debug.Log("MainMenu");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
