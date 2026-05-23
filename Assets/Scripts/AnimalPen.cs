using UnityEngine;

public class AnimalPen : MonoBehaviour
{
    [SerializeField] Animal currentAnimal;

    public bool isOccupied => currentAnimal != null;

    public Animal CurrentAnimal => currentAnimal;

    /*[SerializeField]private FoodBowl foodBowl;
    [SerializeField] private WaterBowl waterBowl;*/


    public void AssignAnimal(Animal animal)
    {
        currentAnimal = animal;
    }

    public void RemoveAnimal()
    {
        currentAnimal = null;
    }

    public void FeedAnimal(float amount)
    {
        currentAnimal.Feed(amount);
    }

    public void GiveWater(float amount)
    {
        currentAnimal.Drink(amount);
    }
}
