using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("Input")]
    public float jumpHoldTime = 0.3f;
    float horizontalInput;//movement this frame
    float lastHorizontalInput;//movement last frame
    bool dead = false;
    bool atExit = false;

    [Header("Basic Movement")]
    public float moveSpeed = 5f;
    float currentMove = 0f;
    bool moving = false;
    public bool facingRight = true;

    Vector2 finalMovement;
    bool runMovement = false;

    Rigidbody2D _rigidbody;
    Animator animator;

    [Header("Vertical Movement")]
    public float jumpSpeed = 5;
    public bool inAir = true;
    bool jumpHeld = false;

    [Header("Gravity")]
    public float upGravity = -16f;
    public float downGravity = -20f;
    public float jumpHoldGravity = -5f;
    public float maxFallSpeed = -10;
    float currentGravity = 0f;

    [Header("Collision Handling")]
    public bool wallRight = false;
    public bool wallLeft = false;
    Collider2D currentGround;
    Collider2D leftWall;
    Collider2D rightWall;

    [Header("GUI")]
    public GameObject levelExitGUI;
    public GameObject gameOverGUI;
    public Text scoreText;

    [Header("Score")]
    public int currentScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score - " + currentScore.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(dead)
        {
            return;
        }
        GetInput();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if(horizontalInput != 0//if there is movement this frame
            && lastHorizontalInput == 0)//and there wasn't last frame
        {//then move
            moving = true;
            animator.SetBool("Moving", true);
        }

        if(horizontalInput == 0//no movement this frame
            && lastHorizontalInput != 0)//there WAS movement last frame
        {
            //then stop
            moving = false;
            animator.SetBool("Moving", false);
        }

        if(horizontalInput > 0f && !facingRight
            || horizontalInput < 0f && facingRight)
        {
            Flip();
        }

        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if(Input.GetAxis("Vertical") != 0
            && atExit)//end level
        {
            EndLevel(false);
        }

        lastHorizontalInput = horizontalInput;
    }

    void EndLevel(bool isDeath)
    {
        if(isDeath)
        {
            StartCoroutine(GameOverRoutine());
        }
        else//if we got the level exit door
        {
            ClearControl();
            levelExitGUI.SetActive(true);
        }
    }

    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        gameOverGUI.SetActive(true);
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(transform.up, 180f);
    }

    void Jump()
    {
        if(inAir)
        {
            return;
        }
        inAir = true;
        animator.SetTrigger("Jump");
        animator.SetBool("InAir", true);
        currentGravity = jumpSpeed;
        StartCoroutine(JumpHoldRoutine());
    }

    IEnumerator JumpHoldRoutine()
    {
        jumpHeld = true;
        float timer = 0f;
        while(timer < jumpHoldTime && Input.GetButton("Jump"))
        {
            timer += Time.deltaTime;
            yield return null;
        }
        jumpHeld = false;
    }

    private void FixedUpdate()
    {
        if(dead)
        {
            return;
        }
        if(inAir)
        {
            SetMovement();
            if(jumpHeld)
            {
                currentGravity += jumpHoldGravity*Time.deltaTime;
            }
            else
            {
                if(currentGravity > 0f)
                {
                    currentGravity += upGravity * Time.deltaTime;
                }
                else if(currentGravity <= 0f)
                {
                    currentGravity += downGravity * Time.deltaTime;
                }
            }

            currentGravity = Mathf.Clamp(currentGravity, maxFallSpeed, jumpSpeed);

            finalMovement.y = currentGravity;
        }
        if(moving)
        {
            SetMovement();
            currentMove = horizontalInput * moveSpeed;

            if(currentMove > 0f && wallRight
                || currentMove < 0f && wallLeft
                || wallRight && wallLeft)
            {
                currentMove = 0f;
            }

            finalMovement.x = currentMove;
        }
        if(runMovement)
        {
            _rigidbody.MovePosition((Vector2)transform.position + finalMovement*Time.deltaTime);
            runMovement = false;
        }
    }

    void SetMovement()
    {
        if(!runMovement)
        {
            runMovement = true;
            finalMovement = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "LevelExit")
        {
            atExit = true;
        }
        if(collision.tag == "ScorePkp")
        {
            ScorePickup pickup = collision.GetComponent<ScorePickup>();//get the reference to the pickup script
            int scoreToAdd = 0;//creat a variable to store the score from the pickup
            pickup.Pickup(out scoreToAdd);//run the method to grab the score and destroy the pickup
            currentScore += scoreToAdd;//add the new score to our current score
            UpdateScore();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "LevelExit")
        {
            atExit = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //        other colldier    our collider
        ColliderDistance2D collDist = collision.collider.Distance(gameObject.GetComponent<Collider2D>());
        Debug.DrawRay(collDist.pointA, collDist.normal, Color.black, 1f);

        print(collDist.normal.ToString());
        print(collDist.isOverlapped.ToString());
        if (collision.collider.tag == "Enviro")
        {
            if (collDist.normal.y > 0.1f)//Ground
            {
                Ground(collision.collider);
            }
            if(collDist.normal.y < -0.1f)//ceiling
            {
                currentGravity = 0f;
                jumpHeld = false;
            }
            if(collDist.normal.x < -0.9f)//right wall
            {
                wallRight = true;
                rightWall = collision.collider;
            }
            if(collDist.normal.x > 0.9f)
            {
                wallLeft = true;
                leftWall = collision.collider;
            }
        }
        else if(collision.collider.tag == "Enemy")
        {
            if(collDist.normal.y > 0.5f)
            {
                inAir = false;
                Jump();
                collision.collider.GetComponent<SlimeControl>().TakeDamage();
            }
            else
            {
                Die();
            }
        }
    }

    void ClearControl()
    {
        StopAllCoroutines();
        dead = true;
        Destroy(_rigidbody);
        Destroy(GetComponent<Collider2D>());
    }

    void Die()
    {
        ClearControl();
        animator.SetTrigger("Death");
        StartCoroutine(DeathRoutine());
        EndLevel(true);
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1);
        while(transform.position.y < 10000)
        {
            transform.Translate(new Vector3(0f, 1f, 0f) * Time.deltaTime);
            yield return null;
        }
    }

    void Ground(Collider2D newGround)
    {
        inAir = false;
        currentGravity = 0f;
        currentGround = newGround;
        animator.SetBool("InAir", false);
    }

    private void OnCollisionExit2D(Collision2D collision)//runs when contact is lost with a collider
    {
        if(collision.collider.tag == "Enviro")
        {
            if(collision.collider == currentGround)
            {
                if(!inAir)
                {
                    inAir = true;
                    currentGround = null;
                    animator.SetTrigger("Jump");
                    animator.SetBool("InAir", true);
                }
            }
            if(collision.collider == rightWall)
            {
                rightWall = null;
                wallRight = false;
            }
            if (collision.collider == leftWall)
            {
                leftWall = null;
                wallLeft = false;
            }
        }
    }
}
