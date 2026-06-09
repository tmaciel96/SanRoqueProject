using UnityEngine;
using TMPro;

public class NotificationBadge : MonoBehaviour
{
    [SerializeField] private GameObject badgeContainer;
    [SerializeField] private TextMeshProUGUI countText;

    public void UpdateBadge(int count)
    {
        if (count <= 0)
        {
            badgeContainer.SetActive(false);
        }
        else
        {
            badgeContainer.SetActive(true);
            countText.text = count.ToString();
        }
    }
}