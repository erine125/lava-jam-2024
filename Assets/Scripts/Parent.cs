using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using static System.TimeZoneInfo;

public abstract class Parent : MonoBehaviour
{
    // Parameters \\

    [HideInInspector]
    public Manager manager;

    public Sprite backgroundSprite;


    // State \\

    protected TransitionMgr transition;


    // Triggers \\

    /// <summary>
    /// This base method should be called even if overridden.
    /// </summary>
    public virtual void Awake()
    {
        transition = new TransitionMgr(this);
    }


    // To Override \\

    /// <summary>
    /// Should only be called by the Manager.
    ///
    /// When overridden, this base method should also be called.
    /// </summary>
    public virtual void Begin()
    {
        GetBackground().sprite = backgroundSprite;
        GetTintOverlay().color = new Color(0, 0, 0, 0);

        transition.StartLoadingIn();
    }

    public virtual void InputButton (string message) { }

    public virtual void InputClick (Vector2 pos) { }

    /// <summary>
    /// This base method should be called even if overridden.
    /// </summary>
    public virtual void Update()
    {
        transition.DoTransition();
    }

    public virtual void DoneFadingIn () { }

    public virtual void DoneFadingOut () { }


    // Functionality \\

    protected SpriteRenderer GetBackground ()
    {
        return GameObject.Find("Background").GetComponent<SpriteRenderer>();
    }
    protected SpriteRenderer GetTintOverlay ()
    {
        return GameObject.Find("TintOverlay").GetComponent<SpriteRenderer>();
    }


    // Structure \\

    public enum Type
    {
        TITLE,
        KITCHEN,
        PANTRY,
        NARRATE,
        NONE
    }

    public class TransitionMgr
    {
        // Storage

        private Parent.Type transitionType;
        private bool loadingIn; // default should be false

        private float delayTimer;
        private float fadingTimer;

        private Parent parent;

        private List<TextMeshProUGUI> textMeshes; // mapped to true color


        // Exposed

        public TransitionMgr (Parent parent)
        {
            transitionType = Type.NONE;
            loadingIn = false;

            delayTimer = 0;
            fadingTimer = 0;

            textMeshes = new List<TextMeshProUGUI>();

            this.parent = parent;
        }

        public void StartLoadingIn ()
        {
            FindAllTextMeshes();

            if (parent.manager.fadeMultiplier == 0)
            {
                parent.DoneFadingIn();
                parent.manager.UpdateTintEffect(0);
                TintTextMeshes(0);
            }
            else
            {
                delayTimer = 0;
                fadingTimer = 0;
                transitionType = Type.NONE;
                loadingIn = true;
                TintTextMeshes(1);
            }
        }

        public void StartLoadingOut (Parent.Type type, float speedMult = 1f)
        {
            FindAllTextMeshes();

            if (speedMult == 0)
            {
                parent.manager.UpdateTintEffect(0);
                parent.manager.ChooseActiveParent(type);
            }
            else
            {
                delayTimer = 0;
                fadingTimer = 0;
                transitionType = type;
                loadingIn = false;
                parent.manager.fadeMultiplier = speedMult;
            }
        }

        public bool IsTransitioning ()
        {
            return transitionType != Type.NONE || loadingIn;
        }

        /// <summary>
        /// Should be called every frame
        /// </summary>
        public void DoTransition ()
        {
            // leaving this parent
            if (transitionType != Type.NONE)
            {
                fadingTimer += Time.deltaTime;
                float scl = Mathf.Min(fadingTimer * parent.manager.fadeOutMultiplier * parent.manager.fadeMultiplier, 1);
                parent.manager.UpdateTintEffect(scl);
                TintTextMeshes(scl);

                if (scl >= 1)
                {
                    parent.DoneFadingOut();
                    parent.manager.ChooseActiveParent(transitionType);
                    transitionType = Type.NONE;
                    fadingTimer = 0;
                }
            }
            // entering this parent
            else if (loadingIn)
            {
                if (delayTimer < parent.manager.pauseBlackOnFade)
                {
                    parent.manager.UpdateTintEffect(1);
                    delayTimer += Time.deltaTime;
                }
                else
                {
                    fadingTimer += Time.deltaTime;
                    float scl = Mathf.Max(1 - fadingTimer * parent.manager.fadeInMultiplier * parent.manager.fadeMultiplier, 0);
                    parent.manager.UpdateTintEffect(scl);
                    TintTextMeshes(scl);

                    if (scl <= 0)
                    {
                        parent.DoneFadingIn();
                        parent.manager.UpdateTintEffect(0);
                        delayTimer = 0;
                        fadingTimer = 0;
                        loadingIn = false;
                    }
                }
                
            }
        }


        // Utility

        public void FindAllTextMeshes ()
        {
            // find all button objects
            textMeshes.Clear();
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                if (go.activeInHierarchy)
                {
                    TextMeshProUGUI tm = go.GetComponent<TextMeshProUGUI>();
                    if (tm != null)
                    {
                        textMeshes.Add(tm);
                    }
                }
            }
        }

        public void TintTextMeshes (float amt)
        {
            foreach (TextMeshProUGUI tm in textMeshes)
            {
                tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, 1-amt);
            }
        }
    }

}
