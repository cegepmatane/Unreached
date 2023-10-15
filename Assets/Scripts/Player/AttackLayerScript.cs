using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLayerScript : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            Debug.Log("SWORD HIT ENEMY");
            col.gameObject?.GetComponent<skeletonScript>()?.TakeDamage(1);
        }
    }
}
