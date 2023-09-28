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
    public Menu Menu;

    private void Awake()
    {
        Instance = this;
    }
}
