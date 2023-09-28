using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    [SerializeField]
    private TextMeshProUGUI m_TextHP;

    //public TextMeshProUGUI TextHP;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateHealth(int a_HP)
    {
        Debug.Log("UpdateHP: " + a_HP);
        m_TextHP.text = a_HP + "HP";
    }
}
