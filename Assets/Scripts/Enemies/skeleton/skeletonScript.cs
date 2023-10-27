using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class SkeletonScript : MonoBehaviour
{
    public int health = 1;
    public bool isFacingLeft = false;
    public int detectionRange = 20;
    public int speed = 50;
    public int getKnockedBack = 10;
    public int lifes = 2;
    public float takeDamageBuffer = 0.5f;

    public int numberOfSplashes = 5;
    public GameObject bloodSplashPrefab;
    public GameObject skeletonParts;
    public GameObject magicPrefab;
    public float bloodSplashForce = 5;
    public float randomDirectionRange = 30;
    public float randomForceMultiplier = 0.5f;

    public bool hasJustLostALife = false;
    private int storedHealth;
    private float randomScale;

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
        randomScale = Random.Range(0.8f, 1.2f);
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(randomScale, randomScale, randomScale));
        // Health based on scale
        health = (int)(health * randomScale);

        // Speed inversely proportional to scale
        float randomSpeed = 1 / randomScale;
        speed = (int)(speed * randomSpeed);

        // AttackSpeed inversely proportional to scale
        float randomAttackSpeed = 1 / (randomScale-0.2f);
        animator.SetFloat("AttackSpeed", randomAttackSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        DeathIfFall();

        if (currentState == State.Dead || currentState == State.Stuned || currentState == State.Attack || Time.timeScale < 1)
        {
            return;
        }

        CheckPlayerDistance();

        // If farther than 2 units away from the target, move towards it
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // ground detection distance
        float raycastDistance = transform.localScale.y * 0.35f;
        float selfxVelocity = rb.velocity.x;

        if (!isPlayerNoticed)
        {
            if (isFacingLeft && selfxVelocity > 0 || !isFacingLeft && selfxVelocity < 0)
            {
                Debug.Log("Flipping");
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
                isFacingLeft = !isFacingLeft;
            }
        }
        else
        {
            // player is noticed, face the player
            if (!isFacingLeft && player.transform.position.x < transform.position.x || isFacingLeft && player.transform.position.x > transform.position.x)
            {
                Debug.Log("Flipping");
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
                isFacingLeft = !isFacingLeft;
            }
        }


        // get current target position
        Vector2 t_CurrentTargetPos = currentTarget.transform.position;
        if (Vector2.Distance(transform.position, t_CurrentTargetPos) > 2 && currentState != State.Shielded && currentState != State.Attack)
        {
            Vector2 direction = (t_CurrentTargetPos - (Vector2)transform.position).normalized;
            rb.AddForce(direction * speed - rb.velocity, ForceMode2D.Force);

            RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, LayerMask.GetMask("Ground"));

            if (ground.collider == null)
            {
                // If there is no ground, fall
                Debug.DrawRay(transform.position, Vector2.down * raycastDistance, Color.green);
                rb.velocity = new Vector2(0, -speed*2);
                // If that works that works ok?!
            }
            else
            {
                Debug.DrawRay(transform.position, Vector2.down * raycastDistance, Color.red);
            }
        }
        else if (isPlayerNoticed)
        {
            rb.velocity = Vector2.zero;
            float randomAttack = Random.Range(0f, 1.2f);
            
            if (player.transform.position.y > transform.position.y + transform.localScale.y * 0.5f + player.transform.localScale.y * 0.5f) // If the player is above me (taking account of both scales), play the attack animation 2
            {
                Debug.Log("Attack2 UP");
                animator.SetTrigger("Attack2");
                currentState = State.Attack;
            }
            else if (randomAttack < 0.7f)
            {
                Debug.Log("Attack");
                animator.SetTrigger("Attack");
                currentState = State.Attack;
            }
            else
            {
                Debug.Log("Attack2");
                animator.SetTrigger("Attack2");
                currentState = State.Attack;
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

    public void DeathIfFall()
    {
        if (transform.position.y < -10)
        {
            TakeDamage(999999);
        }   
    }

    public void TakeDamage(int damage)
    {
        if (currentState == State.Dead)
            return;

        if (damage > storedHealth*2)
            Explode();
       

        Vector2 attackerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector2 knockbackDirection = (transform.position - (Vector3)attackerPosition).normalized;
        
        health -= damage;
        if (!hasJustLostALife)
            animator.SetTrigger("TakeHit");

        if (health <= 0)
        {
            animator.SetBool("IsDead", true);
            //StartCoroutine(TakeDamageBuffer());

            //Coroutine to revive after 5 seconds
            Debug.Log("Lifes: " + lifes + " Damage: " + damage + " StoredHealth: " + storedHealth);
            if (lifes > 0 && !(damage > 2*storedHealth) && !hasJustLostALife) // If the damage is more than twice the health, don't revive
            {
                lifes--;
                hasJustLostALife = true;
                StartCoroutine(Revive());
                currentState = State.Dead;
            }
            else if (lifes <= 0 && !hasJustLostALife)
            {
                Explode();
            }
        }
        else
        {
            // Apply knockback effect
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForce(knockbackDirection * getKnockedBack, ForceMode2D.Impulse);
            // state stuned

            if (currentState != State.Dead)
            {
                currentState = State.Stuned;
                StartCoroutine(UnStund());
            }
        }
        BloodSplatterEffect(knockbackDirection);
    }

    public void Explode()
    {
        currentState = State.Dead;
        // Spawn skeleton parts
        GameObject skeletonPartsInstance = Instantiate(skeletonParts, transform.position, Quaternion.identity);
        // Apply the same scale to the skeleton parts
        skeletonPartsInstance.transform.localScale = skeletonPartsInstance.transform.localScale * randomScale;
        // Flip it the same way as the enemy
        if (isFacingLeft)
        {
            skeletonPartsInstance.transform.localScale = Vector3.Scale(skeletonPartsInstance.transform.localScale, new Vector3(-1, 1, 1));
        }
        // Destroy the skeleton parts after 5 seconds
        Destroy(skeletonPartsInstance, 5f);
        // Spawn magic after 2s
        Instantiate(magicPrefab, transform.position, Quaternion.identity);
        // Add +1 kill to the player
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Kills++;
        Destroy(gameObject);
    }

    public void BloodSplatterEffect(Vector2 hitDirection)
    {
        for (int i = 0; i < numberOfSplashes; i++)
        {
            // Instantiate the blood splash sprite at the player's position
            GameObject bloodSplash = Instantiate(bloodSplashPrefab, transform.position, Quaternion.identity);

            // Get the Rigidbody2D component of the blood splash
            Rigidbody2D bloodSplashRb = bloodSplash.GetComponent<Rigidbody2D>();

            // Apply randomness to the hit direction and force
            float randomAngle = Random.Range(-randomDirectionRange, randomDirectionRange);
            float randomForce = bloodSplashForce * Random.Range(1 - randomForceMultiplier, 1 + randomForceMultiplier);
            Vector2 randomDirection = Quaternion.Euler(0, 0, randomAngle) * hitDirection;

            // Apply a force to the blood splash in the random hit direction
            bloodSplashRb.AddForce(randomDirection.normalized * randomForce, ForceMode2D.Impulse);

            // Set the blood splash to look at the target position based on the random hit direction
            Vector3 targetPosition = transform.position - (Vector3)randomDirection;
            targetPosition.z = bloodSplash.transform.position.z;
            bloodSplash.transform.up = targetPosition - bloodSplash.transform.position;
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
        if (currentState == State.Stuned)
            currentState = State.Idle;
    }

    IEnumerator Revive()
    {
        // Random time between 5 and 10 seconds
        float reviveTime = UnityEngine.Random.Range(5f, 10f);
        yield return new WaitForSeconds(reviveTime);
        animator.SetBool("IsDead", false);
        hasJustLostALife = false;
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
