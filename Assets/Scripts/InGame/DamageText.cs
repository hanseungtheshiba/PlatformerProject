using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Pool;

[RequireComponent(typeof(TMP_Text))]
public class DamageText : MonoBehaviour, IResettable
{
    [SerializeField]
    private float duration = 0.5f;
    [SerializeField]
    private Vector2 targetPosition = new Vector2(0f, 1f);
    [SerializeField]
    private float thresholdX = 0.01f;
    [SerializeField]
    private TMP_Text tmpText = null;
    private Vector2 calculatedPosition = Vector2.zero;

    public event EventHandler Hide;

    public void Reset()
    {
        tmpText.color = Color.white;
        gameObject.SetActive(false);
    }
    
    public void Show(int damage)
    {
        if (tmpText == null)
        {
            tmpText = GetComponent<TMP_Text>();
        }
        gameObject.SetActive(true);
        calculatedPosition = (Vector2)transform.position + targetPosition;
        calculatedPosition.x += UnityEngine.Random.Range(-thresholdX, thresholdX);
        StartCoroutine(DoShowAnimation(damage));
    }

    private IEnumerator DoShowAnimation(int damage)
    {
        tmpText.text = string.Format("{0}", damage);
        Color color = tmpText.color;
        float alpha = tmpText.color.a;
        
        Vector2 position = transform.position;
        for (float timer = 0f; timer < 1f; timer += Time.unscaledDeltaTime / duration)
        {
            color.a = Mathf.SmoothStep(alpha, 0f, timer);
            position = Vector2.Lerp(position, calculatedPosition, timer);
            tmpText.color = color;
            transform.position = position;
            yield return null;
        }

        Hide?.Invoke(this, null);
    }
}
