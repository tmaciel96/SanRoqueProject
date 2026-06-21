using UnityEngine;

public class AdoptionHeartEffect : MonoBehaviour
{

    
    [SerializeField] private AudioClip popClip;

    public void PlayPopSound()
    {
        AudioSource.PlayClipAtPoint(popClip, transform.position);
    }

    public void DestroyEffect()
    {

        Destroy(transform.root.gameObject);
    }
}
