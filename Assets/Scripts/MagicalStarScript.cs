using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalStarScript : MonoBehaviour
{
    public float shrinkSpeed = 1.0f;
    private float originalSize;

    void Start()
    {
       originalSize = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        // if size is less than 1, self destruct
        if (transform.localScale.x < 0.01)
        {
            //Destroy(gameObject);
        }

        // Decrease size using an inverse function and sync with Time.deltaTime
        // smaller the size, faster it shrinks
        float shrinkFactor = (1 / (transform.localScale.x + 1)) * shrinkSpeed * Time.deltaTime;
        if (transform.localScale.x < originalSize*0.95)
        {
            transform.localScale -= Vector3.one * shrinkFactor * 10;
        }
        else
        {
            transform.localScale -= Vector3.one * shrinkFactor;
        }
    
    }
}
