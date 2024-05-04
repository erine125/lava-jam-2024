using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrateParent : Parent
{

    // Storage \\

    public Page page { private get; set; }

    private TextMeshProUGUI titleText;


    // Triggers \\

    public override void Awake()
    {
        base.Awake();

        titleText = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>();
    }

    public override void Begin()
    {
        base.Begin();

        ChooseText();
    }


    // Utility \\

    private void ChooseText ()
    {
        switch (page)
        {
            case Page.INTRO:
                titleText.text = "WELCOME TO LAVA CHEF!";
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
        RULES
    }

}
