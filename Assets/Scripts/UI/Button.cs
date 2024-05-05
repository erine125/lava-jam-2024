using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    // Parameters \\

    public string message;
    public Sprite normalSprite;
    public Sprite highlightSprite;
    public bool DontOverrideSprite;


    // State \\

    public bool wasHighlighted;


    // Storage \\

    private SpriteRenderer spriteRenderer;


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

    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    void Update()
    {
        if (!DontOverrideSprite)
        {
            HandleHighlighting();
        }
    }


    // Utility \\

    private void HandleHighlighting ()
    {
        if (spriteRenderer != null)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) * Manager.PIX_TO_UNI;
            bool highlight = OverlapsPoint(mousePos) && !Input.GetMouseButton(0);

            if (highlight != wasHighlighted)
            {
                if (highlight)
                {
                    spriteRenderer.sprite = highlightSprite;
                }
                else
                {
                    spriteRenderer.sprite = normalSprite;
                }

                // update for next frame
                wasHighlighted = highlight;
            }
        }
    }

}
