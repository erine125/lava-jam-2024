using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesParent : Parent
{
    // Triggers \\

    public override void InputClick(Vector2 pos)
    {
        transition.StartLoadingOut(Type.TITLE, 0);
    }
}
