using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerPlayerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    public float jumpSpeed = 2;
    public float jumpTime = 0.4f;
    public int numJumps = 1;

    private float curJumpTime;
    private bool lastJumped;
    public int curNumJumps;

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

        if(Input.GetButton("Jump"))
        {
            curJumpTime += Time.deltaTime;
            if ((lastJumped && curJumpTime < jumpTime) || (curNumJumps < numJumps && !lastJumped)) {
                if(!lastJumped) {
                    lastJumped = true;
                    curJumpTime = 0; 
                    curNumJumps++;
                    if(curNumJumps >= numJumps) lastJumped = true;
                    velocity.y = jumpTakeOffSpeed;
                }
                velocity.y += jumpSpeed;
            }
            
        } else if (Input.GetButtonUp("Jump")) {
            lastJumped = false;
            if(velocity.y > 0)
            {
                velocity.y *= 0.5f;
            }
        } else {
            if (grounded) curNumJumps = 0;
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
}
