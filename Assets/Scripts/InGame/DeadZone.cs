using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {        
        EventManager.TriggerEvent(EventManager.EventName.PLAYER_DEAD);
    }
}
