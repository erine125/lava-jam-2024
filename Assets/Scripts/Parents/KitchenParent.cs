using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class KitchenParent : Parent
{

    // Parameters \\

    public Activity activity { private get; set; }


    // Storage \\

    private TextMeshProUGUI titleText, choiceText, opt1Text, opt2Text;
    private TextMeshProUGUI dishNameText, dishFlavorText, dishSepText, recipeListText,
        recipeFlavorText, recipeBeginText;
    private Button envelopeLeft, envelopeRight, recipePaper;
    private SpriteRenderer[] listedIngredients;

    private Dish optionLeft, optionRight;


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

        listedIngredients = new SpriteRenderer[4];
        for (int i = 0; i < listedIngredients.Length; i++)
        {
            listedIngredients[i] = GameObject.Find("KitListed" + (i+1)).GetComponent<SpriteRenderer>();
        }
    }

    public override void Begin()
    {
        base.Begin();

        recipePaper.gameObject.SetActive(false);
        envelopeLeft.gameObject.SetActive(activity == Activity.RECIPE);
        envelopeRight.gameObject.SetActive(activity == Activity.RECIPE);
        foreach (SpriteRenderer sr in listedIngredients)
        {
            sr.sprite = null;
        }

        if (activity == Activity.RECIPE)
        {
            ChooseDishes();
            GetTintOverlay().color = new Color(0, 0, 0, 0.92f);   
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
        }
    }

    public override void Update()
    {
        base.Update();
    }


    // Utility \\

    private void ChooseText ()
    {
        if (activity == Activity.RECIPE)
        {
            switch (manager.currentRound)
            {
                case 1:
                    titleText.text = "ROUND 1 - APPETIZER";
                    break;
                case 2:
                    titleText.text = "ROUND 2 - MAIN COURSE";
                    break;
                case 3:
                    titleText.text = "ROUND 3 - DESSERT";
                    break;
            }
            choiceText.text = "What should I make?";
            opt1Text.text = "Something\nLocal"; //TODO
            opt2Text.text = "Something\nFancy"; //TODO

            dishNameText.text = "";
            dishFlavorText.text = "";
            dishSepText.text = "";
            recipeListText.text = "";
            recipeFlavorText.text = "";
            recipeBeginText.text = "";
        }
        else
        {
            // TODO
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


    // Structure \\

    public enum Activity
    {
        RECIPE,
        COOKING
    }
}
