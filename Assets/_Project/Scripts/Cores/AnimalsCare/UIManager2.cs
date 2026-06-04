using UnityEngine;

public class UIManager2 : MonoBehaviour

{
    public static UIManager2 Instance;

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
