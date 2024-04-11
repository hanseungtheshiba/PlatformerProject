using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class PanelBase : MonoBehaviour
{
    [SerializeField]
    protected float showDuration = 1f;
    [SerializeField]
    protected float hideDuration = 1f;
    [SerializeField]
    private Button buttonOk = null;
    [SerializeField]
    private Button buttonCancel = null;

    private CanvasGroup canvasGroup = null;

    public Action Show;
    public Action Hide;

    protected bool isShowing = false;

    private Coroutine showCoroutine = null;
    private Coroutine hideCoroutine = null;

    private bool isInitialized = false;

    private void Start()
    {
        Init();
    }

    public void ShowOrHide()
    {
        if (isShowing)
        {
            Hide?.Invoke();
            isShowing = false;
        }
        else
        {
            Show?.Invoke();
            isShowing = true;
        }
    }

    protected virtual void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Show = new Action(OnShow);
        Hide = new Action(OnHide);
        buttonOk?.onClick.AddListener(() => OnClickOK());
        buttonCancel?.onClick.AddListener(() => OnClickCancel());
        EventSystem.current.SetSelectedGameObject(buttonCancel.gameObject);
        isInitialized = true;
    }

    private void OnEnable()
    {
        if (!isInitialized) {
            Init(); 
        }
    }

    protected virtual void OnShow()
    {
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        showCoroutine = StartCoroutine(DoShowAnimation());        
    }

    private IEnumerator DoShowAnimation()
    {        
        float alpha = canvasGroup.alpha;
        for (float timer = 0f; timer < 1f; timer += Time.unscaledDeltaTime / showDuration) 
        {
            canvasGroup.alpha = Mathf.SmoothStep(alpha, 1f, timer);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        Time.timeScale = 0f;
    }    

    protected virtual void OnHide()
    {
        if (showCoroutine != null) StopCoroutine(showCoroutine);
        hideCoroutine = StartCoroutine(DoHideAnimation());
    }

    private IEnumerator DoHideAnimation()
    {
        float alpha = canvasGroup.alpha;
        for (float timer = 0f; timer < 1f; timer += Time.unscaledDeltaTime / hideDuration)
        {
            canvasGroup.alpha = Mathf.SmoothStep(alpha, 0f, timer);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    protected virtual void OnClickOK()
    {
        Hide?.Invoke();
    }

    protected virtual void OnClickCancel()
    {
        Hide?.Invoke();
    }
}
