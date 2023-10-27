using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{

    public float DistanceFromPlayerUntilSpawn;
    public float TimeBetweenSpawns;

    public int NumberMinOfMonstersPerSpawn;
    public int NumberMaxOfMonstersPerSpawn;
    public int MaxNumberOfMonstersAlive;

    public GameObject MonsterPrefab;
    public GameObject SpawnParticles;
    private GameObject Player;

    private float m_TimeSinceLastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        m_TimeSinceLastSpawn = 0;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        m_TimeSinceLastSpawn += Time.deltaTime;

        GameObject[] listOfAllEnnemies = GameObject.FindGameObjectsWithTag("Enemy");
        int numberOfCloseEnnemies = 0;

        foreach (GameObject ennemy in listOfAllEnnemies)
        {
            // If the enemy is closer than twice the distance from the player until spawn, count it
            if (Vector3.Distance(ennemy.transform.position, transform.position) < DistanceFromPlayerUntilSpawn)
            {
                numberOfCloseEnnemies++;
            }
        } 
        Debug.Log(numberOfCloseEnnemies);

        if (m_TimeSinceLastSpawn > TimeBetweenSpawns)
        {
            m_TimeSinceLastSpawn = 0;
            if (IsPlayerInReach())
            {
                SpawnEnnemies();
            }
        }
    }

    bool IsPlayerInReach()
    {
        return Vector3.Distance(Player.transform.position, transform.position) < DistanceFromPlayerUntilSpawn;
    }

    void SpawnEnnemies()
    {
        // Spawn particles
        Instantiate(SpawnParticles, transform.position, Quaternion.identity);

        // Spawn ennemies in a circle around the spawner
        int numberOfMonsters = Random.Range(NumberMinOfMonstersPerSpawn, NumberMaxOfMonstersPerSpawn);
        for (int i = 0; i < numberOfMonsters; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 spawnPosition = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 2;
            // if lower than the position of the spawner, put it at the same height
            if (spawnPosition.y < transform.position.y + 5)
            {
                spawnPosition.y = transform.position.y + 5;
            }
            Instantiate(MonsterPrefab, spawnPosition, Quaternion.identity);
        }

    }
}
