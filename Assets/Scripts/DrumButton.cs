/*
 * Represents a button that corresponds to a part of the drum set
 */

using UnityEngine;
using Vuforia;

public class DrumButton : MonoBehaviour
{
    // connects the Vuforia Virtual Button with an animation to trigger
    public VirtualButtonBehaviour button;
    public Animator anim;

    void Start()
    {
        button.RegisterOnButtonPressed(OnButtonPressed);
    }

    // set trigger when vuforia virtual button is pressed
    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        anim.SetTrigger("Play");
    }
}