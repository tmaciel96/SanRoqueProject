using UnityEngine;

public class ShelterManager : MonoBehaviour
{
    [SerializeField] private GameObject dogPrfb;


    public void Start()
    {
        SpawnDog();
    }
    public void SpawnDog()
    {
        GameObject dog = Instantiate(
            dogPrfb,
            Vector3.zero,
            Quaternion.identity
            );

        Animal animal = dog.GetComponent<Animal>();

        animal.Initialize(0, "Firulais", 50.0f, 50.0f, 50.0f, 40.0f);
        Debug.Log(animal.ToString());
    }
}
