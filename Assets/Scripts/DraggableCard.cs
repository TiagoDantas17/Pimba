using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Cartão que se arrasta com o rato dentro da área da mesa (o objeto pai).
/// Ao clicar, vem para a frente dos outros cartões.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DraggableCard : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float dragScale = 1.06f;

    private RectTransform rect;
    private RectTransform bounds;
    private Vector2 grabOffset;
    private Vector3 baseScale;

    private void Awake()
    {
        rect = (RectTransform)transform;
        bounds = rect.parent as RectTransform;
        baseScale = rect.localScale;
    }

    public void OnPointerDown(PointerEventData e) => rect.SetAsLastSibling();

    public void OnBeginDrag(PointerEventData e)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(bounds, e.position, e.pressEventCamera, out var p)) return;
        grabOffset = rect.anchoredPosition - p;
        rect.localScale = baseScale * dragScale;
    }

    public void OnDrag(PointerEventData e)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(bounds, e.position, e.pressEventCamera, out var p)) return;

        Vector2 pos = p + grabOffset;
        Rect r = bounds.rect;
        pos.x = Mathf.Clamp(pos.x, r.xMin, r.xMax);
        pos.y = Mathf.Clamp(pos.y, r.yMin, r.yMax);
        rect.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData e) => rect.localScale = baseScale;
}
