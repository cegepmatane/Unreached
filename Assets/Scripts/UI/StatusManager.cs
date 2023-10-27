using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private GameObject HeartPanel, HeartFill;
    [SerializeField] private GameObject MagicPanel, MagicFill;
    [SerializeField] private GameObject KillsText;
    [SerializeField] private GameObject VictoryScreen;

    [SerializeField] private GameObject KillsTextVictory;
    [SerializeField] private GameObject TimeTextVictory;

    private float HeartFillValue, MagicFillValue;
    private int Kills;
    private float timeSinceStart = 0;

    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        setHealth(100);
        setMagic(0);
        setKills(0);
    }

    private void FixedUpdate()
    {
        // Fill the heartbar with animation
        RectTransform rt = HeartFill.GetComponent<RectTransform>();
        if (rt.offsetMax != new Vector2(HeartFillValue, rt.offsetMax.y))
        {
            rt.offsetMax = Vector2.Lerp(rt.offsetMax, new Vector2(HeartFillValue, rt.offsetMax.y), 0.1f);
        }
        RectTransform rt2 = MagicFill.GetComponent<RectTransform>();
        if (rt2.offsetMax != new Vector2(MagicFillValue, rt2.offsetMax.y))
        {
            rt2.offsetMax = Vector2.Lerp(rt2.offsetMax, new Vector2(MagicFillValue, rt2.offsetMax.y), 0.1f);
        }
        if (Kills >= 10 && !isGameOver)
        {
            VictoryScreen.SetActive(true);
            KillsTextVictory.GetComponent<TMPro.TextMeshProUGUI>().text = Kills.ToString();
            TimeSpan time = TimeSpan.FromSeconds(timeSinceStart);
            TimeTextVictory.GetComponent<TMPro.TextMeshProUGUI>().text = time.ToString(@"mm\:ss");
            isGameOver = true;
        }
        timeSinceStart += Time.deltaTime;
    }

    public void setHealth(int health)
    {
        float AvailableSpace = HeartPanel.GetComponent<RectTransform>().rect.width-10;
        if (health > 100) health = 100;
        else if (health < 0) health = 0;
        HeartFillValue = AvailableSpace * health / 100;
    }

    public void setMagic(int magic)
    {
        float AvailableSpace = MagicPanel.GetComponent<RectTransform>().rect.width-10;
        if (magic > 100) magic = 100;
        else if (magic < 0) magic = 0;
        MagicFillValue = AvailableSpace * magic / 100;
    }

    internal void setKills(int kills)
    {
        Kills = kills;
        KillsText.GetComponent<TMPro.TextMeshProUGUI>().text = Kills.ToString();
    }
}
