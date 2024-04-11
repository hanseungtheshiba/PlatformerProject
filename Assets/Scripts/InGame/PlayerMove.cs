using System;
using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Flags]
    private enum PlayerState
    {
        NONE = 0,
        JUMP = 1 << 0,
        JUMPING_DOWN = 1 << 1,
        ATTACK = 1 << 2,
        READY_TO_RUN = 1 << 3,
        RUN = 1 << 4
    }

    [SerializeField]
    private int maxJumpCount = 2;

    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float walkSpeed = 1f;
    [SerializeField]
    private float runSpeed = 2f;
    [SerializeField]
    private float maxSpeed = 10f;
    [SerializeField]
    private float doubleTapDelay = 0.5f;
       

    private PlayerState state = PlayerState.NONE;

    private Collider2D col = null;
    private Rigidbody2D playerRigid = null;
    private Animator animator = null;

    private float moveSpeed = 0f;
    private float velocityX = 0f;
    private float dashTimer = 0f;
    private float attackTimer = 0f;

    private int jumpCount = 0;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        playerRigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();           
    }

    private void Update()
    {
        SetCharacterDirection();
        Move();
        Dash();
        Jump();        
        PlayJumpAnimation();
        Attack();
        CheckAttackDelay();
    }

    private void Idle()
    {
        jumpCount = 0;
        state = PlayerState.NONE;
        animator.Play("Idle");
    }

    private void Move()
    {
        velocityX = Input.GetAxisRaw("Horizontal");
        if(state.HasFlag(PlayerState.RUN))
        {
            moveSpeed = runSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }
        playerRigid.velocity = new Vector2(velocityX * moveSpeed, Mathf.Clamp(playerRigid.velocity.y, -maxSpeed, maxSpeed));
    }

    private void Dash()
    {
        if (state == PlayerState.RUN)
        {
            if (Input.GetButton("Horizontal"))
            {
                animator.Play("Run");
            }
            if (Input.GetAxisRaw("Horizontal") == 0f)
            {
                Idle();
            }
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            state = PlayerState.READY_TO_RUN;            
        }

        if (state.HasFlag(PlayerState.READY_TO_RUN))
        {
            animator.Play("Idle");
            dashTimer += Time.deltaTime;
            if (Input.GetButtonDown("Horizontal"))
            {
                state = PlayerState.RUN;
            }
            if (dashTimer >= doubleTapDelay)
            {
                dashTimer = 0f;
                Idle();
            }
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {           
            if (Input.GetAxisRaw("Vertical") < 0f && transform.position.y >= 0f)
            {
                if (state.HasFlag(PlayerState.JUMPING_DOWN)) return;
                state |= PlayerState.JUMPING_DOWN;
                StartCoroutine(JumpDown());
                return;
            }
            else
            {
                state |= PlayerState.JUMP;
                jumpCount++;
                playerRigid.AddForce(Vector2.up * maxSpeed, ForceMode2D.Impulse);
                return;
            }
        }

        if (Input.GetButtonDown("Jump") && state.HasFlag(PlayerState.JUMP) && jumpCount < maxJumpCount)
        {
            jumpCount++;
            playerRigid.AddForce(Vector2.up * maxSpeed, ForceMode2D.Impulse);
        }
    }

    private void Attack()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            attackTimer = 0f;
            state = PlayerState.ATTACK;
            animator.Play("Attack");
        }        
    }

    private void SetCharacterDirection()
    {
        if (velocityX > 0f)
        {
            animator.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (velocityX < 0f)
        {
            animator.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void PlayJumpAnimation()
    {
        animator.SetFloat("velocityX", Mathf.Abs(velocityX));

        if(state.HasFlag(PlayerState.JUMP))
        {
            if (playerRigid.velocity.y > 0.1f)
            {
                animator.Play("Jump");
            }
            else if (playerRigid.velocity.y < -0.1f)
            {
                animator.Play("Fallen");
            }
            else if(IsGrounded())
            {
                jumpCount = 0;
                state &= ~PlayerState.JUMP;
                animator.Play("Idle");
            }
        }
    }

    private void CheckAttackDelay()
    {
        if (state.HasFlag(PlayerState.ATTACK))
        {
            attackTimer += Time.deltaTime;            
            
            if(attackTimer >= doubleTapDelay)
            {                
                attackTimer = 0f;
                state &= ~PlayerState.ATTACK;
                return;
            }
        }        
    }

    private bool IsGrounded()
    {        
        return Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 180f, layerMask);
    }

    private IEnumerator JumpDown()
    {
        col.isTrigger = true;
        playerRigid.AddForce(Vector2.down, ForceMode2D.Impulse);        

        // 바닥과 충돌하였는지 체크
        while (IsGrounded())
        {            
            yield return null;            
        }
                
        col.isTrigger = false;
        state = PlayerState.NONE;
    }
}
