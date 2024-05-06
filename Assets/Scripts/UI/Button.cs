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
    private bool IsMouseOver = false; 


    // Exposed \\

    public Collider2D GetCollider ()
    {
        return gameObject.GetComponent<Collider2D>();
    }

    public bool OverlapsPoint (Vector2 point)
    {
        return GetCollider().OverlapPoint(point);
    }

    // bool IsMouseOverGameObject()
    // {
    //     Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //     RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

    //     if (hit.collider != null && hit.collider.gameObject == this.gameObject)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    void OnMouseOver()
    {
        IsMouseOver = true;
    }

    void OnMouseExit()
    {
        IsMouseOver = false;
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

            // Vector2 mousePos = Input.mousePosition;
            // Vector2 viewportPos = Camera.main.ScreenToViewportPoint(mousePos);
            // Vector2 worldPos = Camera.main.ViewportToWorldPoint(new Vector2(viewportPos.x, viewportPos.y));


            // Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) * Manager.PIX_TO_UNI;
            // Vector2 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition) * Manager.PIX_TO_UNI;
            // // mousePos -= new Vector3(0.5f, 0.5f, 0.0f) * factor;
            // Debug.Log(mousePos);
            // Debug.Log(worldPos);
            bool highlight = IsMouseOver && !Input.GetMouseButton(0);

            // bool highlight = OverlapsPoint(mousePos) && !Input.GetMouseButton(0);

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
