using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeControl : MonoBehaviour
{
    [Header("Casting")]
    public Transform castOrigin;
    public Transform castDestination;
    public LayerMask castMask;

    [Header("Movement")]
    public float moveSpeed = 4;
    bool facingRight = false;
    Vector2 direction;

    Rigidbody2D _rigidbody;
    Animator animator;

    Coroutine moveRoutine;

    [Header("Damage")]
    public int health = 2;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ResetBehaviour();
    }

    public void ResetBehaviour()
    {
        moveRoutine = StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        //determine our direction
        DetermineDirection();
        while(true)
        {
            //if we are at the edge of a platform or at a wall
            if (GroundCheck())
            {
                Flip();//then flip
                DetermineDirection();//determine direction
            }
            else
            {
                Vector2 newPosition = (Vector2)transform.position + (direction * moveSpeed * Time.deltaTime);
                _rigidbody.MovePosition(newPosition);
            }
            yield return null;
        }
    }

    bool GroundCheck()//returns true if at wall or ledge, returns false if the path continues
    {
        bool endReached = false;

        RaycastHit2D[] hitArray = new RaycastHit2D[1];
        if (Physics2D.LinecastNonAlloc((Vector2)castOrigin.position,
            (Vector2)castDestination.position,
            hitArray,
            castMask) > 0)
        {
            Vector2 hitVector = hitArray[0].transform.TransformDirection(hitArray[0].normal);
            if(hitVector.y > 0.9f)//if we're still hitting ground
            {
                endReached = false;
            }
            else//if we hit something but its not ground ie. wall
            {
                endReached = true;
            }
        }
        else //if this intersects nothing, we've reached the end of our platform/ground
        {
            endReached = true;
        }

        return endReached;
    }

    void DetermineDirection()
    {
        if(facingRight)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = Vector2.left;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(Vector2.up, 180f);
    }

    public void TakeDamage()
    {
        health -= 1;
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        if(health > 0)//still alive
        {
            animator.SetTrigger("Hit");
        }
        else//death
        {
            Destroy(_rigidbody);
            Destroy(GetComponent<Collider2D>());
            animator.SetTrigger("Death");
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
