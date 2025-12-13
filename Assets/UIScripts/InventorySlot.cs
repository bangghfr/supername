using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Item currentItem;     // Предмет в слоте
    public int amount;           // Количество предметов

    [SerializeField] private Image icon; // UI-иконка предмета

    // Установить предмет в слот
    public void SetItem(Item item, int quantity)
    {
        currentItem = item;
        amount = quantity;

        if (icon != null)
        {
            icon.sprite = item != null ? item.icon : null;
            icon.enabled = item != null;
        }
    }

    // Очистить слот
    public void ClearSlot()
    {
        currentItem = null;
        amount = 0;

        if (icon != null)
            icon.enabled = false;
    }

    // Проверить, пустой ли слот
    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void AddItem(Item item, int quantity = 1)
    {
        if (item == null)
            return;

        // Если слот пустой, просто ставим предмет
        if (IsEmpty())
        {
            SetItem(item, quantity);
            return;
        }

        // Если предмет тот же и стекуемый — увеличиваем количество
        if (currentItem == item && item.isStackable)
        {
            amount += quantity;
            return;
        }

        // Если слот занят другим предметом — можно переопределить поведение
        Debug.LogWarning("Слот занят другим предметом!");
    }

}
