using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{
    public GameObject player;
    public GameObject trailPrefab;
    public float spawnRate = 0.01f;
    public float stepsPerUnit = 5;

    private Vector3 previousPosition;
    private float timeSinceLastSpawn;
    private List<GameObject> trailSprites = new List<GameObject>();

    private void Start()
    {
        previousPosition = player.transform.position;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnRate)
        {
            SpawnTrail();
            timeSinceLastSpawn = 0;
        }
    }

    void SpawnTrail()
    {
        float distance = Vector3.Distance(previousPosition, player.transform.position);
        int interpolationSteps = Mathf.Max(1, Mathf.RoundToInt(distance * stepsPerUnit));

        for (int i = 1; i <= interpolationSteps; i++)
        {
            float t = (float)i / interpolationSteps;
            Vector3 interpolatedPosition = Vector3.Lerp(previousPosition, player.transform.position, t);
            GameObject trailSprite = SpawnSpriteAt(interpolatedPosition);
            trailSprites.Add(trailSprite);
        }
        previousPosition = player.transform.position;
    }

    GameObject SpawnSpriteAt(Vector3 position)
    {
        GameObject trail = Instantiate(trailPrefab, position, Quaternion.identity);
        trail.GetComponent<SpriteRenderer>().sprite = player.GetComponent<SpriteRenderer>().sprite;
        trail.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder - 1;
        trail.transform.localScale = player.transform.localScale;
        // Reduce opacity by 50% (one line)
        trail.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        return trail;
    }

    private void OnEnable()
    {
        previousPosition = player.transform.position;
    }
}


