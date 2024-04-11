using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerSounds : MonoBehaviour
{
    [Header("Weapon")]
    public AudioClip[] whipSounds;

    [Header("Footsteps")]
    public List<AudioClip> footsteps;

    public AudioSource weaponSource;
    public AudioSource footstepSource;

    public XRController xrController;
    // Start is called before the first frame update
    void Start()
    {
        footstepSource = GetComponent<AudioSource>();
        xrController = GetComponent<XRController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 primary2DAxisValue;
        //Input.GetKeyDown("w")|| Input.GetKeyDown("s")|| Input.GetKeyDown("a")|| Input.GetKeyDown("d")
        //if ((xrController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out primary2DAxisValue);
        //{
        //    if(primary2DAxisValue.magnitude > 0.1f)
        //    {
        //        PlayFootstep();
        //    }
        //    else
        //    {
        //        StopFootstep();
        //    }
            
        //}
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
