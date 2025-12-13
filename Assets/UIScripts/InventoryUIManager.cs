using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject _inventoryPanel;

    [Header("Input")]
    [SerializeField] private KeyCode _toggleKey = KeyCode.I;

    [Header("Button Reference")]
    [SerializeField] private Button _closeButton; // Кнопка для закрытия инвентаря

    [Header("Buttons open")]
    [SerializeField] private GameObject _openInventoryButton;
    [SerializeField] private GameObject _openMenuButton;

    public bool IsOpen { get; private set; }

    void Start()
    {
        CloseInventory();

        // Подписываем кнопку на метод закрытия инвентаря
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(CloseInventory);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            ToggleInventory();
            if (IsOpen)
            {     
             _openMenuButton.SetActive(true);
             _openInventoryButton.SetActive(true);
            }

            else
            {
             _openMenuButton.SetActive(false);
             _openInventoryButton.SetActive(false);
            }
           
        } 
    }

    public void ToggleInventory()
    {
        if (IsOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        _inventoryPanel.SetActive(true);
        IsOpen = true;
        _openMenuButton.SetActive(false);
        _openInventoryButton.SetActive(false);
    }

    public void CloseInventory()
    {
        _inventoryPanel.SetActive(false);
        IsOpen = false;
        _openMenuButton.SetActive(true);
        _openInventoryButton.SetActive(true);
    }
}
