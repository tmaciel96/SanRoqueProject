using UnityEngine;

public enum PenState { Locked, Available, Empty, Occupied }

[System.Serializable]
public class AnimalData
{
    public string animalName;
    public AnimalType species;
    public float hunger = 100f;
    public float affection = 50f;
}

public class Pen : MonoBehaviour, IInventoryItemTarget
{
    [Header("Upgrade")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel = 3;
    [SerializeField] private Color validItemColor = new Color(1f, 0.86f, 0.35f, 1f);

    private int row;
    private int column;
    private PenState currentState;
    private int unlockCost;
    private AnimalData currentAnimal;
    private ShelterGridManager gridManager;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

    [Header("Visual States")]
    [SerializeField] private GameObject visualLocked;
    [SerializeField] private GameObject visualAvailable;
    [SerializeField] private GameObject visualEmpty;
    [SerializeField] private GameObject animalContainer;

    public void Init(int r, int c, PenState initialState, int cost, ShelterGridManager manager)
    {
        row = r;
        column = c;
        currentState = initialState;
        unlockCost = cost;
        gridManager = manager;

        UpdateVisuals();
    }

    /*public void OnClicked()
    {
        if (ItemSelectionManager.Instance != null && ItemSelectionManager.Instance.TryApplyToTarget(this))
            return;

        switch (currentState)
        {
            case PenState.Available:
                BuyPen();
                break;
            case PenState.Empty:
                // Temporary MVP placeholder to simulate rescue/assignment
                AssignAnimal(new AnimalData { animalName = "Oliver", species = AnimalType.Dog });
                break;
            case PenState.Occupied:
                InteractWithAnimal();
                break;
            case PenState.Locked:
                Debug.Log("This pen is locked. Unlock adjacent pens first!");
                break;
        }
    }*/

    private void BuyPen()
    {
        // TODO: Validate with currency system later
        currentState = PenState.Empty;
        UpdateVisuals();
        
        // Crucial: Tell the manager to unlock neighbors
        gridManager.UnlockNeighbors(row, column);
    }

    public void SetAvailable()
    {
        if (currentState == PenState.Locked)
        {
            currentState = PenState.Available;
            UpdateVisuals();
        }
    }

    public void AssignAnimal(AnimalData newAnimal)
    {
        currentAnimal = newAnimal;
        currentState = PenState.Occupied;
        UpdateVisuals();
    }

    private void InteractWithAnimal()
    {
        if (currentAnimal == null) return;
        currentAnimal.hunger = Mathf.Min(currentAnimal.hunger + 20f, 100f);
        Debug.Log($"Interacted with {currentAnimal.animalName}. Hunger: {currentAnimal.hunger}");
    }

    private void UpdateVisuals()
    {
        // First, turn off all visual objects to avoid overlapping
        visualLocked.SetActive(false);
        visualAvailable.SetActive(false);
        visualEmpty.SetActive(false);
        animalContainer.SetActive(false);

        // Turn on only the one needed for the current state
        switch (currentState)
        {
            case PenState.Locked:
                visualLocked.SetActive(true);
                break;

            case PenState.Available:
                visualAvailable.SetActive(true);
                break;

            case PenState.Empty:
                visualEmpty.SetActive(true);
                break;

            case PenState.Occupied:
                visualEmpty.SetActive(true); // Keep the kennel background visible
                animalContainer.SetActive(true); // Show the animal sprite on top
                break;
        }
    }

    public bool CanReceiveItem(ShelterItemData itemData)
    {
        return itemData != null
            && itemData.TargetType == ShelterItemTargetType.Pen
            && currentState != PenState.Locked
            && currentLevel < maxLevel;
    }

    public bool ApplyItem(ShelterItemData itemData)
    {
        if (!CanReceiveItem(itemData))
            return false;

        int previousLevel = currentLevel;
        currentLevel = Mathf.Min(currentLevel + itemData.PenUpgradeAmount, maxLevel);
        Debug.Log($"Corral mejorado: nivel {previousLevel}->{currentLevel} en fila {row}, columna {column}.");
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
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;
    }
}
