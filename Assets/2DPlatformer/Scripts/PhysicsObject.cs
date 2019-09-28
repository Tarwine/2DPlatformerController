using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float gravityModifier = 1f;
    public float minGroundNormalY = 0.65f;

    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDist = 0.001f;
    protected const float shellRadius = 0.01f;
    
    void OnEnable ()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 move = Vector2.up * deltaPosition.y;
        Movement(move);
    }

    void Movement(Vector2 moveVec)
    {
        float distance = moveVec.magnitude;

        if(distance > minMoveDist)
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
            }

        }

        rb2d.position = rb2d.position + moveVec;
    }
}
