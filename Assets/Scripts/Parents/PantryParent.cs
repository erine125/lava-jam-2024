using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PantryParent : Parent
{

    // Parameters \\

    public Sprite[] backgroundFrames;
    public Sprite[] foregroundFrames;
    public float oscillatePerSec = 2f;
    public static float GridSquareSize = 0.4444f;
    public Vector2 gridOffset;
    public TextAsset tilemapCsv;
    public float timeAllowed = 31f;

    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip fallingLavaSound;

    public Sprite shadowSprite;
    public Vector2 shadowOffset = Vector2.zero;

    public AudioSource musicSource;
    public AudioClip dungeonMusic;


    // State \\

    private float timeSinceOsc;
    private int oscillateFrame;

    private Player player;

    [HideInInspector] public Tile[][] tiles;
    private List<Vector2> groundTiles;

    private float timeRemaining;

    private float playerShrinkingTimer;


    // Storage \\

    private TextMeshProUGUI timerText;
    private SpriteRenderer foregroundRenderer;

    private List<Vector2> collectibleLocations; // these three must be matching order
    private List<GameObject> collectibleObjects, collectibleShadows;
    private List<Ingredient> collectibleIngredients;


    // Triggers \\

    public override void Awake()
    {
        base.Awake();

        collectibleLocations = new List<Vector2>();
        collectibleObjects = new List<GameObject>();
        collectibleShadows = new List<GameObject>();
        collectibleIngredients = new List<Ingredient>();

        LoadTiles();

        player = GameObject.Find("PlayerChar").GetComponent<Player>();
        timerText = GameObject.Find("PanTimerText").GetComponent<TextMeshProUGUI>();
        foregroundRenderer = GameObject.Find("PanForeSprite").GetComponent<SpriteRenderer>();
    }

    public override void Begin()
    {
        base.Begin();

        musicSource.clip = dungeonMusic;
        musicSource.volume = 0.3f;
        musicSource.Play();

        manager.RemoveAllHeldIngredients();
        RemoveAllCollectibles();
        DistributeCollectibles();
        itemFrame.SetActive(true);
        timeRemaining = timeAllowed;
        playerShrinkingTimer = 0;

        for (int i = 0; i < itemSprites.Length; i++)
        {
            itemSprites[i].sprite = null;
        }

        player.Init();
    }

    public override void Update()
    {
        base.Update();

        if (manager.activeParentType == Type.PANTRY)
        {
            HandleOscillating();

            if (!transition.IsTransitioning())
            {
                HandlePickup();
                ListenForKeyboardDropping();
                UpdateTimer();
            }
            else if (player.state == Player.State.DYING)
            {
                HandlePlayerShrinking();
            }
        }
    }

    public override void InputButton(string message)
    {
        if (!transition.IsTransitioning())
        {
            if (message.Substring(0, 7) == "PanItem")
            {
                HandleDropping(System.Int32.Parse(message.Substring(7, 1)) - 1);
            }
        }
    }


    // Exposed \\

    public void PlayerVulnerable()
    {
        if (CheckClosestTile(player.pos.x, player.pos.y) == Tile.LAVA)
        {
            player.coyoteTime -= Time.deltaTime;

            if (player.coyoteTime <= 0)
            {
                // play falling in lava sound
                audioSource.PlayOneShot(fallingLavaSound, 0.5f);

                player.state = Player.State.DYING;

                manager.narrateParent.page = NarrateParent.Page.DIED_LAVA;
                transition.StartLoadingOut(Type.NARRATE, 0.5f);
            }
        }
        else
        {
            player.coyoteTimeLeft = player.coyoteTime;
        }
    }

    public void PlayerLeave()
    {
        musicSource.Stop();
        player.state = Player.State.LEAVING;
        manager.kitchenParent.activity = KitchenParent.Activity.COOKING;
        transition.StartLoadingOut(Type.KITCHEN, 0);
    }


    // Utility \\

    private void HandleOscillating()
    {
        timeSinceOsc += Time.deltaTime;

        if (timeSinceOsc >= 1f / oscillatePerSec)
        {
            oscillateFrame++;
            GetBackground().sprite = backgroundFrames[oscillateFrame % 12];
            foregroundRenderer.sprite = foregroundFrames[oscillateFrame % 4];
            timeSinceOsc = 0;
        }
    }

    private void LoadTiles()
    {
        groundTiles = new List<Vector2>();

        tiles = new Tile[41][];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile[24];
        }

        string[] lines = tilemapCsv.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] entries = lines[i].Split(",");

            for (int j = 0; j < entries.Length; j++)
            {
                tiles[j][23 - i] = entries[j].Trim() == "L" ? Tile.LAVA : Tile.GROUND;
                if (tiles[j][23 - i] == Tile.GROUND)
                {
                    groundTiles.Add(new Vector2(j, 23 - i));
                }
            }
        }
    }

    private Tile CheckClosestTile(float x, float y)
    {
        int xidx = (int)Mathf.Round(x + gridOffset.x);
        int yidx = (int)Mathf.Round(y + gridOffset.y);

        return tiles[xidx][yidx];
    }

    private void UpdateTimer()
    {
        // cosmetic
        string stringForm = Mathf.FloorToInt(Mathf.Max(timeRemaining, 0)).ToString();
        stringForm = (stringForm.Length < 2) ? "0:0" + stringForm : "0:" + stringForm;
        timerText.text = stringForm;

        // kill player
        if (timeRemaining < 0)
        {
            player.state = Player.State.DYING;

            manager.narrateParent.page = NarrateParent.Page.DIED_ERUPT;
            transition.StartLoadingOut(Type.NARRATE, 0.5f);
        }
        timeRemaining -= Time.deltaTime;
    }

    private void DistributeCollectibles()
    {
        RemoveAllCollectibles();

        // fill up the to distribute
        List<string> toDistribute = new List<string>();
        toDistribute.AddRange(manager.chosenDish.ingredients);
        string[] keys = manager.ingredients.Keys.ToArray<string>();
        for (int i = 0; i < 5; i++)
        {
            int random = Mathf.FloorToInt(Random.Range(0, 15.999f));
            toDistribute.Add(keys[random]);
        }

        List<Vector2> groundTilesLocal = new List<Vector2>();
        foreach (Vector2 gt in groundTiles)
        {
            if (gt.x > 3)
            {
                groundTilesLocal.Add(new Vector2(gt.x, gt.y));
            }
        }

        // place them
        int nameNum = 0;
        foreach (string name in toDistribute)
        {
            nameNum++;
            // pick a spot
            int index = Mathf.FloorToInt(Random.Range(0, groundTilesLocal.Count - 0.001f));
            Vector2 pos = groundTilesLocal[index];
            groundTilesLocal.RemoveAt(index);
            collectibleLocations.Add(pos);

            // create
            Ingredient picked = manager.ingredients[name];
            collectibleIngredients.Add(picked);
            AddIngredientSprite("KitGroundIngredient" + nameNum, picked, pos);
        }
    }

    private void RemoveAllCollectibles()
    {
        foreach (GameObject go in collectibleObjects)
        {
            GameObject.Destroy(go);
        }
        foreach (GameObject go in collectibleShadows)
        {
            GameObject.Destroy(go);
        }
        collectibleObjects.Clear();
        collectibleShadows.Clear();
        collectibleIngredients.Clear();
        collectibleLocations.Clear();
    }

    private void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.C) && manager.CountHeldIngredients() < 4)
        {
            // find closest valid ingredient index
            int index = -1;
            float dst = player.pickupDistance;
            for (int i = 0; i < collectibleLocations.Count; i++)
            {
                Vector2 pos = collectibleLocations[i];

                float d = Mathf.Sqrt(Mathf.Pow((pos.x - 0.5f) - player.pos.x, 2) + Mathf.Pow(pos.y - player.pos.y, 2));
                if (d <= dst)
                {
                    index = i;
                }
            }

            // pickup item
            if (index != -1)
            {
                int idx = manager.AddToHeldIngredients(collectibleIngredients[index]);
                itemSprites[idx].sprite = collectibleIngredients[index].sprite;

                // remove it from the view
                Destroy(collectibleObjects[index]);
                Destroy(collectibleShadows[index]);
                collectibleLocations.RemoveAt(index);
                collectibleObjects.RemoveAt(index);
                collectibleShadows.RemoveAt(index);
                collectibleIngredients.RemoveAt(index);

                // play pickup audio
                audioSource.PlayOneShot(pickupSound, 0.5f);
            }
        }
    }

    private void ListenForKeyboardDropping ()
    {
        KeyCode[] codes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        for (int i = 0; i < codes.Length; i++)
        {
            if (Input.GetKeyDown(codes[i]))
            {
                HandleDropping(i);
            }
        }
    }

    private void HandleDropping(int index)
    {
        if (manager.heldIngredients[index] != null)
        {
            // re-add it to the ground
            Ingredient ing = manager.heldIngredients[index];
            collectibleIngredients.Add(ing);
            Vector2 pos = new Vector2(player.pos.x, player.pos.y);
            collectibleLocations.Add(pos);
            AddIngredientSprite("KitGroundIngredientDropped", ing, pos);

            // remove it from being held
            manager.heldIngredients[index] = null;
            itemSprites[index].sprite = null;
        }
    }

    private void AddIngredientSprite(string gameObjectName, Ingredient ingredient, Vector2 position)
    {
        GameObject go = new GameObject(gameObjectName);
        go.transform.parent = gameObject.transform;
        go.AddComponent<SpriteRenderer>();
        go.GetComponent<SpriteRenderer>().sprite = ingredient.sprite;
        go.GetComponent<SpriteRenderer>().sortingOrder = 1;
        go.transform.localScale = new Vector3(10, 10);
        go.transform.position = new Vector3(GridSquareSize * position.x, GridSquareSize * position.y, 0);
        collectibleObjects.Add(go);

        GameObject gos = new GameObject(gameObjectName + "Shadow");
        gos.transform.parent = gameObject.transform;
        gos.AddComponent<SpriteRenderer>();
        gos.GetComponent<SpriteRenderer>().sprite = shadowSprite;
        gos.transform.localScale = new Vector3(10, 10);
        gos.transform.position = new Vector3(GridSquareSize * position.x + shadowOffset.x,
            GridSquareSize * position.y + shadowOffset.y, 0);
        collectibleShadows.Add(gos);
    }

    private void HandlePlayerShrinking()
    {
        playerShrinkingTimer += Time.deltaTime;
        float amt = Mathf.Max(1f - playerShrinkingTimer / 1f, 0) * 10f;
        player.gameObject.transform.localScale = new Vector3(amt, amt, 1);
    }


    // Structure \\

    public enum Tile
    {
        LAVA,
        GROUND
    }


}