using UnityEngine;
using UnityEngine.UI;

public class ReputationCard : HUDCard
{
    [Header("Reputación")]
    [SerializeField] private Image xpBar;

    private int _level = 1;

    protected override void Awake()
    {
        base.Awake();
        SetLabel("REP.  Nv.1");
        SetValue("0 / 100 XP");
    }

    public void UpdateReputation(int level, int currentXP, int maxXP)
    {
        _level = level;
        SetLabel($"REPUTACIÓN  Nv.{level}");
        SetValue($"{currentXP} / {maxXP} XP");

        if (xpBar != null)
            xpBar.fillAmount = (float)currentXP / maxXP;
    }
}