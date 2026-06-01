using System;

[Serializable]
public class RescueRequest
{
    public AnimalData animalData;
    public int expirationDay;

    public RescueRequest(AnimalData animal, int expireDay)
    {
        animalData = animal;
        expirationDay = expireDay;
    }
}