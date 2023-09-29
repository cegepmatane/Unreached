using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float RunSpeed = 3f;
    public float RunSpeedIncrease = 1.3f;
    public float RunSpeedDescrease = 0.1f;
    public float QuickJumpSpeedIncrease = 0.2f;
    public float QuickJumpForceDecrease = 4f;
    public float WalkSpeed = 1f;
    public float JumpForce = 10f;
    public float SlideForceBoost = 1.2f;
    public float SlideForceDamping = 0.3f;
    public int Health = 100;
    public float AttackWaitTime = 0.5f;
    public float AttackFallSpeed = 0.2f;

    private bool m_UserJump = false;
    private bool m_UserQuickJump = false;
    private bool m_IsGrounded = false;
    private bool m_IsSliding = false;
    private bool m_IsAttacking = false;
    private float StoredRunSpeed;

    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2D;

    [SerializeField]
    private Transform m_FeetPos;
    [SerializeField]
    private LayerMask m_GroundLayer;

    private void Awake()
    {
        m_Animator = this.gameObject.GetComponent<Animator>();
        m_Rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StoredRunSpeed = RunSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        // Jump input
        if (Input.GetButtonDown("Jump") && m_IsGrounded && !m_IsSliding) {
            m_UserJump = true;
            m_Animator.SetTrigger("Jump");
        }
        else if (Input.GetButtonDown("Jump") && m_IsGrounded && m_IsSliding)
        {
            m_UserQuickJump = true;
            m_Animator.SetTrigger("QuickJump");
        }


        // Attack input
        if (Input.GetButtonDown("Fire1") && !m_IsSliding && !m_IsAttacking) {
            Debug.Log("Attack");
            m_Animator.SetTrigger("Attack");
            m_IsAttacking = true;
            StartCoroutine(ResetIsAttacking());
        }


        // If the user touch the void, he dies
        if (transform.position.y < -10)
            TakeDamage(99999);
        
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
    }

    private void FixedUpdate()
    {
        //Debug.Log("RunSpeed: " + RunSpeed);
        //Debug.Log("Velocity: " + m_Rigidbody2D.velocity.x);

        float t_MoveX = Input.GetAxis("Horizontal");
        float t_SpeedX = Mathf.Abs(m_Rigidbody2D.velocity.x);
        m_Animator.SetFloat("Speed", Mathf.Abs(t_MoveX));

        float t_Force = t_MoveX * RunSpeed;
        if (Mathf.Abs(t_Force) > WalkSpeed)
            t_Force = WalkSpeed * Mathf.Sign(t_Force);
        //Debug.Log("Force: " + t_Force);

        if (!m_IsSliding && !m_IsAttacking && m_IsGrounded)
            m_Rigidbody2D.velocity = new Vector2(t_MoveX * (RunSpeed), m_Rigidbody2D.velocity.y);
            //m_Rigidbody2D.AddForce(new Vector2(t_Force, 0), ForceMode2D.Force);
        else if (!m_IsSliding && m_IsAttacking)
            m_Rigidbody2D.velocity = new Vector2(t_MoveX * (WalkSpeed), m_Rigidbody2D.velocity.y);
        else if (!m_IsSliding && !m_IsAttacking && !m_IsGrounded)
            m_Rigidbody2D.AddForce(new Vector2(t_Force, 0), ForceMode2D.Force);
        
        // Ground detection
        bool t_previousGrounded = m_IsGrounded;
        m_IsGrounded = Physics2D.OverlapCircle(m_FeetPos.position, 0.2f, m_GroundLayer) != null ? true : false;
        m_Animator.SetBool("IsGrounded", m_IsGrounded);

        if (m_IsGrounded == true && t_previousGrounded == false) {
            //m_Animator.SetTrigger("Land");
            RunSpeed = StoredRunSpeed * RunSpeedIncrease;
            //Debug.Log("Land or takeoff, i have no idea");
        }
        if (RunSpeed > StoredRunSpeed && m_IsGrounded) {
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
        if (Input.GetAxis("Shift") > 0 && !m_IsSliding && t_SpeedX > 3 && m_IsGrounded)
        {
            m_IsSliding = true;
            m_Animator.SetBool("IsSliding", true);
            // Add force to the player
            Vector2 t_Velocity = Vector2.right * SlideForceBoost * transform.localScale.x;
            if (Mathf.Abs(m_Rigidbody2D.velocity.x) > RunSpeed)
                t_Velocity = Vector2.right * SlideForceBoost * transform.localScale.x * SlideForceDamping;
            m_Rigidbody2D.AddForce(t_Velocity, ForceMode2D.Impulse);

        }
        else if (Input.GetAxis("Shift") <= 0 && m_IsSliding)
        {
            m_IsSliding = false;
            m_Animator.SetBool("IsSliding", false);
        }
        else if (t_SpeedX <= 3 || !m_IsGrounded)
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
    }

    public void TakeDamage(int p_Damage)
    {
        Health -= p_Damage;
        if (Health <= 0)
        {
            m_Animator.SetBool("IsDead", true);
            this.gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }
}
