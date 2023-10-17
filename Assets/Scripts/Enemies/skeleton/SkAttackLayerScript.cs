using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkAttackLayerScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collision detected " + col.gameObject.tag);
        if (col.gameObject.tag == "Player")
        {
            Debug.Log("Player takes damage");
            col.gameObject?.GetComponent<PlayerController>()?.TakeDamage(20);
        }
    }
}
