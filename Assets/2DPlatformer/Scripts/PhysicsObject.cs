using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float gravityModifier = 1f;
    public float minGroundNormalY = 0.65f;

    protected bool wasGrounded;
    public bool grounded;
    protected Vector2 groundNormal;

    protected Vector2 targetVelocity;
    public bool showDebugVectors;

    protected Rigidbody2D rb2d;
    protected Collider2D collider2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDist = 0.001f;
    protected const float shellRadius = 0.02f;
    
    void OnEnable ()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        // Use the game object's layer as the collision mask layer
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity() {

    }

    protected virtual void OnLeftGround() {

    }

    void FixedUpdate()
    {
        grounded = false;

        // Add gravity
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;     

        Vector2 deltaPosition = velocity * Time.deltaTime;

        // Get the normal vector along the ground by mirroring x
        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;
        
        Movement(move, false);

        if(showDebugVectors) {
            Debug.DrawRay(rb2d.worldCenterOfMass,
                            move * 3, 
                            Color.yellow,
                            Time.deltaTime);

            Debug.DrawRay(rb2d.worldCenterOfMass,
                            groundNormal * 1, 
                            Color.magenta,
                            Time.deltaTime);
        }
        
        move = Vector2.up * deltaPosition.y;

        Movement(move, true);
        

        if(!grounded && wasGrounded) OnLeftGround();
        wasGrounded = grounded;
    }

    void Movement(Vector2 moveVec, bool yMovement)
    {
        float distance = moveVec.magnitude;

        if(distance > minMoveDist || true)
        {
            int hitCount = rb2d.Cast(moveVec, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for(int i = 0; i < hitCount; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }
            for(int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if(currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if(yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                // 1. If A and B are perpendicular, the result of the dot product will be zero.
                // 2. If the angle between A and B  is less than 90 degrees, the Dot product will be positive.
                // 3. If the angle between A and B is greater than 90 degrees, the Dot product will be negative.
                float projection = Vector2.Dot(velocity, currentNormal);
                // Cancel out part of velocity which would be stopped by collision
                if(projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }
                
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        rb2d.position = rb2d.position + moveVec.normalized * distance;
    }
}
