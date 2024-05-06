using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TitleParent : Parent
{
    public AudioSource audioSource; 
    public AudioClip startSound; 

    // Triggers \\

    public override void InputButton (string message)
    {
        if (!transition.IsTransitioning ())
        {
            if (message == "Start")
            {
                audioSource.PlayOneShot(startSound, 0.5f);
                manager.narrateParent.page = NarrateParent.Page.INTRO;
                transition.StartLoadingOut(Type.NARRATE);
            }
            else if (message == "Rules")
            {
                audioSource.PlayOneShot(startSound, 0.5f);
                transition.StartLoadingOut(Type.RULES, 0);
            }
        }
    }

}
