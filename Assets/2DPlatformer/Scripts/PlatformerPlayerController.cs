using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayerController : PhysicsObject
{
    // Horizontal speed
    public float maxSpeed = 5;
    
    // Jump variables
    public float jumpTakeOffSpeed = 6;
    public float jumpSpeed = 0.45f;
    public float jumpTime = 0.13f;
    public float jumpFallTolerance = 0.1f;
    public int numJumps = 2;
    private float curJumpTime;
    private bool lastJumped;
    private int curNumJumps;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        
        move.x = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump")) {
            lastJumped = false;
            
        } else if(Input.GetButton("Jump")) {
            curJumpTime += Time.deltaTime;
            if ((lastJumped && curJumpTime < jumpTime) || (curNumJumps < numJumps && !lastJumped)) {
                if(!lastJumped) {
                    lastJumped = true;
                    curJumpTime = 0; 
                    curNumJumps++;
                    velocity.y = jumpTakeOffSpeed;
                }
                velocity.y += jumpSpeed;
            }
            
        } else if (grounded && curNumJumps > 0){
            curNumJumps = 0;
            lastJumped = false;
        } 
        
        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));

        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    protected override void OnLeftGround() {
        StartCoroutine(disableJump());
    }

    IEnumerator disableJump()
    {
        yield return new WaitForSeconds(jumpFallTolerance);
        if(!lastJumped && !grounded) 
        {
            curNumJumps = 1;
        }
    }
}
