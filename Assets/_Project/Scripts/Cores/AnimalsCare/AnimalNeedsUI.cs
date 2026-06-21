using JetBrains.Annotations;
using UnityEngine;

public class AnimalNeedsUI : MonoBehaviour
{

    [SerializeField] private Animal animal;

    [SerializeField] private GameObject needsContainer;
    [SerializeField] private GameObject adoptionPanel;

    [SerializeField] private NeedBubble hungerBubble;
    [SerializeField] private NeedBubble thirstBubble;
    [SerializeField] private NeedBubble happinessBubble;
    [SerializeField] private NeedBubble healthBubble;
    [SerializeField] private DrainableIcon hungerIcon;
    [SerializeField] private DrainableIcon healthIcon;
    [SerializeField] private DrainableIcon happinessIcon;
    [SerializeField] private DrainableIcon thirstIcon;


    // Update is called once per frame
    void Update()
    {
        if(animal.IsAdoptable)
        {
            needsContainer.SetActive(false);
            adoptionPanel.SetActive(true);
            return;
        }

        needsContainer.SetActive(true);
        adoptionPanel.SetActive(false);

        /*hungerBubble.SetFill(animal.Hunger / 100f);
        thirstBubble.SetFill(animal.Thirst / 100f);
        happinessBubble.SetFill(animal.Happiness / 100f);
        healthBubble.SetFill(animal.Health / 100f);*/

        hungerIcon.SetFill(animal.Hunger / 100f);
        healthIcon.SetFill(animal.Health / 100f);
        happinessIcon.SetFill(animal.Happiness / 100f);
        thirstIcon.SetFill(animal.Thirst / 100f);

    }

    public void AcceptAdoption()
    {
        animal.Adoption();
    }

    public void RejectAdoption()
    {
        animal.RejectAdoption();
    }
}
