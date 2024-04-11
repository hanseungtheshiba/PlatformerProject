using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{        
    public Action Dead;

    private int currentHealth = 0;

    private SpriteRenderer spriteRenderer = null;    
    private EnemyController enemy = null;
    private Animator animator = null;
    private PlayerMove playerMove = null;

    private bool isDamaged = false;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = GameManager.Instance.PlayerInfo.maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();        
        playerMove = GetComponent<PlayerMove>();
        Dead = new Action(OnDead);
        EventManager.StartListening(EventManager.EventName.PLAYER_DEAD, Dead);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy")))
        {
            if (isDamaged) return;
            enemy = collision.gameObject.GetComponent<EnemyController>();
            Damage();
        }
    }

    private void Damage()
    {
        isDamaged = true;        
        if (enemy != null) 
        {            
            GameManager.Instance.ChangeHealthValue(-enemy.Atk);
            GameManager.Instance.ShowDamageText(transform, enemy.Atk);
            currentHealth = GameManager.Instance.PlayerInfo.health;
            EventManager.TriggerEvent(EventManager.EventName.PLAYER_DAMAGED);
            if (currentHealth <= 0) 
            {
                OnDead();
                EventManager.TriggerEvent(EventManager.EventName.PLAYER_DEAD);
                return;
            }
        }        
        StartCoroutine(DoDamageAnimation());
    }

    private void OnDead()
    {        
        isDamaged = false;
        isDead = true;        
        playerMove.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Item");
        animator.Play("Dead");
    }

    private IEnumerator DoDamageAnimation()
    {
        gameObject.layer = LayerMask.NameToLayer("Item");
        for(int i = 0; i < 5; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.layer = LayerMask.NameToLayer("Player");
        isDamaged = false;
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EventManager.EventName.PLAYER_DEAD, Dead);
    }
}
