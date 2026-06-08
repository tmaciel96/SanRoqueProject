using UnityEngine;
using TMPro;

public class ShoppingUI : BasePanel
{
    private const int FoodCost = 250;
    private const int MedicineCost = 500;
    private const int ToysCost = 150;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI txtCantAlimento;
    [SerializeField] private TextMeshProUGUI txtCantMedicina;
    [SerializeField] private TextMeshProUGUI txtCantJuguetes;

    // pausesTime = true por defecto desde BasePanel (fin del día sí pausa)

    /// <summary>
    /// Abre el panel actualizando los datos ANTES de mostrarlo,
    /// lo que evita el bug del frame anterior visible.
    /// </summary>
    public override void Open()
    {
        ShowSummary(); // datos primero
        base.Open();   // activar después
    }

    private void ShowSummary()
    {
        
        int totalFood = 10;
        int totalMedicine = 10;
        int totalToys = 10;
        
        txtCantAlimento.text = $"{totalFood}";
        txtCantMedicina.text = $"{totalMedicine}";
        txtCantJuguetes.text = $"{totalToys}";
    }

}