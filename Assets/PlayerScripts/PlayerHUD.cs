using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public MoveController player;

    public Slider dashCooldownBar;
    public Slider superDashCooldownBar;
    public Text hpText;

    void Update()
    {
        dashCooldownBar.value = player.GetDashCooldown01();
        superDashCooldownBar.value = player.GetSuperDashCooldown01();
        hpText.text = player._playerHp.ToString();
    }
}
