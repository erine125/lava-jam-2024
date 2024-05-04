using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public const float UNI_TO_PIX = 36f;
    public const float PIX_TO_UNI = 1f / UNI_TO_PIX;

    // Parameters \\

    public Parent TitleParent;
    public Parent KitchenParent;
    public Parent PantryParent;
    public Parent NarrateParent;


    // Storage \\

    private Parent activeParent;
    private List<Button> activeButtons;

    private bool mouseWasPressed;

    public int finishedDishes;


    // Triggers \\

    void Start()
    {
        activeButtons = new List<Button>();
        finishedDishes = 0;

        InitializeParents();
        ChooseActiveParent(Parent.Type.TITLE);
    }

    void Update()
    {
        HandleMouseInput();
    }


    // Exposed \\

    public void ChooseActiveParent (Parent.Type parent)
    {
        // set the correct active parent
        TitleParent.SetActive(parent == Parent.Type.TITLE);
        KitchenParent.SetActive(parent == Parent.Type.KITCHEN);
        PantryParent.SetActive(parent == Parent.Type.PANTRY);
        NarrateParent.SetActive(parent == Parent.Type.NARRATE);
        switch (parent)
        {
            case Parent.Type.TITLE:
                TitleParent.Begin();
                activeParent = TitleParent;
                break;
            case Parent.Type.KITCHEN:
                KitchenParent.Begin();
                activeParent = KitchenParent;
                break;
            case Parent.Type.PANTRY:
                PantryParent.Begin();
                activeParent = PantryParent;
                break;
            case Parent.Type.NARRATE:
                NarrateParent.Begin();
                activeParent = NarrateParent;
                break;
        }

        // find all button objects
        activeButtons.Clear();
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.activeInHierarchy)
            {
                Button b = go.GetComponent<Button>();
                if (b != null)
                {
                    activeButtons.Add(b);
                }
            }
        }
    }


    // Utility \\

    private void InitializeParents ()
    {
        TitleParent.manager = this;
        KitchenParent.manager = this;
        PantryParent.manager = this;
        NarrateParent.manager = this;
    }

    private void HandleMouseInput ()
    {
        bool mousePressed = Input.GetMouseButton(0);
        Vector2 point = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        point *= PIX_TO_UNI;

        if (!mousePressed && mouseWasPressed)
        {
            bool handeled = false;

            // check for buttons
            foreach (Button b in activeButtons)
            {
                if (b.OverlapsPoint(point))
                {
                    activeParent.InputButton(b.message);
                    handeled = true;
                    break;
                }
            }
            // pass on unhandeled clicks
            if (!handeled)
            {
                activeParent.InputClick(point);
            }
        }

        mouseWasPressed = mousePressed;
    }

}
