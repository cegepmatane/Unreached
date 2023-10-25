using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceScript : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player collides with the ice, then the player will slide
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().isSlidingOnIce = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // If the player exits the ice, then the player will stop sliding
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().isSlidingOnIce = false;
        }
    }
}
