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

public class Pen : MonoBehaviour
{
    private int row;
    private int column;
    private PenState currentState;
    private int unlockCost;
    private AnimalData currentAnimal;
    private ShelterGridManager gridManager;

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
}