using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       //Randomize size by 20%
       float randomSize = Random.Range(0.2f, 1.4f);
       transform.localScale = transform.localScale * randomSize;
    }


    // If i touch the ground, destroy myself
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Tilemap")
        {
            Destroy(gameObject);
        }
    }
}
