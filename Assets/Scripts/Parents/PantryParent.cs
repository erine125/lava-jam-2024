using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PantryParent : Parent
{

    // Parameters \\

    public Sprite[] backgroundFrames;
    public float OscillatePerSecond;
    public static float GridSquareSize = 0.4444f;
    public Vector2 GridAnchor = new Vector2(-8.889f, -4.8884f); // player centered of bottom left tile
    public TextAsset tilemapCsv;
    public float timeAllowed = 31f;


    // State \\

    private float timeSinceOsc;
    private int oscillateFrame;

    private Player player;

    [HideInInspector] public Tile[][] tiles;
    private List<Vector2> groundTiles;

    private float timeRemaining;


    // Storage \\

    private TextMeshProUGUI timerText;

    private List<Vector2> collectibleLocations;
    private List<GameObject> collectibleObjects; // these two must be matching order
    private List<Ingredient> collectibleIngredients;


    // Triggers \\

    public override void Awake()
    {
        base.Awake();

        collectibleLocations = new List<Vector2>();
        collectibleObjects = new List<GameObject>();
        collectibleIngredients = new List<Ingredient>();

        LoadTiles();

        player = GameObject.Find("PlayerChar").GetComponent<Player>();
        timerText = GameObject.Find("PanTimerText").GetComponent<TextMeshProUGUI>();
    }

    public override void Begin()
    {
        base.Begin();

        manager.heldIngredients.Clear();
        DistributeCollectibles();
        itemFrame.SetActive(true);
        timeRemaining = timeAllowed;

        for (int i = 0; i < itemSprites.Length; i++)
        {
            itemSprites[i].sprite = null;
        }

        player.Init();
    }

    public override void Update ()
    {
        base.Update();

        HandleOscillating();

        if (!transition.IsTransitioning ())
        {
            HandlePickup();
            UpdateTimer();
        }
    }

    public override void DoneFadingOut()
    {
        base.DoneFadingOut();

        RemoveAllCollectibles();
    }


    // Exposed \\

    public void PlayerReachedTarget()
    {
        if (CheckClosestTile (player.pos.x, player.pos.y) == Tile.LAVA)
        {
            player.state = Player.State.DYING;
               
            manager.narrateParent.page = NarrateParent.Page.DIED_LAVA;
            transition.StartLoadingOut(Type.NARRATE, 0.5f);
        }
    }

    public void PlayerLeave ()
    {
        player.state = Player.State.LEAVING;
        manager.kitchenParent.activity = KitchenParent.Activity.COOKING;
        transition.StartLoadingOut(Type.KITCHEN, 0);
    }


    // Utility \\

    private void HandleOscillating ()
    {
        timeSinceOsc += Time.deltaTime;

        if (timeSinceOsc >= 1f / OscillatePerSecond)
        {
            oscillateFrame = (oscillateFrame + 1) % 12;
            GetBackground().sprite = backgroundFrames[oscillateFrame];
            timeSinceOsc = 0;
        }
    }

    private void LoadTiles ()
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
                tiles[j][23 - i] = entries[j] == "L" ? Tile.LAVA : Tile.GROUND;
                if (tiles[j][23 - i] == Tile.GROUND)
                {
                    groundTiles.Add(new Vector2(j, 23 - i));
                }
            }
        }
    }

    private Tile CheckTileAtUnitsPos (float x, float y)
    {
        int xidx = (int)Mathf.Round((x - GridAnchor.x) * GridSquareSize);
        int yidx = (int)Mathf.Round((y - GridAnchor.y) * GridSquareSize);

        return tiles[xidx][yidx];
    }
    private Tile CheckClosestTile (float x, float y)
    {
        int xidx = (int)Mathf.Round(x);
        int yidx = (int)Mathf.Round(y);

        return tiles[xidx][yidx];
    }

    private void UpdateTimer ()
    {
        // cosmetic
        string stringForm = Mathf.FloorToInt(timeRemaining).ToString();
        stringForm = (stringForm.Length < 2) ? "0:0" + stringForm : "0:" + stringForm;
        timerText.text = stringForm;

        // kill player
        if (timeRemaining < 0)
        {
            player.state = Player.State.DYING;
            player.target = player.pos;
            player.source = player.pos;

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
            GameObject go = new GameObject("KitGroundIngredient" + nameNum);
            go.transform.parent = gameObject.transform;
            go.AddComponent<SpriteRenderer>();
            go.GetComponent<SpriteRenderer>().sprite = picked.sprite;
            go.transform.localScale = new Vector3(10, 10);
            go.transform.position = new Vector3(GridSquareSize * pos.x, GridSquareSize * pos.y, 0);
            collectibleObjects.Add(go);
        }     
    }

    private void RemoveAllCollectibles()
    {
        foreach (GameObject go in collectibleObjects)
        {
            GameObject.Destroy(go);
        }
        collectibleObjects.Clear();
        collectibleIngredients.Clear();
        collectibleLocations.Clear();
    }

    private void HandlePickup ()
    {
        if (Input.GetKeyDown(KeyCode.C) && manager.heldIngredients.Count < 4)
        {
            // find closest valid ingredient index
            int index = -1;
            float dst = player.pickupDistance;
            for (int i = 0; i < collectibleLocations.Count; i++)
            {
                Vector2 pos = collectibleLocations[i];

                float d = Mathf.Sqrt(Mathf.Pow(pos.x - player.pos.x, 2) + Mathf.Pow(pos.y - player.pos.y, 2));
                if (d <= dst)
                {
                    index = i;
                }
            }

            // pickup item
            if (index != -1)
            {
                manager.heldIngredients.Add(collectibleIngredients[index]);
                itemSprites[manager.heldIngredients.Count - 1].sprite = collectibleIngredients[index].sprite;

                // remove it from the view
                Destroy(collectibleObjects[index]);
                collectibleLocations.RemoveAt(index);
                collectibleObjects.RemoveAt(index);
                collectibleIngredients.RemoveAt(index);
            }
        }
    }


    // Structure \\

    public enum Tile
    {
        LAVA,
        GROUND
    }

    
}
