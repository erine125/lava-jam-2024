using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleParent : Parent
{

    // Triggers \\

    public override void InputButton (string message)
    {
        if (!transition.IsTransitioning ())
        {
            if (message == "Start")
            {
                manager.narrateParent.page = NarrateParent.Page.INTRO;
                transition.StartLoadingOut(Type.NARRATE);
            }
            else if (message == "Rules")
            {
                transition.StartLoadingOut(Type.RULES, 0);
            }
        }
    }

}
