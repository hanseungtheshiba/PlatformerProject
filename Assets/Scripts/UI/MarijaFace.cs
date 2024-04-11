using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MarijaFace : MonoBehaviour
{
    private Animator animator = null;
    public Action Damaged;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Damaged = new Action(OnDamaged);
        EventManager.StartListening(EventManager.EventName.PLAYER_DAMAGED, Damaged);
    }

    private void OnDamaged()
    {
        animator.Play("Damaged");
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EventManager.EventName.PLAYER_DAMAGED, Damaged);
    }
}
