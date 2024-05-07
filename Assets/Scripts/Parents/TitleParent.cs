using UnityEngine;

public class TitleParent : Parent
{
    public AudioSource audioSource; 
    public AudioClip startSound; 

    public AudioSource musicSource; 
    public AudioClip menuMusic;

    // Triggers \\

    public override void Begin(){
        base.Begin();
        musicSource.clip = menuMusic; 
        musicSource.Play();
    }

    public override void InputButton (string message)
    {
        if (!transition.IsTransitioning ())
        {
            if (message == "Start")
            {
                audioSource.PlayOneShot(startSound, 0.5f);
                musicSource.Stop();
                manager.narrateParent.page = NarrateParent.Page.INTRO;
                manager.currentRound = 1;
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
