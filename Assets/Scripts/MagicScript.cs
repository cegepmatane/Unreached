using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MagicScript : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public int value = 20;

    private Vector2 originalPos;
    private float elapsedTime;
    private bool isSpawning = true;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1, 1, 1, 0);
        // Scale bonus based on value
        transform.localScale = new Vector3(transform.localScale.x * (value / 20), transform.localScale.y * (value / 20), transform.localScale.z * (value / 20));
        // Move magic to the ground
        transform.position = new Vector2(transform.position.x, transform.position.y - 3);
    }

    // Update is called once per frame
    void Update()
    {
        // if opacity is less than 1, increase opacity
        if (spriteRenderer.color.a < 1)
        {
            spriteRenderer.color = new Color(1, 1, 1, spriteRenderer.color.a + 0.5f * Time.deltaTime);
        }

        if (isSpawning)
        {
            // if magic is spawning, move it up
            //transform.position = new Vector2(transform.position.x, transform.position.y + 2f * Time.deltaTime);
            transform.position = Vector2.Lerp(transform.position, originalPos, 2f * Time.deltaTime);
            if (transform.position.y >= originalPos.y-0.1)
            {
                isSpawning = false;
            }
            return;
        }

        elapsedTime += Time.deltaTime;
        float newY = originalPos.y + Mathf.Sin(elapsedTime * frequency) * amplitude;
        transform.position = new Vector2(originalPos.x, newY);

        // Rotate the object around its local Y axis at 1 degree per second
        transform.Rotate(0, Time.deltaTime * 100, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // if player collides with magic, destroy magic and increase player's magic
            Destroy(gameObject);
            collision.gameObject.GetComponent<PlayerController>().GatherMagic(value);
        }
    }
}
