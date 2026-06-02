using UnityEngine;

public class ReputacionCard : HUDCard
{
   
    public void SetReputacion(float reputacion)
    {
        SetValue(reputacion.ToString("0"));
    }
}
