/*
 * Handles playing the audio file for a corresponding drum button & turning on the display lights
 */

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundLight : MonoBehaviour
{
    private AudioSource aSource;

    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    // connected with an animation event - plays sound & turns on light
    void PlayThisSoundNow()
    {
        aSource.Play();
        transform.Find("light").gameObject.SetActive(true);
    }

    // connected with an animation event - turns off light at end of animation
    void StopLighting()
    {
        transform.Find("light").gameObject.SetActive(false);
    }
}
