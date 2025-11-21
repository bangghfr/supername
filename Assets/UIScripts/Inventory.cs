using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

// Класс инвентаря для управления предметами
public class Inventory : MonoBehaviour
{
    public static Inventory instance; // Синглтон для удобного доступа

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Inventory уже существует!");
            return;
        }
        instance = this;
    }

    public List<Item> items = new List<Item>(); // Список предметов в инвентаре
    public int space = 20; // Максимальное количество предметов

    // Делегат для уведомления об изменениях в инвентаре
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    // Метод для добавления предмета
    public bool Add(Item item)
    {
        if (items.Count >= space)
        {
            Debug.Log("Нет места в инвентаре!");
            return false;
        }

        items.Add(item);
        onItemChangedCallback?.Invoke(); // Уведомляем об изменениях
        return true;
    }

    // Метод для удаления предмета
    public void Remove(Item item)
    {
        items.Remove(item);
        onItemChangedCallback?.Invoke(); // Уведомляем об изменениях
    }
}