using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(SpriteRenderer))]
[ExecuteAlways]
public class RectSpriteResizer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void SetSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = rectTransform.rect.size;
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    private void Update()
    {
        SetSize();
    }
}
