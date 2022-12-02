/*
 * Handles playing the audio file for a corresponding drum button
 */

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySound : MonoBehaviour
{
    private AudioSource aSource;

    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    // connected with an animation event
    void PlayThisSoundNow()
    {
        aSource.Play();
        
    }
}
