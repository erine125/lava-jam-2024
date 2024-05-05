using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrateParent : Parent
{

    // Storage \\

    public Page page { private get; set; }

    private TextMeshProUGUI titleText, rightText, contText;


    // Triggers \\

    public override void Awake()
    {
        base.Awake();

        titleText = GameObject.Find("NarTitleText").GetComponent<TextMeshProUGUI>();
        rightText = GameObject.Find("NarRightText").GetComponent<TextMeshProUGUI>();
        contText = GameObject.Find("NarContText").GetComponent<TextMeshProUGUI>();
    }

    public override void Begin()
    {
        base.Begin();

        ChooseText();
    }

    public override void InputClick(Vector2 pos)
    {
        if (!transition.IsTransitioning ())
        {
            if (page == Page.INTRO)
            {
                transition.StartLoadingOut(Type.KITCHEN);
                manager.kitchenParent.activity = KitchenParent.Activity.RECIPE;
            }
        }
    }


    // Utility \\

    private void ChooseText ()
    {
        switch (page)
        {
            case Page.INTRO:
                titleText.text = "WELCOME TO LAVA CHEF!";
                rightText.text = "TODO - Hello and welcome. You are about to compete in Lava Chef."
                    + "There will be three rounds, one for each course!";
                contText.text = "(Click to Continue)";
                break;
            case Page.RULES:
                titleText.text = "RULES OF LAVA CHEF";
                break;
        }
    }


    // Structure \\

    public enum Page
    {
        INTRO,
        DISH_SUCCESS,
        DISH_FAIL,
        RULES,
        DIED_LAVA,
        DIED_ERUPT,
        WINNER
    }

}
