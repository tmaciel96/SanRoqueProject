using TMPro;
using UnityEngine;

public class AnimalInfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hungerText;
    [SerializeField] private TMP_Text healthText;

    public void UpdateInfo(Animal animal)
    {
        nameText.text = animal.AnimalName;
        hungerText.text = animal.Hunger.ToString();
        healthText.text = animal.Health.ToString();
    }
}
