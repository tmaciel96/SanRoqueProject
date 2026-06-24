using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Animal : MonoBehaviour
{
    [SerializeField] private string id;
    [SerializeField] AnimalType animalType;
    private string animalName;

    private Animator animator;
    private bool isReacting;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barkClip;
    [SerializeField] private AudioClip sadClip;
    [SerializeField] private AudioClip angryClip;
    [SerializeField] private AudioClip pantingClip;
    [SerializeField] private AudioClip drinkClip;
    [SerializeField] private AudioClip eatClip;

    [SerializeField] private GameObject heartEffect;
    [SerializeField] private GameObject visualRoot;

    [SerializeField] private float hunger;
    [SerializeField] private float thirst;
    [SerializeField] private float happiness;
    [SerializeField] private float health;

    private float randomDiseaseTimer = 0f;
    private bool wasSick;
    private bool wasSad;

    [SerializeField] private int daysInShelter = 0;
    public bool IsAdoptable { get; private set; }
    

    [Header("Visual Variants"), Tooltip("Diferentes variantes visuales para esta especie. Asignar en el inspector las variantes que quiera activar." +
        "Estan dentro del prefab principal de la especie, en un GameObject llamado 'VisualVariants'.")]
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

    public void Initialize(string id, string name, AnimalType type, float hunger, float thirst, float happiness, float health, int variantIndex = -1)
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
    public string ID => id;

    public bool IsHealthy => Health > 40f;
    public bool IsSick => Health <= 40f;
    public bool IsCritical => Health <= 10f;

    public bool IsSad => Happiness <= 40f;
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
        //CheckAdoptable();
    }

    public void Drink(float amount) 
    { 
        thirst += amount;
        //CheckAdoptable();
    }

    public void Heal(float amount)
    {
        health += amount;
        //CheckAdoptable();
    }

    public void Pet(float amount)
    {
        happiness += amount;
        //CheckAdoptable();
    }

    /*public override string ToString()
    {
        return "ID: " + id +
            "\nNombre: " + animalName +
            " Hambre: " + hunger +
            " Sed: " + thirst +
            " Felicidad: " + happiness +
            " Salud: " + health; 
    }*/

    private void Update()
    {
        CheckAdoptable();

        if (IsAdoptable) return;

        Hunger -= Time.deltaTime * 0.75f;
        Thirst -= Time.deltaTime * 0.5f;
        Happiness -= Time.deltaTime * 0.30f;
        //Debug.Log(Hunger);

        CheckingHealth();
        
        animator.SetBool("isSick", IsSick);
        animator.SetBool("isSad", IsSad && !IsSick);

        bool currentlySick = IsSick;
        bool currentlySad = IsSad;

        if((currentlySick && !wasSick) || (currentlySad && !wasSad)) audioSource.PlayOneShot(sadClip);

        if((wasSick && !currentlySick) || (wasSad && !currentlySad)) PlayReaction(ReactionType.Happy);
        
        wasSick = currentlySick;
        wasSad = currentlySad;
        
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
                    
                    audioSource.PlayOneShot(angryClip);
                    break; 
                }
                

                Feed(IsSick || IsSad ? 10f : 20f);
                Heal(IsSick ? 0f : 4f);

                if (IsHealthy)
                {
                    PlayReaction(ReactionType.Eating);
                    audioSource.PlayOneShot(eatClip); 
                }

                break;
            case CareType.Water:
                if (IsCritical)
                {
                    
                    audioSource.PlayOneShot(angryClip);
                    break;
                }

                Drink(IsSick || IsSad ? 10f : 20f);
                Heal(IsSick ? 0f : 4f);

                if (IsHealthy)
                {
                    PlayReaction(ReactionType.Drinking);
                    audioSource.PlayOneShot(drinkClip);
                }

                break;
            case CareType.Petting:
                if (IsCritical)
                {                
                    audioSource.PlayOneShot(angryClip);
                    break;
                }

                Pet(IsSick ? 1f : 3.5f);
                Heal(IsSick ? 0f : 4f);

                if (IsHealthy && !IsSad)
                {
                    PlayReaction(ReactionType.Love);
                    
                    audioSource.PlayOneShot(pantingClip);
                }
                
                break;
            case CareType.Medicine:
                
                if(IsHealthy) break;

                Heal(100f);
                Feed(60f);
                Drink(60f);
                Pet(30);
                
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

    //Adopci�n
    private void OnEnable()
    {
        DayManager.OnDayEnded += AddDayInShelter;
    }

    private void OnDisable()
    {
        DayManager.OnDayEnded -= AddDayInShelter;
    }

    private void AddDayInShelter()
    {
        daysInShelter++;

        CheckAdoptable();
    }

    private void CheckAdoptable()
    {
        if (IsAdoptable) return;

        bool statsReady =
            Hunger >= 95f &&
            Thirst >= 95f &&
            Happiness >= 95f &&
            Health >= 95f;

        bool timeReady = daysInShelter >= 2;

        if (statsReady && timeReady)
        {
            IsAdoptable = true;
            Debug.Log($"{AnimalName} está listo para la adopción.");
        }
        

    }

    public void Adoption()
    {

        
        visualRoot.SetActive(false);
        heartEffect.SetActive(true);
        
    }

    public void RejectAdoption()
    {
        IsAdoptable = false;
        daysInShelter = 0;

    }
}
