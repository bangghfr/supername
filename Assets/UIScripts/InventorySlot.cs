using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

// Класс для управления отдельным слотом инвентаря
public class InventorySlot : MonoBehaviour
{
    public Image icon;      // Иконка предмета
    public Button removeButton; // Кнопка удаления предмета

    public Item item;      // Хранимый предмет

    // Метод для добавления предмета в слот
    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    // Метод для очистки слота
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    // Метод для удаления предмета по нажатию кнопки
    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

    // Метод для использования предмета
    public void UseItem()
    {
        if (item != null)
        {
            item.Use(); // Вызываем действие предмета
        }
    }
}