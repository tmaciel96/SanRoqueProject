using System.Collections;
using UnityEngine;

public class AdoptionHeartEffect : MonoBehaviour
{

    
    [SerializeField] private AudioClip popClip;

    public void PlayPopSound()
    {
        AudioSource.PlayClipAtPoint(popClip, transform.position);
    }

    /*public void DestroyAnimal()
    {

        StartCoroutine(DestroyAnimalNextFrame());
    }

    private IEnumerator DestroyAnimalNextFrame()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(transform.root.gameObject);
    }*/
}
