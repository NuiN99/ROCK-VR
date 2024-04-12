using NuiN.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerSounds : MonoBehaviour
{
    [Header("Weapon")]
    [SerializeField] AudioClip[] whipSounds;

    [Header("Footsteps")]
    public List<AudioClip> footsteps;

    public AudioSource weaponSource;
    public AudioSource footstepSource;


    [SerializeField] Rigidbody rb;
    [SerializeField] GroundMovement movement;

    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        footstepSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = movement.Grounded && rb.velocity.magnitude > 0.1f;

        //Vector2 primary2DAxisValue;
        //Input.GetKeyDown("w")|| Input.GetKeyDown("s")|| Input.GetKeyDown("a")|| Input.GetKeyDown("d")
       
        if(isMoving)
        {
            StartCoroutine(PlayFootstep());
        }
        else
        {
            StopFootstep();
        }
          
       
    }

    IEnumerator PlayFootstep()
    {
        if (!footstepSource.isPlaying)
        {
            footstepSource.clip = footsteps[Random.Range(0, footsteps.Count)];
            footstepSource.Play();

            yield return new WaitForSeconds(0.5f);
        }
    }
    void StopFootstep()
    {
        footstepSource.Stop();
    }
}
