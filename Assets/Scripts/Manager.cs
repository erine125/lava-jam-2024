using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public const float UNI_TO_PIX = 36f;
    public const float PIX_TO_UNI = 1f / UNI_TO_PIX;

    // Parameters \\
    [Header("Global Settings")]
    [Range(0.25f, 4f)] public float fadeOutMultiplier = 1.5f;
    [Range(0.25f, 4f)] public float fadeInMultiplier = 2.5f;
    public float pauseBlackOnFade = 0.5f;

    [Header("Object Pointers")]
    public TitleParent titleParent;
    public RulesParent rulesParent;
    public KitchenParent kitchenParent;
    public PantryParent pantryParent;
    public NarrateParent narrateParent;

    [Header("Assets")]
    public TextAsset dishTsv;
    public TextAsset ingredientTsv;
    public Sprite[] dishSprites, ingredientSprites; // must be same order as the TSV file


    // Shared State \\

    [HideInInspector] public int currentRound = 1;
    [HideInInspector] public float fadeMultiplier = 0f; // 0 to start, but 1 is default after that
    [HideInInspector] public Dish chosenDish;
    [HideInInspector] public Ingredient[] heldIngredients;


    // Storage \\

    private Parent activeParent;
    public Parent.Type activeParentType { private set; get; }
    private List<Button> activeButtons;
    private PostEffect tintEffect;

    private bool mouseWasPressed;

    [HideInInspector] public Dictionary<string, Dish> dishes;
    [HideInInspector] public Dictionary<string, Ingredient> ingredients;


    // Triggers \\

    void Start()
    {
        activeButtons = new List<Button>();
        currentRound = 1;
        tintEffect = gameObject.GetComponent<PostEffect>();

        LoadDishesAndIngredients();
        heldIngredients = new Ingredient[4];

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
        this.activeParentType = parent;

        titleParent.gameObject.SetActive(parent == Parent.Type.TITLE);
        rulesParent.gameObject.SetActive(parent == Parent.Type.RULES);
        kitchenParent.gameObject.SetActive(parent == Parent.Type.KITCHEN);
        pantryParent.gameObject.SetActive(parent == Parent.Type.PANTRY);
        narrateParent.gameObject.SetActive(parent == Parent.Type.NARRATE);
        switch (parent)
        {
            case Parent.Type.TITLE:
                titleParent.Begin();
                activeParent = titleParent;
                break;
            case Parent.Type.RULES:
                rulesParent.Begin();
                activeParent = rulesParent;
                break;
            case Parent.Type.KITCHEN:
                kitchenParent.Begin();
                activeParent = kitchenParent;
                break;
            case Parent.Type.PANTRY:
                pantryParent.Begin();
                activeParent = pantryParent;
                break;
            case Parent.Type.NARRATE:
                narrateParent.Begin();
                activeParent = narrateParent;
                break;
        }

        RefreshButtons();
    }

    public void RefreshButtons ()
    {
        activeButtons.Clear();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
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

    public void UpdateTintEffect (float strength)
    {
        tintEffect.material.SetFloat("_Strength", strength);
    }

    public int CountHeldIngredients ()
    {
        int ct = 0;
        for (int i = 0; i < heldIngredients.Length; i++)
        {
            ct += (heldIngredients[i] == null) ? 0 : 1;
        }
        return ct;
    }
    public void RemoveAllHeldIngredients ()
    {
        for (int i = 0; i < heldIngredients.Length; i++)
        {
            heldIngredients[i] = null;
        }
    }
    public int AddToHeldIngredients (Ingredient ingredient)
    {
        for (int i = 0; i < heldIngredients.Length; i++)
        {
            if (heldIngredients[i] == null)
            {
                heldIngredients[i] = ingredient;
                return i;
            }
        }
        return -1;
    }


    // Utility \\

    private void InitializeParents ()
    {
        titleParent.manager = this;
        kitchenParent.manager = this;
        pantryParent.manager = this;
        narrateParent.manager = this;
        rulesParent.manager = this;
    }

    private void HandleMouseInput ()
    {
        bool mousePressed = Input.GetMouseButton(0);
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // point *= PIX_TO_UNI;

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

    private void LoadDishesAndIngredients ()
    {
        dishes = new Dictionary<string, Dish>();
        string[] dishLines = dishTsv.text.Split('\n');
        for (int i = 1; i < dishLines.Length; i++)
        {  
            Dish dish = Dish.CreateFromLine(dishLines[i]);
            dish.sprite = dishSprites[i-1];
            dishes.Add(dish.filename.Trim(), dish);
        }

        ingredients = new Dictionary<string, Ingredient>();
        string[] ingLines = ingredientTsv.text.Split('\n');
        for (int i = 1; i < ingLines.Length; i++)
        {
            Ingredient ing = Ingredient.CreateFromLine(ingLines[i]);
            ing.sprite = ingredientSprites[i - 1];
            ingredients.Add(ing.filename.Trim(), ing);
        }
    }

}
