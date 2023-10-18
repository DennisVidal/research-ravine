using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour
{
    public AudioSource audioSource;

    public Vector2 minMaxVolume = new Vector2(0.0f, 1.0f);
    public Vector2 minMaxSpeed;

    public Vehicle vehicle;
    void Awake()
    {
        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        float volumeFactor = (vehicle.currentMovementSpeed - minMaxSpeed.x) / (minMaxSpeed.y - minMaxSpeed.x);
        audioSource.volume = Mathf.Lerp(minMaxVolume.x, minMaxVolume.y, volumeFactor);

        if (vehicle.currentMovementSpeed < minMaxSpeed.x)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }
}
