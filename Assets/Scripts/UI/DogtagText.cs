using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class DogtagText : MonoBehaviour
{
    private TMP_Text dogtagText = null;
    public Action UpdateValue;
    private void Awake()
    {
        dogtagText = GetComponent<TMP_Text>();
    }

    private void DoUpdateValue()
    {
        dogtagText.text = string.Format("{0}", GameManager.Instance.PlayerInfo.dogtag);
    }
    private void Start()
    {
        UpdateValue = new Action(DoUpdateValue);
        EventManager.StartListening(EventManager.EventName.GET_DOGTAG, UpdateValue);
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EventManager.EventName.GET_DOGTAG, UpdateValue);
    }
}
