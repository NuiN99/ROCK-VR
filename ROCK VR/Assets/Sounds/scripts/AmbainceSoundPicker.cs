using System.Collections;
using System.Collections.Generic;
using NuiN.NExtensions;
using UnityEngine;

public class AmbainceSoundPicker : MonoBehaviour
{
    public AudioSource sound;
    public AudioClip cave, forst;
    [SerializeField] CollisionProxy proxy;

    void OnEnable()
    {
        proxy.TriggerEnter += Sound;
    }

    void OnDisable()
    {
        proxy.TriggerEnter -= Sound;
    }

    void Sound(Collider other)
    {
        if (other.gameObject.CompareTag("caveSound"))
        {
            sound.clip = cave;
            
        }
        else if (other.gameObject.CompareTag("forstSound"))
        {
            sound.clip = forst;
        }

        sound.Stop();
        sound.Play();
    }
}
