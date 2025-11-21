using UnityEngine;

// ScriptableObject для описания базовых данных предмета
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName; // Название предмета
    public Sprite icon;     // Иконка предмета
    public bool isStackable; // Можно ли складывать в стопки

    // Метод, который может быть переопределен для уникальных действий с предметом
    public virtual void Use()
    {
        Debug.Log($"Использован предмет: {itemName}");
    }
}