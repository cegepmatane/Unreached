using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float RunSpeed = 3f;
    public float WalkSpeed = 1f;
    public float JumpForce = 10f;
    public float SlideForceBoost = 1.2f;
    public int Health = 100;
    public float AttackWaitTime = 0.5f;

    private bool m_UserJump = false;
    private bool m_IsGrounded = false;
    private bool m_IsSliding = false;
    private bool m_IsAttacking = false;

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

    // Update is called once per frame
    private void Update()
    {
        // Jump input
        if (Input.GetButtonDown("Jump") && m_IsGrounded)
            m_UserJump = true;

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
        yield return new WaitForSeconds(AttackWaitTime);
        m_IsAttacking = false;
    }

    private void FixedUpdate()
    {
        float t_MoveX = Input.GetAxis("Horizontal");
        float t_SpeedX = Mathf.Abs(m_Rigidbody2D.velocity.x);
        m_Animator.SetFloat("Speed", Mathf.Abs(t_MoveX));

        float t_Force = t_MoveX * RunSpeed;
        //Debug.Log("Force: " + t_Force);

        if (!m_IsSliding && !m_IsAttacking)
            m_Rigidbody2D.velocity = new Vector2(t_MoveX * (RunSpeed), m_Rigidbody2D.velocity.y);
            //m_Rigidbody2D.AddForce(new Vector2(t_Force, 0), ForceMode2D.Force);
        else if (!m_IsSliding && m_IsAttacking)
            m_Rigidbody2D.velocity = new Vector2(t_MoveX * (WalkSpeed), m_Rigidbody2D.velocity.y);
        
        // Ground detection
        m_IsGrounded = Physics2D.OverlapCircle(m_FeetPos.position, 0.2f, m_GroundLayer) != null ? true : false;
        m_Animator.SetBool("IsGrounded", m_IsGrounded);

 
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
            m_Rigidbody2D.AddForce(Vector2.right * SlideForceBoost * transform.localScale.x, ForceMode2D.Impulse);
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
            m_UserJump = false;
            m_IsGrounded = false;
            m_Animator.SetTrigger("Jump");
            m_Rigidbody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
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
