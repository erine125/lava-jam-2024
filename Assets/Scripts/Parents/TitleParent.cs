using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleParent : Parent
{

    public Sprite backgroundSprite;

    public override void Begin ()
    {
        SpriteRenderer bgSr = GetBackground();
        bgSr.color = Color.white;
        bgSr.sprite = backgroundSprite;
    }

    public override void InputButton (string message)
    {
        if (message == "Start")
        {
            // TODO add gradual screen transition

            manager.ChooseActiveParent(Type.NARRATE);        
        }
        else if (message == "Rules")
        {
            // TODO show the rules
        }
    }


}
