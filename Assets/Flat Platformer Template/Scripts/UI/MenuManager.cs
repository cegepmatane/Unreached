using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public HUD HUD;
    public MenuPause Pause;
    public MenuGameOver GameOver;

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Pause.gameObject.activeSelf)
            {
                Pause.gameObject.SetActive(false);
            }
            else
            {
                Pause.gameObject.SetActive(true);
            }
        }
    }
}
