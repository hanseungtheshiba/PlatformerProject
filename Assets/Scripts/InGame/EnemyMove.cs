using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private enum EnemyState
    {
        NONE = 0,
        PATROL = 1 << 0,
        CHASE = 1 << 1,
        DAMAGED = 1 << 2,
        DEAD = 1 << 3
    }

    [SerializeField]
    private float moveSpeed = 0.5f;
    [SerializeField]
    private float idleDelay = 1f;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private Transform groundDetection = null;

    private EnemyState state = EnemyState.PATROL;
    private EnemyState previousState = EnemyState.NONE;

    private Rigidbody2D enemyRigid = null;
    private Animator animator = null;

    private float direction = -1f;
    private float idleTimer = 0f;

    private void OnEnable()
    {
        state = EnemyState.PATROL;
        animator = GetComponentInChildren<Animator>();
        direction = (GameManager.Instance.PlayerMove.transform.position.x - transform.position.x) >= 0f ? -1f: 1f;
        SetDirection();
        enemyRigid = GetComponent<Rigidbody2D>();        
    }
  

    private void FixedUpdate()
    {
        DecideBehaviour();        
        SetDirection();
    }

    private void DecideBehaviour()
    {
        switch (state)
        {
            case EnemyState.PATROL:
                PatrolLeftAndRight();
                break;
            case EnemyState.NONE:
                Idle();
                break;
            case EnemyState.DAMAGED:
                break;
        }
    }

    private void SetDirection()
    {        
        animator.transform.localScale = direction < 0f ? Vector3.one : new Vector3(-1f, 1f, 1f);
    }

    private void PatrolLeftAndRight()
    {        
        if (!IsGrounded())
        {
            state = EnemyState.NONE;
            return;
        }
        animator.Play("Walk");
        animator.SetFloat("velocityX", Mathf.Abs(direction));
        enemyRigid.velocity = new Vector2(direction * moveSpeed, 0f);
    }

    private void Idle()
    {
        state = EnemyState.NONE;
        animator.Play("Idle");
        animator.SetFloat("velocityX", 0f);
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDelay)
        {
            idleTimer = 0f;
            direction *= -1f;
            state = EnemyState.PATROL;
        }
    }

    public void ReturnToPreviousState()
    {
        state = previousState;
        DecideBehaviour();
    }

    public void Damaged()
    {
        previousState = state;
        state = EnemyState.DAMAGED;
        animator.Play("Damage");
    }

    public void Dead()
    {
        state = EnemyState.DEAD;
        animator.Play("Dead");
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundDetection.position, 0.05f, layerMask);
    }
}
