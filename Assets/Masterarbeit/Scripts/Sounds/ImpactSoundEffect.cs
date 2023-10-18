using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundEffect : MonoBehaviour
{
    public AudioClip terrainClip;
    public AudioClip waterClip;

    bool playedClip;

    private void OnCollisionEnter(Collision collision)
    {
        if(!playedClip)
        {
            switch(collision.gameObject.tag)
            {
                case "Terrain":
                    AudioSource.PlayClipAtPoint(terrainClip, collision.GetContact(0).point);
                    break;
                case "Water":
                    AudioSource.PlayClipAtPoint(waterClip, collision.GetContact(0).point);
                    break;

            }
            playedClip = true;
        }
    }
}
