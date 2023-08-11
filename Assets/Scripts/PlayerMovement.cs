using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
	private Animator anim;
	private SpriteRenderer sprite;
	private BoxCollider2D coll;
	[SerializeField] private LayerMask jumpableGround;
	public Joystick joystick;
	
	private float dirX = 0f;
	[SerializeField]private float moveSpeed = 7f;
	[SerializeField]private float jumpForce = 14f;
	
	private enum MovementState{idle,running,jumping,falling}
	
	[SerializeField] private AudioSource jumpSound;
	
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        //dirX = Input.GetAxisRaw("Horizontal");
        dirX = joystick.Horizontal;

        rb.velocity = new Vector2(dirX * moveSpeed , rb.velocity.y); 
		
		
        
		UpdateAnimationState();
    }
	public void Jump()
	{
		if(IsGrounded())
        {
			jumpSound.Play();
			rb.velocity = new Vector2(rb.velocity.x , jumpForce);
		}
	}
	
	
	private void UpdateAnimationState()
	{
		MovementState state;
		if (dirX > 0f)
		{
			state = MovementState.running;
			sprite.flipX = false;
		}
		else if(dirX < 0f)
		{
			state = MovementState.running;
			sprite.flipX = true;
		}
		else
		{
			state = MovementState.idle;
		}
		
		if (rb.velocity.y > .1f )
		{
			state = MovementState.jumping;
		}
		else if (rb.velocity.y < -.1f)
		{
			state = MovementState.falling;
		}
		else
		{
			state = MovementState.idle;
		}		

			anim.SetInteger("state",(int)state);
	}
	private bool IsGrounded()
	{
		return Physics2D.BoxCast(coll.bounds.center , coll.bounds.size , 0f , Vector2.down , .1f , jumpableGround );
	}
}
