using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Parent : MonoBehaviour
{
    [HideInInspector]
    public Manager manager;

    // To Override \\

    /// <summary>
    /// Should only be called by the Manager
    /// </summary>
    public abstract void Begin();

    public virtual void InputButton (string message) { }

    public virtual void InputClick (Vector2 pos) { }


    // Exposed \\

    public void SetActive (bool active)
    {
        gameObject.SetActive(active);
    }


    // Functionality \\

    protected SpriteRenderer GetBackground ()
    {
        return GameObject.Find("Background").GetComponent<SpriteRenderer>();
    }


    // Type \\

    public enum Type
    {
        TITLE,
        KITCHEN,
        PANTRY,
        NARRATE
    }

}
