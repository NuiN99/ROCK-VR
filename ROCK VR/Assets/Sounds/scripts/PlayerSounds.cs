using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Weapon")]
    public AudioClip[] whipSounds;

    [Header("Footsteps")]
    public List<AudioClip> footsteps;

    public AudioSource weaponSource;
    public AudioSource footstepSource;
    // Start is called before the first frame update
    void Start()
    {
        footstepSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("w")|| Input.GetKeyDown("s")|| Input.GetKeyDown("a")|| Input.GetKeyDown("d"))
        {
            PlayFootstep();
        }
        if (Input.GetKeyUp("w")|| Input.GetKeyUp("s")|| Input.GetKeyUp("a")|| Input.GetKeyUp("d"))
        {
            StopFootstep();
        }
    }

    void PlayFootstep()
    {

        footstepSource.clip = footsteps[Random.Range(0, footsteps.Count)];
        footstepSource.Play();

    }
    void StopFootstep()
    {
        footstepSource.Stop();
    }
}
