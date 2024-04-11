using System;
using System.Collections;
using UnityEngine;
using Pool;

public class EnemyController : MonoBehaviour, IResettable
{
    [SerializeField]
    private Character enemyInfo;
    
    public int Atk { 
        get {
            return enemyInfo.atk;
        }
    }

    public event EventHandler Dead;

    private int currentHealth;

    private SpriteRenderer spriteRenderer = null;
    private Rigidbody2D enemyRigid = null;

    private bool isDamaged = false;
    private bool isDead = false;
    private PlayerController playerController = null;
    private EnemyMove enemyMove = null;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyRigid = GetComponent<Rigidbody2D>();
        enemyMove = GetComponent<EnemyMove>();
        currentHealth = enemyInfo.health;
    }

    public void Reset()
    {
        Init();
        currentHealth = enemyInfo.health;
        
        isDamaged = false;
        isDead = false;

        enemyRigid.simulated = true;
        enemyMove.enabled = true;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;
        if (isDamaged) return;        
        if (collision.gameObject.CompareTag("Attack"))
        {                        
            playerController = collision.GetComponentInParent<PlayerController>();
            if (playerController != null) 
            {                
                Damaged();
            }            
        }
    }

    private void Damaged()
    {
        isDamaged = true;
        currentHealth -= GameManager.Instance.PlayerInfo.atk;
        GameManager.Instance.ShowDamageText(transform, GameManager.Instance.PlayerInfo.atk);
        if (currentHealth <= 0)
        {
            OnDead();
            return;
        }
        StartCoroutine(DoDamageAnimation());
    }

    private void OnDead()
    {
        isDamaged = false;
        isDead = true;
        enemyMove.enabled = false;
        enemyRigid.simulated = false;
        StartCoroutine(DoDeadAnimation());
    }

    private IEnumerator DoDamageAnimation()
    {                
        enemyMove.Damaged();
        spriteRenderer.material.SetColor("_Color", new Color(1f, 0f, 0f, 0f));
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
        enemyMove.ReturnToPreviousState();
        isDamaged = false;
    }

    private IEnumerator DoDeadAnimation()
    {
        enemyMove.Dead();
        for(int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }        
        Dead?.Invoke(this, null);
    }    
}
