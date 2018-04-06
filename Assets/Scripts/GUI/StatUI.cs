using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour {

    public Text attackStat;
    public Text strengthStat;
    public Text defenseStat;

    private PlayerStats stats;

    private void Start()
    {
        stats = PlayerController.instance.GetStats();
        EquipmentManager.instance.onEquipmentChangedCallback += OnEquipmentChanged;
        ContextMenu.instance.onUpdateUICallback += UpdateUI;
        UpdateUI();
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        attackStat.text = FormatText("Attack", stats.damage.GetBase(), stats.damage.GetModifier());
        strengthStat.text = FormatText("Strength", stats.strength.GetBase(), stats.strength.GetModifier());
        defenseStat.text = FormatText("Defense", stats.defense.GetBase(), stats.defense.GetModifier());
    }

    private string FormatText(string stat, int baseStat, int modStat)
    {
        return stat + ": base " + baseStat + " (+ " + modStat + " mod)";
    }
}
