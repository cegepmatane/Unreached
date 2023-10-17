using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private GameObject HeartPanel, HeartFill;
    [SerializeField] private GameObject MagicPanel, MagicFill;

    private float HeartFillValue, MagicFillValue;

    // Start is called before the first frame update
    void Start()
    {
        setHealth(100);
        setMagic(0);
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
    }

    public void setHealth(int health)
    {
        float AvailableSpace = HeartPanel.GetComponent<RectTransform>().rect.width-10;
        Debug.Log(AvailableSpace);
        HeartFillValue = AvailableSpace * health / 100;
        Debug.Log(HeartFillValue);
    }

    public void setMagic(int magic)
    {
        float AvailableSpace = MagicPanel.GetComponent<RectTransform>().rect.width-10;
        Debug.Log(AvailableSpace);
        MagicFillValue = AvailableSpace * magic / 100;
        Debug.Log(MagicFillValue);
    }
}
