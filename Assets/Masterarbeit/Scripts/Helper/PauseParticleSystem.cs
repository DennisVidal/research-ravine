using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseParticleSystem : MonoBehaviour
{
    public ParticleSystem particles;

    private void Start()
    {
        if(particles == null)
        {
            particles = GetComponent<ParticleSystem>();
        }
    }

    void OnEnable()
    {
        GameManager.Instance.onGameplayResumed += OnGameplayResumed;
        GameManager.Instance.onGameplayPaused += OnGameplayPaused;
    }
    void OnDisable()
    {
        GameManager.Instance.onGameplayResumed -= OnGameplayResumed;
        GameManager.Instance.onGameplayPaused -= OnGameplayPaused;
    }

    void OnGameplayResumed()
    {
        particles.Play(true);
    }
    void OnGameplayPaused()
    {
        particles.Pause(true);
    }
}
