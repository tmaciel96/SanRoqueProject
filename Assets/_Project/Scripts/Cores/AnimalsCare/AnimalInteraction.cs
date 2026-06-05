using UnityEngine;

public class AnimalInteraction : MonoBehaviour, IInventoryItemTarget
{
    [Header("Item Preview")]
    [SerializeField] private Color validItemColor = new Color(1f, 0.86f, 0.35f, 1f);

    private Animal animal;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

    private void Awake()
    {
        animal = GetComponent<Animal>();
        CacheRenderers();
    }

    private void OnMouseDown()
    {
        if (ItemSelectionManager.Instance != null && ItemSelectionManager.Instance.TryApplyToTarget(this))
            return;

        Debug.Log("Click sobre animal");

        if (UIManager.Instance != null)
            UIManager.Instance.ShowAnimalPanel(animal);
    }

    public bool CanReceiveItem(ShelterItemData itemData)
    {
        return itemData != null && itemData.TargetType == ShelterItemTargetType.Animal && animal != null;
    }

    public bool ApplyItem(ShelterItemData itemData)
    {
        if (!CanReceiveItem(itemData))
            return false;

        float previousHunger = animal.Hunger;
        float previousHappiness = animal.Happiness;
        float previousHealth = animal.Health;

        animal.Hunger -= itemData.HungerReduction;
        animal.Happiness += itemData.HappinessIncrease;
        animal.Health += itemData.HealthIncrease;

        Debug.Log(
            $"Item aplicado: {itemData.DisplayName} sobre {animal.AnimalName}. " +
            $"Hambre {previousHunger:0}->{animal.Hunger:0}, " +
            $"Felicidad {previousHappiness:0}->{animal.Happiness:0}, " +
            $"Salud {previousHealth:0}->{animal.Health:0}.");

        return true;
    }

    public void SetItemPreview(bool active, ShelterItemData itemData)
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            CacheRenderers();

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            spriteRenderers[i].color = active
                ? Color.Lerp(originalColors[i], validItemColor, 0.45f)
                : originalColors[i];
        }
    }

    private void CacheRenderers()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;
    }
}
