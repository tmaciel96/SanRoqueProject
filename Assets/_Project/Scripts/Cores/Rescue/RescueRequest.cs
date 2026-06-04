using System;

[Serializable]
public class RescueRequest
{
    public AnimalData animalData;
    public int daysRemaining;

    public RescueRequest(AnimalData animal, int days)
    {
        animalData = animal;
        daysRemaining = days;
    }
}