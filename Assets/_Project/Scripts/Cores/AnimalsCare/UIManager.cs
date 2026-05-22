using UnityEngine;

public class UIManager : MonoBehaviour

{
    public static UIManager Instance;

    [SerializeField] private AnimalInfoPanel animalPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowAnimalPanel(Animal animal)
    {
        animalPanel.gameObject.SetActive(true);

        animalPanel.UpdateInfo(animal);
    }
}
