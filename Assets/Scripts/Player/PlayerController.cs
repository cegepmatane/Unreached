using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float RunSpeed = 3f;
    public float RunSpeedIncrease = 1.3f;
    public float RunSpeedDescrease = 0.1f;
    public float MaxRunSpeedVelocity = 20f;
    public float QuickJumpSpeedIncrease = 0.2f;
    public float QuickJumpForceDecrease = 4f;
    public float WalkSpeed = 1f;

    public float JumpForce = 10f;
    public int JumpCount = 2;

    public float SlideForceBoost = 1.2f;
    public float SlideForceDamping = 0.3f;

    public int Health = 100;

    public float AttackWaitTime = 0.5f;
    public float AttackFallSpeed = 0.2f;

    public GameObject MagicalStarPrefab;
    public GameObject PlayerBlur;

    private bool m_UserJump = false;
    private bool m_UserMagicalAbility = false;
    private bool m_UserQuickJump = false;
    private bool m_SecondaryJump = false;
    private bool m_IsGrounded = false;
    private bool m_IsSliding = false;
    private bool m_IsAttacking = false;
    private bool m_EdgeGrab = false;
    private bool m_AttackCombo = false;
    private bool m_UserStoredAttack = false;
    private float m_StoredRunSpeed;
    private int m_StoredJumpCount;
    private Vector2 m_StoredVelocity;
    private bool m_IsJumping = false;

    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2D;

    [SerializeField]
    private Transform m_FeetPos, m_HeadSlidePos, m_WallRideR, m_CeilingRideU;
    [SerializeField]
    private LayerMask m_GroundLayer;
    [SerializeField]
    private GameObject m_GameOverScren;
    [SerializeField]
    private GameObject m_StatusManager;

    private void Awake()
    {
        m_Animator = this.gameObject.GetComponent<Animator>();
        m_Rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_StoredRunSpeed = RunSpeed;
        m_StoredJumpCount = JumpCount;
        m_StoredVelocity = m_Rigidbody2D.velocity;
    }

    // Update is called once per frame
    private void Update()
    {
        // Jump input
        if (Input.GetButtonDown("Jump") && !m_IsSliding && m_StoredJumpCount > 0) { // if the player is not sliding and has jump left
            if (m_StoredJumpCount != JumpCount)
                m_SecondaryJump = true; // if the player is jumping for the second time, it's a secondary jump
            m_StoredJumpCount--;
            m_UserJump = true;
            m_Animator.SetTrigger("Jump");
        }
        else if (Input.GetButtonDown("Jump") && m_IsSliding && m_StoredJumpCount > 0) // if the player is sliding and has jump left
        {
            if (m_StoredJumpCount != JumpCount)
                m_SecondaryJump = true;
            m_StoredJumpCount--;
            m_UserQuickJump = true;
            m_Animator.SetTrigger("QuickJump");
        }


        // Attack input
        if ((Input.GetButtonDown("Fire1") || m_UserStoredAttack) && !m_IsSliding && !m_IsAttacking && !m_EdgeGrab && !m_AttackCombo) { // if the player is not sliding and is not attacking
            Debug.Log("Attack");
            m_Animator.SetTrigger("Attack");
            m_Animator.SetBool("IsNotACombo", false);
            m_IsAttacking = true;
            m_UserStoredAttack = false;
            StartCoroutine(ResetIsAttacking());
        }
        else if (m_IsAttacking && Input.GetButtonDown("Fire1") && !m_AttackCombo)
        {
            Debug.Log("AttackCombot");
            m_AttackCombo = true;
        }
        else if (m_AttackCombo && Input.GetButtonDown("Fire1") && !m_UserStoredAttack)
        {
            Debug.Log("Attack on wait");
            m_UserStoredAttack = true;
        }

        // Magical ability input
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("MagicalAbility");
            m_UserMagicalAbility = true;
        }


        // If the user touch the void, he dies
        if (transform.position.y < -10) // if the player is below -10 on the Y axis
            TakeDamage(99999);
        
    }

    void IsACombo()
    {
        if (!m_AttackCombo)
        {
            m_IsAttacking = false;
            m_Animator.SetBool("IsNotACombo", true);
            // Stop ResetIsAttacking
            StopCoroutine(ResetIsAttacking());
        }
    }

    IEnumerator ResetIsAttacking()
    {
        //yield return new WaitForSeconds(AttackWaitTime);
        // for 2 seconds, the player Y movement is slowed down to 20%
        float t_Time = 0;
        while (t_Time < AttackWaitTime)
        {
            t_Time += Time.deltaTime;
            float t_Velocity = m_Rigidbody2D.velocity.y * AttackFallSpeed;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, t_Velocity);
            yield return null;
        }
        m_IsAttacking = false;
        m_AttackCombo = false;
    }

    private void FixedUpdate()
    {
        MotionBlur();

        //Debug.Log("RunSpeed: " + RunSpeed);
        //Debug.Log("Velocity: " + m_Rigidbody2D.velocity.x);

        if (m_UserMagicalAbility)
        {
            Debug.Log("MagicalAbility");
            m_UserMagicalAbility = false;
            MagicalStar();
        }

        float t_MoveX = Input.GetAxis("Horizontal");
        float t_SpeedX = Mathf.Abs(m_Rigidbody2D.velocity.x);
        m_Animator.SetFloat("Speed", Mathf.Abs(t_MoveX));

        float t_Force = t_MoveX * RunSpeed;
        if (Mathf.Abs(t_Force) > WalkSpeed) // if the force is greater than the walk speed, clamp it
            t_Force = WalkSpeed * Mathf.Sign(t_Force);
        //Debug.Log("Force: " + t_Force);

        if (!m_IsSliding && !m_IsAttacking && m_IsGrounded)
            m_Rigidbody2D.velocity = new Vector2(t_MoveX * (RunSpeed), m_Rigidbody2D.velocity.y);
            //m_Rigidbody2D.AddForce(new Vector2(t_Force, 0), ForceMode2D.Force);
        else if (!m_IsSliding && m_IsAttacking && m_IsGrounded)
            m_Rigidbody2D.velocity = new Vector2(t_MoveX * (WalkSpeed), m_Rigidbody2D.velocity.y);
        else if (!m_IsSliding && !m_IsAttacking && !m_IsGrounded)
            m_Rigidbody2D.AddForce(new Vector2(t_Force, 0), ForceMode2D.Force);

        // Ground detection
        bool t_previousGrounded = m_IsGrounded;
        Collider2D t_Collider = Physics2D.OverlapCircle(m_FeetPos.position, 0.2f, m_GroundLayer);
        m_IsGrounded = t_Collider != null ? true : false;
        m_Animator.SetBool("IsGrounded", m_IsGrounded);

        // Wall detection
        Collider2D t_WallRCollider = Physics2D.OverlapCircle(m_WallRideR.position, 0.01f, m_GroundLayer);

        // Ceiling detection
        bool t_IsCeilingRide = Physics2D.OverlapCircle(m_CeilingRideU.position, 0.01f, m_GroundLayer) != null ? true : false;

        bool t_PreviousEdgeGrab = m_EdgeGrab;

        // Moving Platform detection
        if (t_Collider != null && t_Collider.gameObject.CompareTag("MovingPlatform") && !m_UserJump && !m_IsJumping && !m_IsSliding)
            transform.parent = t_Collider.gameObject.transform;
        else if (t_WallRCollider != null && t_WallRCollider.gameObject.CompareTag("MovingPlatform") && !m_UserJump && !m_IsJumping && !m_IsSliding && !m_IsGrounded && !t_IsCeilingRide) {
            Debug.Log("EdgeGrabR");
            transform.parent = t_WallRCollider.gameObject.transform;
            m_EdgeGrab = true;
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        else if (t_WallRCollider != null && !t_WallRCollider.gameObject.CompareTag("MovingPlatform") && !m_IsSliding && !m_UserJump && !m_IsJumping && !m_IsSliding && !m_IsGrounded)
        {
            Debug.Log("EdgeGrabR");
            m_EdgeGrab = true;
            m_Rigidbody2D.velocity = Vector2.zero;
        }
        else {
            transform.parent = null;
            m_EdgeGrab = false;
            m_Rigidbody2D.gravityScale = 1;
            m_Animator.SetBool("IsGrabing", false);
        }

        if (m_EdgeGrab != t_PreviousEdgeGrab && m_EdgeGrab)
        {
            m_Animator.SetTrigger("EdgeGrab");
            m_Animator.SetBool("IsGrabing", true);
            m_Rigidbody2D.gravityScale = 0;
            m_StoredJumpCount = JumpCount;
        }


        if (m_IsGrounded == true && t_previousGrounded == false)
        { // if the player just landed
            //m_Animator.SetTrigger("Land");
            // Give a boost depending on the previous velocity
            float t_RunSpeed = m_StoredRunSpeed * RunSpeedIncrease;
            float t_RunSpeed2 = Mathf.Abs(m_StoredVelocity.x);
            // take whichever is the greatest
            if (t_RunSpeed2 > t_RunSpeed)
                RunSpeed = t_RunSpeed2;
            else
                RunSpeed = t_RunSpeed;
            if (RunSpeed > MaxRunSpeedVelocity)
                RunSpeed = MaxRunSpeedVelocity;
            m_StoredJumpCount = JumpCount;
            Debug.Log("Landed");
        }

        if (RunSpeed > m_StoredRunSpeed && m_IsGrounded) {
            // Slowely decrease the run speed
            RunSpeed -= RunSpeedDescrease;
            //Debug.Log("RunSpeed: " + RunSpeed);
        }
 
        // Flip sprite
        if (t_MoveX < 0 && transform.localScale.x > 0 || t_MoveX > 0 && transform.localScale.x < 0 && !m_IsSliding)
        {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1));
        }

        // Sliding
        // Ceiling detection
        bool t_IsCeiling = false;
        if (m_IsSliding)
            t_IsCeiling = Physics2D.OverlapCircle(m_HeadSlidePos.position, 0.1f, m_GroundLayer) != null ? true : false;
        if (m_IsGrounded)
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        // If the player velocity is greater than 4 and the player is player is facing the same direction as the velocity
        if (Input.GetAxis("Shift") > 0 && !m_IsSliding && m_IsGrounded && t_SpeedX > 4 && Mathf.Sign(m_Rigidbody2D.velocity.x) == Mathf.Sign(transform.localScale.x))
        {
            m_IsSliding = true;
            m_Animator.SetBool("IsSliding", true);
            // Add force to the player
            Vector2 t_Velocity = Vector2.right * SlideForceBoost * (m_StoredVelocity.x + Mathf.Abs(m_StoredVelocity.y)/2);
            if (Mathf.Abs(m_Rigidbody2D.velocity.x) > RunSpeed)
                t_Velocity = Vector2.right * SlideForceBoost * (m_StoredVelocity.x + Mathf.Abs(m_StoredVelocity.y)/2) * SlideForceDamping;
            m_Rigidbody2D.AddForce(t_Velocity, ForceMode2D.Impulse);

        }
        else if (m_IsSliding && t_IsCeiling) // if the player is sliding and there is a ceiling above him
        {
            // Conserve the horizontal velocity
            m_Rigidbody2D.velocity = m_StoredVelocity;
        }
        else if (Input.GetAxis("Shift") <= 0 && m_IsSliding && !t_IsCeiling) // if the player is sliding and the user release the shift key
        {
            m_IsSliding = false;
            m_Animator.SetBool("IsSliding", false);
        }
        else if ((t_SpeedX <= 3 || !m_IsGrounded) && !t_IsCeiling) // if the player is not sliding and the speed is below 3
        {
            m_IsSliding = false;
            m_Animator.SetBool("IsSliding", false);
        }


        // Jump
        if (m_UserJump)
        {
            Debug.Log("Jump");
            m_UserJump = false;
            m_IsGrounded = false;
            m_IsJumping = true;
            StartCoroutine(ResetIsJumping());

            if (m_SecondaryJump)
            {
                m_SecondaryJump = false;
                // if the player is facing the opposite direction of the velocity, reverse it
                if (Mathf.Sign(m_Rigidbody2D.velocity.x) != Mathf.Sign(transform.localScale.x))
                    m_Rigidbody2D.velocity = new Vector2(-m_Rigidbody2D.velocity.x/2, m_Rigidbody2D.velocity.y);
            }

            if (t_PreviousEdgeGrab)
            {
                m_Rigidbody2D.AddForce(Vector2.up * JumpForce * 0.15f, ForceMode2D.Impulse);
            }

            m_Rigidbody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
        else if (m_UserQuickJump)
        {
            Debug.Log("QuickJump");
            m_UserQuickJump = false;
            m_IsGrounded = false;
            m_Rigidbody2D.AddForce(Vector2.up * (JumpForce-QuickJumpForceDecrease), ForceMode2D.Impulse);
            // increase current horizontal velocity by 20%
            float t_Velocity = m_Rigidbody2D.velocity.x * QuickJumpSpeedIncrease;

            // Calculate the force to be applied with damping
            //float t_force = Vector2.right.x * t_Velocity * Mathf.Clamp01(1 - (Mathf.Abs(m_Rigidbody2D.velocity.x) / 10));
            //Debug.Log("Mathf " + Mathf.Clamp01(1 - (Mathf.Abs(m_Rigidbody2D.velocity.x) / 1)));

            //m_Rigidbody2D.AddForce(new Vector2(t_force, 0), ForceMode2D.Impulse);
        }

        m_StoredVelocity = m_Rigidbody2D.velocity;
    }

    IEnumerator ResetIsJumping()
    {
        yield return new WaitForSeconds(0.1f);
        m_IsJumping = false;
    }

    public void MagicalStar()
    {
        Debug.Log("MagicalStar");
        // Create a magical star
        GameObject t_MagicalStar = Instantiate(MagicalStarPrefab, transform.position, Quaternion.identity);
        // get the position of the mouse
        Vector3 t_MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // position the star on the mouse position
        t_MagicalStar.transform.position = new Vector3(t_MousePosition.x, t_MousePosition.y, 0);
        // activate it
        t_MagicalStar.SetActive(true);
    }

    public void TakeDamage(int p_Damage)
    {
        Health -= p_Damage;
        if (Health <= 0)
        {
            m_Animator.SetBool("IsDead", true);
            Time.timeScale = 0.33f;
            m_GameOverScren.SetActive(true);
            this.gameObject.GetComponent<PlayerController>().enabled = false;
            m_StatusManager.GetComponent<StatusManager>().setHealth(0);
            
        }
        else
        {
            m_Animator.SetTrigger("TakeDamage");
            m_StatusManager.GetComponent<StatusManager>().setHealth(Health);
        }
    }

    public void MotionBlur()
    {
        // Each frame, we get the current sprite of the player
        SpriteRenderer t_SpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        // We summon a prefab "PlayerBlur" and we set it's sprite to the current sprite of the player
        GameObject t_PlayerBlur = Instantiate(PlayerBlur, transform.position, Quaternion.identity);
        t_PlayerBlur.GetComponent<SpriteRenderer>().sprite = t_SpriteRenderer.sprite;
        // We spawn it on the player position
        t_PlayerBlur.transform.position = transform.position;
        // We give it the same scale as the player
        t_PlayerBlur.transform.localScale = transform.localScale;
        // Move it on the layer n-1
        t_PlayerBlur.GetComponent<SpriteRenderer>().sortingOrder = t_SpriteRenderer.sortingOrder - 1;
    }
}
