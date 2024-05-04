using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleParent : Parent
{

    // Triggers \\

    public override void InputButton (string message)
    {
        if (!transition.isTransitioning ())
        {
            if (message == "Start")
            {
                transition.StartLoadingOut(Type.NARRATE);
                manager.narrateParent.page = NarrateParent.Page.INTRO;
            }
            else if (message == "Rules")
            {
                transition.StartLoadingOut(Type.NARRATE, 0);
                manager.narrateParent.page = NarrateParent.Page.RULES;
            }
        }
    }

}
