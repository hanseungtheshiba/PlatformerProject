using System;
using System.Collections;
using UnityEngine;
using Pool;

public class DialogueZone : MonoBehaviour
{
    public enum TriggerType
    {
        AREA_ENTERED = 0,
        PRESS_KEY = 1,
        BARK = 2
    }

    [SerializeField]
    private GameObject dialogueBoxPrefab = null;
    [SerializeField]
    private string dialogueGroupId = "";
    [SerializeField]
    private TriggerType dialogueTrigger = TriggerType.AREA_ENTERED;
    [SerializeField]
    private string buttonName = "Fire2";
    [SerializeField]
    private bool isPlayingOnlyOnce = false;

    private Pool<DialogueBox> dialogueBoxPool;
    private DialogueBox currentDialogueBox = null;
    private Transform talkingTarget = null;
    private DialogueGroup dialogueGroup;
    private Coroutine wait;
    private void Start()
    {
        dialogueBoxPool = new Pool<DialogueBox>(new PrefabFactory<DialogueBox>(dialogueBoxPrefab), 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if(dialogueTrigger == TriggerType.AREA_ENTERED)
        {
            if (collision.CompareTag("Player"))
            {
                talkingTarget = collision.transform;
                ShowDialogueBox();
            }            
        }        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) { return; }
        if (dialogueTrigger != TriggerType.PRESS_KEY) { return; }
        //if (isPlayerStaying) { return; }
        //isPlayerStaying = true;
        talkingTarget = collision.transform;
        wait = StartCoroutine(WaitUntilConfirmButton());
    }

    private IEnumerator WaitUntilConfirmButton()
    {
        yield return new WaitUntil(() => Input.GetButton(buttonName));        
        ShowDialogueBox();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        StopAllCoroutines();
        if (isPlayingOnlyOnce && dialogueTrigger == TriggerType.AREA_ENTERED)
        {
            if(currentDialogueBox != null)
            {
                currentDialogueBox.Hide += () => { gameObject.SetActive(false); };                
            }
            return;
        }

        if (collision.CompareTag("Player"))
        {            
            if (currentDialogueBox != null)
            {                
                currentDialogueBox.Hide?.Invoke();                
            }
        }
    }

    private void ShowDialogueBox()
    {        
        StartCoroutine(DoShowDialogueBox());
    }

    private IEnumerator DoShowDialogueBox()
    {
        if (currentDialogueBox == null)
        {
            currentDialogueBox = dialogueBoxPool.Allocate();
        }

        if (currentDialogueBox.isActiveAndEnabled) { yield break; }
        
        dialogueGroup = GameManager.Instance.GetDataManager.GetDialogueGroup(dialogueGroupId);

        yield return new WaitUntil(() => dialogueGroup != null);

        currentDialogueBox.transform.SetParent(talkingTarget, false);
        currentDialogueBox.transform.localPosition = Vector3.zero;        
        currentDialogueBox.gameObject.SetActive(true);        
        currentDialogueBox.Init(dialogueGroup);
        currentDialogueBox.Show();
    }
}
