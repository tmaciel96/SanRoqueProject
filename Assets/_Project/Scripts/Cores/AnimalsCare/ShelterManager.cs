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

        animal.Initialize("testID", "Firulais", AnimalType.Dog, 100f, 100f, 100.0f, 100.0f);
        Debug.Log(animal.ToString());
    }
}
