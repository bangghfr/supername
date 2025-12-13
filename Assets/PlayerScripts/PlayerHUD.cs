using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public MoveController player;

    public Slider hpSlider;
    public Slider armorSlider;
    public Slider sanitySlider;

    void Update()
    {
        if (!player) return;

        // Обновляем здоровье
        hpSlider.value = Mathf.Clamp01(player._playerHp / 100f);

        // Обновляем броню
        armorSlider.value = Mathf.Clamp01(player._playerArmor / 100f);

        // Обновляем психику
        sanitySlider.value = Mathf.Clamp01(player._playerSanity / 100f);
    }
}
