using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbainceSoundPicker : MonoBehaviour
{
    public AudioSource sound;
    public AudioClip cave, forst;
    // Start is called before the first frame update
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)

    {
        Debug.Log("I touch something");
        if (other.gameObject.tag == "caveSound")
        {
            Debug.Log("caveSound trigger");
            sound.clip = cave;
            
        }
        else if (other.gameObject.tag == "forstSound")
        {
            Debug.Log("forstSound trigger");
            sound.clip = forst;
        }

        sound.Stop();
        sound.Play();
    }
}
