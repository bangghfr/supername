using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDImages : MonoBehaviour
{
    [Header("Player Reference")]
    public MoveController player;

    [Header("Bars")]
    public Image hpBar;
    public Image armorBar;
    public Image sanityBar;

    [Header("Bar Max Widths (px)")]
    public float hpMaxWidth = 200f;
    public float armorMaxWidth = 200f;
    public float sanityMaxWidth = 200f;

    void Update()
    {
        if (!player) return;

        // Обновляем ширину полоски здоровья
        if (hpBar)
            hpBar.rectTransform.sizeDelta = new Vector2(player._playerHp / 100f * hpMaxWidth, hpBar.rectTransform.sizeDelta.y);

        // Обновляем ширину полоски брони
        if (armorBar)
            armorBar.rectTransform.sizeDelta = new Vector2(player._playerArmor / 100f * armorMaxWidth, armorBar.rectTransform.sizeDelta.y);

        // Обновляем ширину полоски психики
        if (sanityBar)
            sanityBar.rectTransform.sizeDelta = new Vector2(player._playerSanity / 100f * sanityMaxWidth, sanityBar.rectTransform.sizeDelta.y);
    }
}
