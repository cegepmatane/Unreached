using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalStarScript : MonoBehaviour
{
    public float shrinkSpeed = 1.0f;
    public GameObject explosionEffect1;
    public GameObject explosionEffect2;
    public GameObject explosionEffect3;
    public GameObject explosionEffect4;

    public int damages = 10;

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
        // if size is less than 1, deactivate the 
        if (transform.localScale.x < originalSize * 0.1)
        {
            // Instantiate explosion effect
            Instantiate(explosionEffect1, transform.position, Quaternion.identity);
            Instantiate(explosionEffect2, transform.position, Quaternion.identity);
            GameObject electricalEffect = Instantiate(explosionEffect3, transform.position, Quaternion.identity);
            Instantiate(explosionEffect4, transform.position, Quaternion.identity);
            // Destroy the star after 0.01s
            Destroy(gameObject, 0.2f);
            Destroy(electricalEffect, 1.5f);

            // If an enemy "tag == Ennemy" is in the original size of the star, deal damage
            Collider[] colliders = Physics.OverlapSphere(transform.position, originalSize);
            foreach (Collider nearbyObject in colliders)
            {
                if (nearbyObject.tag == "Enemy")
                {
                    nearbyObject.GetComponent<SkeletonScript>().TakeDamage(damages);
                }
            }
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
