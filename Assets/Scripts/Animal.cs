using Unity.VisualScripting;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] AnimalType animalType;
    private string animalName;

    

    /*private string description;
    private int age;
    private double weight;*/

    [SerializeField] private float hunger;
    [SerializeField] private float thirst;
    [SerializeField] private float happiness;
    [SerializeField] private float health;

    [SerializeField] private bool isAlive;
    [SerializeField] private bool isSleeping = false;
    [SerializeField] private bool isAwake = false;


    public void Initialize (int id, string name, float hunger, float thirst, float happiness, float health )
    {
        this.id = id;
        this.animalName = name;
        this.hunger = hunger;
        this.thirst = thirst;
        this.happiness = happiness;
        this.health = health;
    }


    public int ID => id;
    public string AnimalName
    {
        get => animalName;
        set => animalName = value;
    }

    public float Hunger
    {
        get => hunger;
        set => hunger = Mathf.Clamp(value,0f,100f);
    }

    public float Thirst
    {
        get => thirst;
        set => thirst = Mathf.Clamp(value, 0f, 100f);
    }

    public float Happiness
    {
        get => happiness; 
        set => happiness = Mathf.Clamp(value, 0f, 100f);
    }

    public float Health
    {
        get => health; 
        set => health = Mathf.Clamp(value, 0f, 100f);
    }

    public void Feed(float amount)
    {
        hunger -= amount;
    }

    public void Drink(float amount) 
    { 
        thirst -= amount;
    }

    public override string ToString()
    {
        return "ID: " + id +
            "\nNombre: " + animalName +
            " Hambre: " + hunger +
            " Sed: " + thirst +
            " Felicidad: " + happiness +
            " Salud: " + health; 
    }

}
