using UnityEngine;

public class CapacidadCard : HUDCard
{
    public void SetCapacidad(float capacidad)
    {
        SetValue(capacidad.ToString("0"));
    }
}
