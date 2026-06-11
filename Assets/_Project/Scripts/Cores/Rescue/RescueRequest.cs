using System;

public enum RescueRequestState
{
    Pending,      // Visible en el mapa, esperando que el player la acepte
    InProgress,   // Rescate en curso
    Completed,    // Llegó al corral
    Expired,      // No fue atendida a tiempo
    Rejected      // Player la rechazó manualmente
}

[Serializable]
public class RescueRequest
{
    public AnimalData animalData;

    // Horas que tiene el player para DECIDIR antes de que expire del mapa
    public int hoursUntilExpiry;

    // Horas que tarda el rescate una vez aceptado
    public int rescueDurationHours;

    // Horas restantes del rescate activo (cuenta regresiva desde rescueDurationHours)
    public int hoursRemainingInRescue;

    // Recompensa al completar
    public int rewardAmount;

    // Punto del mapa donde apareció (índice en RescueMapController.SpawnPoints)
    public int spawnPointIndex;

    public RescueRequestState State { get; private set; } = RescueRequestState.Pending;

    public RescueRequest(AnimalData animal, int hoursUntilExpiry, int rescueDurationHours, int rewardAmount, int spawnPointIndex)
    {
        this.animalData           = animal;
        this.hoursUntilExpiry     = hoursUntilExpiry;
        this.rescueDurationHours  = rescueDurationHours;
        this.hoursRemainingInRescue = rescueDurationHours;
        this.rewardAmount         = rewardAmount;
        this.spawnPointIndex      = spawnPointIndex;
    }

    // --- Transiciones de estado ---

    public void StartRescue()
    {
        if (State != RescueRequestState.Pending)
            throw new InvalidOperationException($"No se puede iniciar rescate desde estado {State}");
        State = RescueRequestState.InProgress;
    }

    public void Reject()
    {
        State = RescueRequestState.Rejected;
    }

    public void Expire()
    {
        State = RescueRequestState.Expired;
    }

    // Retorna true si el rescate se completó con este tick
    public bool TickRescueHour()
    {
        if (State != RescueRequestState.InProgress) return false;

        hoursRemainingInRescue--;
        if (hoursRemainingInRescue <= 0)
        {
            State = RescueRequestState.Completed;
            return true;
        }
        return false;
    }

    // Retorna true si el request expiró del mapa con este tick
    public bool TickExpiryHour()
    {
        if (State != RescueRequestState.Pending) return false;

        hoursUntilExpiry--;
        if (hoursUntilExpiry <= 0)
        {
            Expire();
            return true;
        }
        return false;
    }
}