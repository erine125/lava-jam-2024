using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenParent : Parent
{
    public Sprite backgroundSprite;

    public override void Begin()
    {
        SpriteRenderer bgSr = GetBackground();
        bgSr.color = Color.white;
        bgSr.sprite = backgroundSprite;

    }
}
