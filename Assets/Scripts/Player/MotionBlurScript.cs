using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlurScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Si l'opacité est plus petite que 0.1
        if (GetComponent<SpriteRenderer>().color.a < 0.01f)
        {
            Destroy(gameObject);
        }
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, GetComponent<SpriteRenderer>().color.a - 0.05f);
    }
}
