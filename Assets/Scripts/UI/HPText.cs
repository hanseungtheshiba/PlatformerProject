using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class HPText : MonoBehaviour
{
    private TMP_Text hpText;

    public Action UpdateValue;

    private void Awake()
    {
        hpText = GetComponent<TMP_Text>();
    }

    private void DoUpdateValue()
    {
        hpText.text = string.Format("{0}", GameManager.Instance.PlayerInfo.health);
    }

    private void Start()
    {
        UpdateValue = new Action(DoUpdateValue);
        EventManager.StartListening(EventManager.EventName.PLAYER_DAMAGED, UpdateValue);
        DoUpdateValue();
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EventManager.EventName.PLAYER_DAMAGED, UpdateValue);
    }
}
