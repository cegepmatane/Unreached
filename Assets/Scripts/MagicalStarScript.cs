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
    private bool hasExploded = false;

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

            // If an enemy is in the original size of the star, deal damage
            // Spawn a bunch of raycasts in a circle
            if (!hasExploded) {
                for (int i = 0; i < 360; i += 10)
                {
                    float t_RaycastSize = originalSize * 0.8f;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, i) * Vector2.up, t_RaycastSize, LayerMask.GetMask("Enemies"));
                    if (hit.collider != null)
                    {
                        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * Vector2.up * t_RaycastSize, Color.red, 5f);
                        try
                        {
                            hit.collider.GetComponent<SkeletonScript>().TakeDamage(damages);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, i) * Vector2.up * t_RaycastSize, Color.green, 5f);
                    }
                }
            }
            hasExploded = true;
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
