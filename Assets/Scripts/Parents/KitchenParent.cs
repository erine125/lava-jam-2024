using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KitchenParent : Parent
{

    // Parameters \\

    public Activity activity { private get; set; }

    public Sprite[] backgroundFrames;
    public float OscillatePerSecond = 10f;
    public float BasePotShakeTime = 1f;

    public Sprite[] potFrames;

    public AudioSource audioSource; 
    public AudioClip dropInPotSound; 


    // Storage \\

    private TextMeshProUGUI titleText, choiceText, opt1Text, opt2Text;
    private TextMeshProUGUI dishNameText, dishFlavorText, dishSepText, recipeListText,
        recipeFlavorText, recipeBeginText;
    private Button envelopeLeft, envelopeRight, recipePaper;
    private SpriteRenderer[] listedIngredients;
    private TextMeshProUGUI cookInstructText;
    private SpriteRenderer potRender;

    private Dish optionLeft, optionRight;


    // State \\

    private float timeSinceOsc = 0;
    private int oscillateFrame = 0;

    private int ingredientsInPot = 0;

    private float timeSincePot = 0;
    private bool potGoingRight = true;
    private int potFrame = 1;

    private Ingredient[] availableIngredients;


    // Triggers \\

    public override void Awake()
    {
        base.Awake();

        titleText = GameObject.Find("KitTitleText").GetComponent<TextMeshProUGUI>();
        choiceText = GameObject.Find("KitChoiceText").GetComponent<TextMeshProUGUI>();
        opt1Text = GameObject.Find("KitOpt1Text").GetComponent<TextMeshProUGUI>();
        opt2Text = GameObject.Find("KitOpt2Text").GetComponent<TextMeshProUGUI>();

        envelopeLeft = GameObject.Find("EnvelopeLeft").GetComponent<Button>();
        envelopeRight = GameObject.Find("EnvelopeRight").GetComponent<Button>();
        recipePaper = GameObject.Find("RecipePaper").GetComponent<Button>();

        dishNameText = GameObject.Find("KitDishNameText").GetComponent<TextMeshProUGUI>();
        dishFlavorText = GameObject.Find("KitDishFlavorText").GetComponent<TextMeshProUGUI>();
        dishSepText = GameObject.Find("KitDishSepText").GetComponent<TextMeshProUGUI>();
        recipeListText = GameObject.Find("KitRecipeListText").GetComponent<TextMeshProUGUI>();
        recipeFlavorText = GameObject.Find("KitRecipeFlavorText").GetComponent<TextMeshProUGUI>();
        recipeBeginText = GameObject.Find("KitRecipeBeginText").GetComponent<TextMeshProUGUI>();

        cookInstructText = GameObject.Find("KitInstructionText").GetComponent<TextMeshProUGUI>();
        potRender = GameObject.Find("KitPot").GetComponent<SpriteRenderer>();

        listedIngredients = new SpriteRenderer[4];
        for (int i = 0; i < listedIngredients.Length; i++)
        {
            listedIngredients[i] = GameObject.Find("KitListed" + (i+1)).GetComponent<SpriteRenderer>();
        }
    }

    public override void Begin()
    {
        base.Begin();

        ingredientsInPot = 0;
        potFrame = 1;
        potRender.sprite = potFrames[1];

        recipePaper.gameObject.SetActive(false);
        envelopeLeft.gameObject.SetActive(activity == Activity.RECIPE);
        envelopeRight.gameObject.SetActive(activity == Activity.RECIPE);
        itemFrame.SetActive(activity == Activity.COOKING);

        foreach (SpriteRenderer sr in listedIngredients)
        {
            sr.sprite = null;
        }

        if (activity == Activity.RECIPE)
        {
            ChooseDishes();
            GetTintOverlay().color = new Color(0, 0, 0, 0.95f);   
            potRender.sprite = null;
        }
        else if (activity == Activity.COOKING)
        {
            PrepareIngredients();
            potRender.sprite = potFrames[1];
        }
        ChooseText();
    }

    public override void InputButton (string message)
    {
        if (!transition.IsTransitioning ())
        {
            if (message == "EnvelopeLeft" || message == "EnvelopeRight")
            {
                // hide current stuff
                envelopeLeft.gameObject.SetActive(false);
                envelopeRight.gameObject.SetActive(false);
                opt1Text.text = "";
                opt2Text.text = "";
                titleText.text = "";
                choiceText.text = "";

                // pull up recipe
                recipePaper.gameObject.SetActive(true);
                manager.chosenDish = (message == "EnvelopeLeft" ? optionLeft : optionRight);
                UpdateRecipeUi();
                manager.RefreshButtons();
            }
            else if (message == "RecipePaper")
            {
                transition.StartLoadingOut(Type.PANTRY);
            }
            else if (message == "PanItem1")
            {
                audioSource.PlayOneShot(dropInPotSound, 0.5f);
                ingredientsInPot += 1;
                availableIngredients[0] = null;
                itemSprites[0].sprite = null;
                CheckDoneCooking();
            }
            else if (message == "PanItem2")
            {
                audioSource.PlayOneShot(dropInPotSound, 0.5f);
                ingredientsInPot += 1;
                availableIngredients[1] = null;
                itemSprites[1].sprite = null;
                CheckDoneCooking();
            }
            else if (message == "PanItem3")
            {
                audioSource.PlayOneShot(dropInPotSound, 0.5f);
                ingredientsInPot += 1;
                availableIngredients[2] = null;
                itemSprites[2].sprite = null;
                CheckDoneCooking();
            }
            else if (message == "PanItem4")
            {
                audioSource.PlayOneShot(dropInPotSound, 0.5f);
                ingredientsInPot += 1;
                availableIngredients[3] = null;
                itemSprites[3].sprite = null;
                CheckDoneCooking();
            }
        }
    }

    public override void Update()
    {
        base.Update();

        HandleOscillating();
    }


    // Utility \\

    private void ChooseText ()
    {
        dishNameText.text = "";
        dishFlavorText.text = "";
        dishSepText.text = "";
        recipeListText.text = "";
        recipeFlavorText.text = "";
        recipeBeginText.text = "";

        cookInstructText.text = activity == Activity.COOKING ? "Click ingredients to add them!" : "";

        if (activity == Activity.RECIPE)
        {
            switch (manager.currentRound)
            {
                case 1:
                    titleText.text = "ROUND 1 - APPETIZER";
                    choiceText.text = "Your chance to make a good first impression, and show the world that Volcanic cuisine isn’t to be overlooked... What will you make?";
                    opt1Text.text = "Something\nLocal";
                    opt2Text.text = "Something\nTrendy";
                    break;
                case 2:
                    titleText.text = "ROUND 2 - MAIN COURSE";
                    choiceText.text = "The main dish. Show the judges that you cook with heart and soul... What will you make?";
                    opt1Text.text = "Something\nLuxurious";
                    opt2Text.text = "Something\nComforting";
                    break;
                case 3:
                    titleText.text = "ROUND 3 - DESSERT";
                    choiceText.text = "This is it. You’ve made it so far. Now finish off with something to remember... What will you make?";
                    opt1Text.text = "Something\nNostalgic";
                    opt2Text.text = "Something\nIconic";
                    break;
            }
        }
    }

    private void ChooseDishes ()
    {
        optionLeft = null;
        optionRight = null;

        foreach (KeyValuePair<string, Dish> entry in manager.dishes)
        {
            if ((int) entry.Value.course == manager.currentRound)
            {
                if (optionLeft == null)
                {
                    optionLeft = entry.Value;
                }
                else
                {
                    optionRight = entry.Value;
                    break;
                }
            }
        }
    }

    private void UpdateRecipeUi ()
    {
        Dish d = manager.chosenDish;

        dishNameText.text = d.displayName;
        dishFlavorText.text = d.flavorText;
        dishSepText.text = "INGREDIENTS";
        recipeBeginText.text = "(Click to Begin)";

        recipeListText.text = "";
        recipeFlavorText.text = "";

        for (int i = 0; i < d.ingredients.Length; i++)
        {
            string dishIng = d.ingredients[i];
            Ingredient objIng = manager.ingredients[dishIng];

            recipeListText.text += objIng.displayName + "\n";
            recipeFlavorText.text += objIng.flavorText + "\n\n\n\n";
            listedIngredients[i].sprite = objIng.sprite;
        }
    }

    private void HandleOscillating ()
    {
        timeSinceOsc += Time.deltaTime;
        if (timeSinceOsc >= 1f / OscillatePerSecond)
        {
            oscillateFrame = (oscillateFrame + 1) % 41;
            GetBackground().sprite = backgroundFrames[oscillateFrame];
            timeSinceOsc = 0;
        }

        if (ingredientsInPot > 0)
        {
            timeSincePot += Time.deltaTime;
            if (timeSincePot >= BasePotShakeTime / (ingredientsInPot))
            {
                if (potFrame == 2 || potFrame == 0)
                {
                    potFrame = 1;
                }
                else
                {
                    potFrame = potGoingRight ? 2 : 0;
                    potGoingRight = !potGoingRight;
                }
                potRender.sprite = potFrames[potFrame];
                timeSincePot = 0;
            }
        }
    }

    private void PrepareIngredients ()
    {
        availableIngredients = manager.heldIngredients.ToArray ();

        for (int i = 0; i < availableIngredients.Length; i++)
        {
            if (availableIngredients[i] != null)
            {
                itemSprites[i].sprite = availableIngredients[i].sprite;
            }
        }

        CheckDoneCooking();
    }

    private void CheckDoneCooking()
    {
        for (int i = 0; i < availableIngredients.Length; i++)
        {
            if (availableIngredients[i] != null)
            {
                return;
            }
        }

        // done cooking, check if correct
        bool wasCorrect = true;
        foreach (string shouldHave in manager.chosenDish.ingredients)
        {
            bool found = false;
            foreach (Ingredient actuallyHas in manager.heldIngredients)
            {
                if (shouldHave == actuallyHas.filename)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                wasCorrect = false;
                break;
            }
        }

        // leave the kitchen
        manager.narrateParent.page = wasCorrect ?
            NarrateParent.Page.DISH_SUCCESS : NarrateParent.Page.DISH_FAIL;
        transition.StartLoadingOut(Type.NARRATE, 0.2f);
    }


    // Structure \\

    public enum Activity
    {
        RECIPE,
        COOKING
    }
}
