using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] AnimalType animalType;
    private string animalName;

    ReactionType reactionType;
    private Animator animator;
    private bool isReacting;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barkClip;
    [SerializeField] private AudioClip sadClip;
    [SerializeField] private AudioClip angryClip;


    /*private string description;
    private int age;
    private double weight;*/

    [SerializeField] private float hunger;
    [SerializeField] private float thirst;
    [SerializeField] private float happiness;
    [SerializeField] private float health;

    private float lowNeedsTimer = 0f;
    private float randomDiseaseTimer = 0f;
    private bool wasSick;
    
    [SerializeField] private bool isSleeping = false;
    [SerializeField] private bool isAwake = false;

    [Header("Visual Variants")]
    [SerializeField] private GameObject[] visualVariants;

    public int VariantCount => visualVariants != null && visualVariants.Length > 0
        ? visualVariants.Length
        : 1;

    public AnimalType Species => animalType;

    public void Awake()
    {
        ResolveVariant(-1);
        BindComponents();
    }

    public void Initialize(int id, string name, AnimalType type, float hunger, float thirst, float happiness, float health, int variantIndex = -1)
    {
        this.id = id;
        this.animalName = name;
        this.animalType = type;
        this.hunger = hunger;
        this.thirst = thirst;
        this.happiness = happiness;
        this.health = health;

        if (variantIndex >= 0)
        {
            ResolveVariant(variantIndex);
            BindComponents();
        }
    }

    private void ResolveVariant(int index)
    {
        if (visualVariants == null || visualVariants.Length == 0)
            return;

        for (int i = 0; i < visualVariants.Length; i++)
        {
            if (visualVariants[i] != null)
                visualVariants[i].SetActive(false);
        }

        int chosen = index >= 0 && index < visualVariants.Length
            ? index
            : Random.Range(0, visualVariants.Length);

        if (visualVariants[chosen] != null)
            visualVariants[chosen].SetActive(true);
    }

    private void BindComponents()
    {
        animator = GetComponentInChildren<Animator>();
        if (audioSource == null)
            audioSource = GetComponentInChildren<AudioSource>();
    }
    public int ID => id;

    public bool IsHealthy => Health > 40f;
    public bool IsSick => Health <= 40f;
    public bool IsCritical => Health <= 10f;

    public bool isSad => Happiness <= 40f;


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
        
        Hunger -= Time.deltaTime * 5f;
        Thirst -= Time.deltaTime * 2f;

        //Debug.Log(Happiness);

        CheckingHealth();

        animator.SetBool("isSick", IsSick);
        animator.SetBool("isSad", isSad && !IsSick);

        bool currentlySick = IsSick;

        if (currentlySick && !wasSick) audioSource.PlayOneShot(sadClip);

        if(wasSick && !currentlySick) PlayReaction(ReactionType.Happy);
        
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

        switch (reaction)
        {
            case ReactionType.Bark:
                animator.SetTrigger("Bark");
                audioSource.PlayOneShot(barkClip);

                break;
            case ReactionType.Happy:
                animator.SetTrigger("Happy");
                audioSource.PlayOneShot(barkClip);
                break;
            case ReactionType.Love:
                animator.SetTrigger("Love");
                break;
            case ReactionType.Drinking:
                animator.SetTrigger("Drinking");
                break;
            case ReactionType.Eating:
                animator.SetTrigger("Eating");
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
                if (IsCritical) {
                    Debug.Log(IsCritical);
                    audioSource.PlayOneShot(angryClip);
                    break; }
                

                Feed(IsSick ? 10f : 20f);

                if (IsHealthy) PlayReaction(ReactionType.Eating);

                break;
            case CareType.Water:
                if (IsCritical) break;

                Drink(IsSick ? 10f : 20f);

                if (IsHealthy) PlayReaction(ReactionType.Drinking);

                break;
            case CareType.Petting:
                if (IsCritical)  break;

                Pet(IsSick ? 0f : 2f);

                if (IsHealthy) PlayReaction(ReactionType.Love);
                
                break;
            case CareType.Medicine:
                
                if(IsHealthy) break;

                Heal(100f);
                Feed(60f);
                Drink(60f);
                
                break;

        }

            
    }
    private void CheckingHealth()
    {
        if (!IsHealthy)
        {
            
            Health -= Time.deltaTime * 1f;
            Happiness -= Time.deltaTime * 1f;
            
            return;
        }

        //Rule 1
        if (Hunger <= 10 || Thirst <= 10)
        {
            Health = 40f;
            Happiness = 40f;
            return;
        }

        //Rule 2
        if (Hunger <= 40 && Thirst <= 40) {
            Health = 40f;
            Happiness = 40f;
            return; 
        }

            
        //Rule 3
        randomDiseaseTimer += Time.deltaTime;

        if (randomDiseaseTimer >= 120)
        {
            CheckRandomDisease();

            randomDiseaseTimer = 0f;
        } else { 
            randomDiseaseTimer = 0; 
        }

        
    }

    private void CheckRandomDisease()
    {
        float chance = Random.Range(0f, 100f);

        if (chance <= 5 ) Health = 40;
    }

    private void checkingHappiness()
    {

    }

}
