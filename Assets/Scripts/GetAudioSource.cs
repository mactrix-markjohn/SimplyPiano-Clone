using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAudioSource : MonoBehaviour
{
    private AudioSource _audioSource;
    public static AudioSource AudioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioSource = _audioSource;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
