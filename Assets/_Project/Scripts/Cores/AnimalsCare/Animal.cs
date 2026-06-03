using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] AnimalType animalType;
    private string animalName;

    [SerializeField] ReactionType reactionType;
    private Animator animator;
    private bool isReacting;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barkClip;
    [SerializeField] private AudioClip sadClip;


    /*private string description;
    private int age;
    private double weight;*/

    [SerializeField] private float hunger;
    [SerializeField] private float thirst;
    [SerializeField] private float happiness;
    [SerializeField] private float health;

    [SerializeField] private bool isSick = false;
    private bool wasSick;
    [SerializeField] private bool isSleeping = false;
    [SerializeField] private bool isAwake = false;


    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        audioSource = GetComponentInChildren<AudioSource>();

    }

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

    
    public bool isHealthy => Health > 40f;
    public bool IsSick => Health <= 40f;
    public bool isCritical => Health <= 10f;


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
        hunger += amount;
    }

    public void Drink(float amount) 
    { 
        thirst += amount;
    }

    public void Heal(float amount)
    {
        health += amount;
    }

    public void Pet(float amount)
    {
        happiness += amount;
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

    private void Update()
    {
        Hunger -= Time.deltaTime * 2f;
        Thirst -= Time.deltaTime * 3f;

        animator.SetBool("isSick", IsSick);

        bool currentlySick = IsSick;

        if (currentlySick && !wasSick) 
        {
            audioSource.PlayOneShot(sadClip);
        }

        wasSick = currentlySick;
        
    }

    public void PlayReaction(ReactionType reaction)
    {
        if (isReacting) return;

        StartCoroutine(ReactionRoutine(reaction));
        
    }

    private IEnumerator ReactionRoutine(ReactionType reaction)
    {
        isReacting = true;

        switch (reactionType)
        {
            case ReactionType.Bark:
                animator.SetTrigger("Bark");
                audioSource.PlayOneShot(barkClip);

                break;
            case ReactionType.Happy:
                animator.SetTrigger("Happy");
                audioSource.PlayOneShot(barkClip);
                break;
        }

        yield return new WaitForSeconds(0.5f);

        isReacting = false;
    }

    public void ApplyCare(CareType careType)
    {
        switch (careType)
        {
            case CareType.Food:
                if(isCritical) break;

                Feed(isSick ? 10f : 20f);

                if (isHealthy) PlayReaction(ReactionType.Bark);
                
                break;
            case CareType.Water:
                if (isCritical) break;

                Drink(isSick ? 10f : 20f);

                if(isHealthy) PlayReaction(ReactionType.Bark);

                break;
            case CareType.Petting:
                if (isCritical) break;

                Pet(isSick ? 0f : 20f);

                if (isHealthy) PlayReaction(ReactionType.Happy);


                break;
            case CareType.Medicine:
                Heal(100f);
                PlayReaction(ReactionType.Happy);
                break;

        }

            
    }

    

}
