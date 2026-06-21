using UnityEngine;

public class AdoptionHeartEffect : MonoBehaviour
{

    
    [SerializeField] private AudioClip popClip;

    public void PlayPopSound()
    {
        AudioSource.PlayClipAtPoint(popClip, transform.position);
    }

    public void DestroyAnimal()
    {

        Destroy(transform.root.gameObject);
    }
}
