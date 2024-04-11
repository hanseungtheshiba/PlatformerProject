using System;
using System.Collections;
using UnityEngine;
using Pool;

public class ItemController : MonoBehaviour, IResettable
{
    [SerializeField]
    private EventManager.EventName eventName = EventManager.EventName.GET_DOGTAG;

    private Collider2D col = null;
    private Rigidbody2D itemRigid = null;
    private bool isCollected = false;

    public event EventHandler Collect;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        itemRigid = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCollected) return;
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {            
            StartCoroutine(OnCollect());
        }        
    }

    private IEnumerator OnCollect()
    {
        isCollected = true;
        EventManager.TriggerEvent(eventName);
        col.enabled = false;
        itemRigid.velocity = Vector2.up * 3f;
        yield return new WaitForSeconds(0.5f);
        Collect?.Invoke(this, null);
    }

    public void Reset()
    {
        isCollected = false;
        col.enabled = true;
        gameObject.SetActive(false);
    }
}
