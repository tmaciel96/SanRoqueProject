using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente para cada botón "+" del sidebar de tienda.
/// Asignar el ShopItemData correspondiente en el Inspector.
/// </summary>
[RequireComponent(typeof(Button))]
public class ShopSidebarButton : MonoBehaviour
{
    [SerializeField] private ShopItemData itemData;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (itemData == null)
        {
            Debug.LogWarning("ShopSidebarButton: itemData no asignado.");
            return;
        }

        UIManager.Instance.ShowShopItemPanel(itemData);
    }

    
}