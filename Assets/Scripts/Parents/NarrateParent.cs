using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrateParent : Parent
{

    // Parameters \\

    public Sprite SplashReading, SplashReadingCrying;


    // Storage \\

    public Page page { private get; set; }

    private TextMeshProUGUI titleText, rightText, contText;
    private SpriteRenderer splashSprite;


    // Triggers \\

    public override void Awake()
    {
        base.Awake();

        titleText = GameObject.Find("NarTitleText").GetComponent<TextMeshProUGUI>();
        rightText = GameObject.Find("NarRightText").GetComponent<TextMeshProUGUI>();
        contText = GameObject.Find("NarContText").GetComponent<TextMeshProUGUI>();
        splashSprite = GameObject.Find("NarSplash").GetComponent<SpriteRenderer>();
    }

    public override void Begin()
    {
        base.Begin();

        ChooseText();
        ChooseSplash();
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
            else if (page == Page.DIED_LAVA || page == Page.DIED_ERUPT || page == Page.DISH_FAIL)
            {
                transition.StartLoadingOut(Type.TITLE);
            }
        }
    }


    // Utility \\

    private void ChooseSplash()
    {
        switch (page)
        {
            case Page.INTRO:
            case Page.WINNER:
            case Page.DISH_SUCCESS:
                splashSprite.sprite = SplashReading;
                break;
            case Page.DISH_FAIL:
                splashSprite.sprite = SplashReadingCrying;
                break;
        }
    }

    private void ChooseText ()
    {
        switch (page)
        {
            case Page.INTRO:
                titleText.text = "WELCOME TO SUPER CHEF!";
                rightText.text =
                    @"Dearest Contestant,

    Congratulations! We, the judges of SuperChef International, have reviewed your application and would like to welcome you as a contestant in this year’s competition. Find your contestant’s apron enclosed. Our competition rulebook will be shipped to you in 3-5 business days, at your expense. 
    As always, our competition involves three rounds of judging. For each round, you must prepare a dish to be reviewed by our committee. Only the best contestants will be selected to proceed. 
    Round one will consist of an appetizer, round two of an entree, and round three of a dessert. The contestant that passes all three rounds will be crowned this year’s Super Chef, and will have their unique recipes featured on our prime-time TV program (terms and conditions apply). Good luck!";
                contText.text = "(Click to Continue)";
                break;
            case Page.RULES:
                titleText.text = "RULES OF SUPER CHEF";
                contText.text = "(Click to Return)";
                break;
            case Page.DIED_LAVA:
                titleText.text = "YOU FELL IN LAVA!";
                rightText.text =
                    @"You quickly scamper to safety. The lava is quite as hot as you imagined and you are able to survive with only minor burns. Happy to still be alive, you decide to put your dreams of being the next SuperChef on hold for a little while.

A few weeks later you see the new SuperChef crowned and you are so full of envy you immediately decide to send in your application for the next year of the contest.";
                contText.text = "(Click to Play Next Year)";
                break;
            case Page.DIED_ERUPT:
                titleText.text = "THE VOLCANO ERUPTED!";
                rightText.text =
                    @"You got a little too greedy while gathering ingredients and the volcano erupted. It was only a very minor eruption, but you are quite cooked. In your last moments you wonder if the judges will account for that when they score you.

Oh well, at least you were doing what you loved. Maybe your twin brother can apply to the competition next year and win in your honor…";
                contText.text = "(Click to Play Next Year)";
                break;
            case Page.DISH_FAIL:
                titleText.text = "YOU WERE ELIMINATED";
                rightText.text = @"Dearest Contestant,

    We regret to inform you that your dish did not meet our culinary standards here at SuperChef International. In fact, one of our judges had to be rushed to the hospital and is currently recovering. In light of this, you will unfortunately NOT be moving on to the next round of our competition. 
    Good luck with your future endeavors.

Yours fearfully,
     SuperChef International Committee";
                contText.text = "(Click to Return Home)";
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
