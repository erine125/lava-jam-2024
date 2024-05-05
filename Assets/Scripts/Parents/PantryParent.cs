using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PantryParent : Parent
{

    // Parameters \\

    public Sprite[] backgroundFrames;
    public float OscillatePerSecond;


    // State \\

    private float timeSinceOsc;
    private int oscillateFrame;


    public override void Begin()
    {
        base.Begin();
    }

    public override void Update ()
    {
        base.Update();

        HandleOscillating();
    }


    // Utility \\

    private void HandleOscillating ()
    {
        timeSinceOsc += Time.deltaTime;

        if (timeSinceOsc >= 1f / OscillatePerSecond)
        {
            oscillateFrame = (oscillateFrame + 1) % 12;
            GetBackground().sprite = backgroundFrames[oscillateFrame];
            timeSinceOsc = 0;
        }
    }

    
}
