using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    [SerializeField] private Canvas canvas;           // Канвас для UI
    [SerializeField] private Image iconImage;         // Иконка предмета
    [SerializeField] private InventorySlot slot;      // Слот, к которому привязан этот предмет

    [Header("World")]
    [SerializeField] private Transform playerTransform; // Для выброса предмета на землю

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.IsEmpty())
            return;

        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slot.IsEmpty())
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter != null)
        {
            InventorySlot targetSlot = eventData.pointerEnter.GetComponent<InventorySlot>();
            if (targetSlot != null)
            {
                SwapItems(slot, targetSlot);
                return;
            }
        }

        // Если кликнули вне слота и нажата правая кнопка мыши — выброс предмета
        if (Input.GetMouseButton(1))
        {
            DropItem(slot);
        }

        // Вернуть предмет на место, если не поменяли
        rectTransform.anchoredPosition = originalPosition;
    }

    private void SwapItems(InventorySlot from, InventorySlot to)
    {
        Item tempItem = from.currentItem;
        int tempAmount = from.amount;

        from.SetItem(to.currentItem, to.amount);
        to.SetItem(tempItem, tempAmount);
    }

    private void DropItem(InventorySlot from)
    {
        if (from.IsEmpty())
            return;

        // Проверяем, есть ли префаб для выброса
        if (from.currentItem.worldPrefab != null)
        {
            GameObject dropped = Instantiate(from.currentItem.worldPrefab,
                                             playerTransform.position + Vector3.forward,
                                             Quaternion.identity);
            Rigidbody rb = dropped.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(playerTransform.forward * 2f + Vector3.up * 2f, ForceMode.Impulse);
        }

        from.ClearSlot();
    }

}
