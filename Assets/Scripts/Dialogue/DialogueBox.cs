using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using Pool;

public class DialogueBox : MonoBehaviour, IResettable
{
    [SerializeField]
    private SpriteRenderer faceSpriteLeft = null;
    [SerializeField]
    private SpriteRenderer faceSpriteRight = null;
    [SerializeField]
    private TMP_Text dialogueText = null;
    [SerializeField]
    private float dialogueDuration = 0.02f;
    [SerializeField]
    private float nextPageDuration = 2f;
    [SerializeField]
    private float hideDuration = 0.5f;
    [SerializeField]
    private string spriteAtlasAddress = "";
    [SerializeField]
    private string buttonName = "Fire2";

    private List<Dialogue> dialogueList = null;
    private SpriteAtlas spriteAtlas = null;
    private Coroutine doShowAnimation = null;
    private Coroutine playDialogue = null;

    public Action Hide;
    
    public void Init(DialogueGroup dialogueGroup)
    {
        dialogueList = dialogueGroup.dialogueList;
        Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasAddress).Completed += SpriteAtlasLoaded;
        Hide = new Action(OnHide);
    }

    public void Show()
    {
        doShowAnimation = StartCoroutine(DoShowAnimation());        
    }

    public void OnHide()
    {
        if(doShowAnimation != null) { StopCoroutine(doShowAnimation); }
        if(playDialogue != null) { StopCoroutine(playDialogue); }

        faceSpriteLeft.gameObject.SetActive(false);
        faceSpriteRight.gameObject.SetActive(false);
        // 대화창 리셋용
        dialogueText.text = " ";

        if(gameObject.activeInHierarchy) StartCoroutine(DoHideAnimation());
    }
    private IEnumerator DoShowAnimation()
    {
        float currentScale = 0f;
        float tempScale = 0f;
        for (float timer = 0f; timer < 1f; timer += Time.unscaledDeltaTime / hideDuration)
        {
            tempScale = Mathf.SmoothStep(currentScale, 1f, timer);
            transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            yield return null;
        }
        
        transform.localScale = Vector3.one;

        playDialogue = StartCoroutine(PlayDialogue());
    }

    private IEnumerator DoHideAnimation()
    {        
        float currentScale = 1f;
        float tempScale = 0f;
        for (float timer = 0f; timer < 1f; timer += Time.unscaledDeltaTime / hideDuration)
        {
            tempScale = Mathf.SmoothStep(currentScale, 0f, timer);
            transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            yield return null;
        }

        transform.SetParent(null);
        gameObject.SetActive(false);
        transform.localScale = Vector3.one;
    }

    private IEnumerator PlayDialogue()
    {
        // spriteAtlas가 로딩될 때까지 기다린다
        yield return new WaitUntil(() => spriteAtlas != null);           

        foreach(Dialogue dialogue in dialogueList)
        {
            faceSpriteLeft.gameObject.SetActive(false);
            faceSpriteRight.gameObject.SetActive(false);

            // 얼굴 방향 판단
            switch (dialogue.facePosition)
            {
                case Dialogue.FacePosition.LEFT:
                    faceSpriteLeft.gameObject.SetActive(true);
                    faceSpriteLeft.sprite = spriteAtlas.GetSprite(dialogue.faceSpriteName);
                    break;
                case Dialogue.FacePosition.RIGHT:
                    faceSpriteRight.gameObject.SetActive(true);
                    faceSpriteRight.sprite = spriteAtlas.GetSprite(dialogue.faceSpriteName);
                    break;
            }

            dialogueText.text = GetPartialPayload(dialogue.dialogue, 0);

            for (int i = 1; i <= dialogue.dialogue.Length; i++)
            {
                dialogueText.text = GetPartialPayload(dialogue.dialogue, i);
                yield return new WaitForDone(dialogueDuration, () => Input.GetButton(buttonName));
            }

            yield return new WaitForDone(nextPageDuration, () => Input.GetButtonUp(buttonName));
        }

        yield return new WaitForDone(nextPageDuration, () => Input.GetButtonUp(buttonName));

        Hide?.Invoke();
    }

    private string GetPartialPayload(string s, int alreadyTyped)
    {
        var tempStr = new System.Text.StringBuilder(s.Length);
        int count = 0;
        bool payload = true;
        for(int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if(c == '<')
            {
                payload = false;
            }

            if(payload && count < alreadyTyped)
            {
                count++;
                tempStr.Append(c);
            }
            else if (!payload)
            {
                tempStr.Append(c);
            }

            if (c == '>')
            {
                payload = true;
            }
        }

        return tempStr.ToString();
    }

    private void SpriteAtlasLoaded(AsyncOperationHandle<SpriteAtlas> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                spriteAtlas = obj.Result;
                break;
            case AsyncOperationStatus.Failed:
                Debug.LogError("Sprite load failed. Using default Sprite.");
                break;
        }
    }

    public void Reset()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
}
