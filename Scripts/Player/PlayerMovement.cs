using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Bring RigidBody2D into the script (This is what allows the physics engine in Unity to be called here)
    private Rigidbody2D body;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private int wallJumpForceAway;
    [SerializeField] private int wallJumpForceUp;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    //4:44 on #3 Wall Jumping
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;
    



    //The Awake() method will be called upon startup of the game, but only then
    //When game starts, the variable "body" will be set equal to the RigidBody2D component
    //When game starts, bring animator variable from unity into visual studio
    private void Awake(){
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();


        //Freeze simulation rotation of the player (Might need to remove if other game objects require spinning)
        
    }

    //I believe this is called every frame. 
    //Use this method to make movement, or anything that needs to be updated every frame.
    private void Update(){

        //Float that has its horizontal position and a vector that has player's velocity
        horizontalInput = Input.GetAxis("Horizontal");
        


        //Player Faces correct direction because of this
        if(horizontalInput > 0.01f) 
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        

        

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        //Wall Jump Logic
        if (wallJumpCooldown > 0.2f) {

            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded()) {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else 
                body.gravityScale = 2.2f;

            //Jump Mechanic
            if(Input.GetKey(KeyCode.Space)) {
                Jump();
            }
            
        }
        else
            wallJumpCooldown += Time.deltaTime;
        }

    private void Jump() {
        if(isGrounded()) {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded()) {
            wallJumpCooldown = 0;
            body.velocity = new Vector2(-1 * Mathf.Sign(transform.localScale.x) * wallJumpForceAway, wallJumpForceUp);
        }

            
        
    }

    

    private bool isGrounded() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

        return raycastHit.collider != null;
    }

    private bool onWall() {
        RaycastHit2D raycastHit2 = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);

        return raycastHit2.collider != null;
    }

    public bool canAttack() {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
