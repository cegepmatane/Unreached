using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Sentis.Layers;
using UnityEngine;

public class skeletonScript : MonoBehaviour
{
    public int health = 1;
    public bool isFacingRight;
    public int detectionRange = 20;
    public int speed = 50;
    public int getKnockedBack = 10;
    private int storedHealth;

    [SerializeField] private Animator animator;

    public enum State
    {
        Idle,
        Walk,
        Stuned,
        Attack,
        Shielded,
        Dead
    }

    public State currentState = State.Idle;
    public GameObject target;
    private GameObject currentTarget;
    public bool isPlayerNoticed = false;

    void Start()
    {
        storedHealth = health;
        currentTarget = target;

        // Randomize the scale by 20%
        float randomScale = UnityEngine.Random.Range(0.8f, 1.2f);
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(randomScale, randomScale, randomScale));
        // Health based on scale
        health = (int)(health * randomScale);

        // Randomize the speed by 20%
        float randomSpeed = UnityEngine.Random.Range(0.8f, 1.2f);
        speed = (int)(speed * randomSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Dead || currentState == State.Stuned || currentState == State.Attack)
        {
            return;
        }

        CheckPlayerDistance();

        // If farther than 2 units away from the target, move towards it
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector2 playerHorizontalPosition = new Vector2(player.transform.position.x, transform.position.y);
        float raycastDistance = 2f; // Adjust this value based on the enemy's size and desired ground detection distance
        float selfxVelocity = rb.velocity.x;

        if (!isPlayerNoticed)
        {
            if (isFacingRight && selfxVelocity > 0 || !isFacingRight && selfxVelocity < 0)
            {
                Debug.Log("Flipping");
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
                isFacingRight = !isFacingRight;
            }
        }
        else
        {
            // player is noticed, face the player
            if (!isFacingRight && player.transform.position.x < transform.position.x || isFacingRight && player.transform.position.x > transform.position.x)
            {
                Debug.Log("Flipping");
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
                isFacingRight = !isFacingRight;
            }
        }


        if (Vector2.Distance(transform.position, playerHorizontalPosition) > 2)
        {
            Vector2 direction = (playerHorizontalPosition - (Vector2)transform.position).normalized;
            rb.AddForce(direction * speed - rb.velocity, ForceMode2D.Force);
            //animator.SetBool("IsWalking", true);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance);
            if (hit.collider != null)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle > 0)
                {
                    Vector2 slopeDirection = Vector2.Perpendicular(hit.normal).normalized;
                    rb.AddForce(slopeDirection * speed * Mathf.Sin(slopeAngle * Mathf.Deg2Rad), ForceMode2D.Force);
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            currentState = State.Attack;
            float randomAttack = UnityEngine.Random.Range(0f, 1f);
            if (randomAttack < 0.7f)
            {
                Debug.Log("Attack");
                animator.SetTrigger("Attack");
            }
            else
            {
                Debug.Log("Attack2");
                animator.SetTrigger("Attack2");
            }
            StartCoroutine(ResetAttack());
        }

        // If i'm going faster than 0.2 unit per second, play the walking animation
        if (rb.velocity.magnitude > 0.2)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }


    void CheckPlayerDistance()
    {
        // get the distance between the player and the enemy
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distance = Vector3.Distance(transform.position, player.transform.position);

        // if the player is within 5 units of the enemy, set isPlayerNoticed to true
        if (distance < detectionRange)
        {
            isPlayerNoticed = true;
        }
        else
        {
            isPlayerNoticed = false;
        }

        // if the player is noticed, set the target to the player
        if (isPlayerNoticed)
        {
            currentTarget = player;
        }
        else
        {
            currentTarget = target;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == State.Dead)
            return;
        
        health -= damage;
        animator.SetTrigger("TakeHit");
        if (health <= 0)
        {
            animator.SetBool("IsDead", true);
            currentState = State.Dead;
            //Coroutine to revive after 5 seconds
            StartCoroutine(Revive());
        }
        else
        {
            // Apply knockback effect
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 attackerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector2 knockbackDirection = (transform.position - (Vector3)attackerPosition).normalized;
            rb.AddForce(knockbackDirection * getKnockedBack, ForceMode2D.Impulse);
            //state stuned
            currentState = State.Stuned;
            StartCoroutine(UnStund());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(1f);
        if (currentState == State.Dead)
            yield break;
        currentState = State.Idle;
    }

    IEnumerator UnStund()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentState == State.Dead)
            yield break;
        currentState = State.Idle;
    }

    IEnumerator Revive()
    {
        // Random time between 5 and 10 seconds
        float reviveTime = UnityEngine.Random.Range(5f, 10f);
        yield return new WaitForSeconds(reviveTime);
        animator.SetBool("IsDead", false);
        StartCoroutine(ReActivate());
    }

    IEnumerator ReActivate()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Reactivating");
        currentState = State.Idle;
        health = storedHealth;
    }
}
