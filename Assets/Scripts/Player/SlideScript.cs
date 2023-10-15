using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideScript : StateMachineBehaviour
{
    private CapsuleCollider2D capsuleCollider;
    private CapsuleCollider2D capsuleCollider2;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter Slide");
        // Find the capsule collider on the player
        CapsuleCollider2D[] capsuleColliders = animator.GetComponents<CapsuleCollider2D>();
        capsuleCollider = capsuleColliders[0];
        capsuleCollider2 = capsuleColliders[1];

        // Store the original size of the collider
        originalColliderSize = capsuleCollider.size;
        originalColliderOffset = capsuleCollider.offset;

        // Set the coolider to horizontal
        capsuleCollider.direction = CapsuleDirection2D.Horizontal;
        capsuleCollider2.direction = CapsuleDirection2D.Horizontal;


        // Slide: reduce the collider height, adjust the collider offset
        capsuleCollider.size = new Vector2(originalColliderSize.y, originalColliderSize.x);
        capsuleCollider.offset = new Vector2(capsuleCollider.offset.x - 0.05f, capsuleCollider.offset.y - 0.09f);

        capsuleCollider2.size = new Vector2(originalColliderSize.y, originalColliderSize.x);
        capsuleCollider2.offset = new Vector2(capsuleCollider.offset.x - 0.05f, capsuleCollider.offset.y - 0.09f);

        // Pause the game 
        //Time.timeScale = 0.0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the collider to its original size
        Debug.Log("Exit Slide");
        capsuleCollider.direction = CapsuleDirection2D.Vertical;
        capsuleCollider.size = originalColliderSize;
        capsuleCollider.offset = originalColliderOffset;

        capsuleCollider2.direction = CapsuleDirection2D.Vertical;
        capsuleCollider2.size = originalColliderSize;
        capsuleCollider2.offset = originalColliderOffset;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
