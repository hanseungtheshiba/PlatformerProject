using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class HPBar : MonoBehaviour
{
    private Slider hpSlider = null;

    public Action UpdateValue;

    private void Awake()
    {
        hpSlider = GetComponent<Slider>();        
    }

    private void DoUpdateValue()
    {        
        hpSlider.value = GameManager.Instance.GetHealthRate();
    }

    private void Start()
    {        
        UpdateValue = new Action(DoUpdateValue);
        EventManager.StartListening(EventManager.EventName.PLAYER_DAMAGED, UpdateValue);        
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EventManager.EventName.PLAYER_DAMAGED, UpdateValue);
    }
}
