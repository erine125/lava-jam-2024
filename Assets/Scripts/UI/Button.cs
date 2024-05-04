using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    // Parameters \\

    public string message;
    public Sprite normalSprite;
    public Sprite highlightSprite;


    // State \\

    public bool wasHighlighted;


    // Exposed \\

    public Collider2D GetCollider ()
    {
        return gameObject.GetComponent<Collider2D>();
    }

    public bool OverlapsPoint (Vector2 point)
    {
        return GetCollider().OverlapPoint(point);
    }


    // Triggers \\

    void Update()
    {
        HandleHighlighting();
    }


    // Utility \\

    private void HandleHighlighting ()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) * Manager.PIX_TO_UNI;
        bool highlight = OverlapsPoint(mousePos) && !Input.GetMouseButton(0);

        if (highlight != wasHighlighted)
        {
            if (highlight)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = highlightSprite;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = normalSprite;
            }

            // update for next frame
            wasHighlighted = highlight;
        }
    }

}
