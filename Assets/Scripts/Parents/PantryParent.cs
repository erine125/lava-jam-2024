using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantryParent : Parent
{
    public Sprite backgroundSprite;

    public override void Begin()
    {
        SpriteRenderer bgSr = GetBackground();
        bgSr.sprite = backgroundSprite;
        bgSr.color = Color.white;
    }
}
