using UnityEngine;

public class Player : MonoBehaviour
{
    // Parameters \\

    public Sprite[] walkFrontSprites, walkSideSprites, walkBackSprites;

    public float walkSpeed;
    public float timePerFrame = 0.2f;
    public float dashMultiplier = 4f;
    public float dashLength = 3f;
    public float pickupDistance = 1.5f;


    // State \\

    [Header("State")]
    public Facing facing;
    public Vector2 pos; // in tiles
    public State state;
    public Vector2 target;
    public Vector2 source;


    // Storage \\

    private SpriteRenderer spriteRenderer;
    private PantryParent parent;

    private float walkTimer;
    private int walkFrameIndex;


    // Exposed \\

    void Awake ()
    {
        facing = Facing.DOWN;
        spriteRenderer = GetComponent<SpriteRenderer>();
        parent = GameObject.Find("PantryOnly").GetComponent<PantryParent>();
    }

    public void Init ()
    {
        pos = new Vector2(1, 8);
        source.Set(pos.x, pos.y);
        target.Set(pos.x, pos.y);
        state = State.STAND;
    }

    void Update()
    {
        if (state == State.STAND || state == State.WALK)
        {
            PollArrowInput();
            PollDashInput();
        }

        if (IsMoving() && state != State.DYING && state != State.LEAVING)
        {
            MoveTowardsTarget();
        }

        CheckBoundsAndLeave();

        transform.position = new Vector3(PantryParent.GridSquareSize * pos.x, PantryParent.GridSquareSize * pos.y, 0);
        UpdateSprite();
    }


    // Utility \\

    private void MoveTowardsTarget ()
    {
        float travel = Time.deltaTime * walkSpeed;
        travel *= state == State.DASHING ? dashMultiplier : 1;
        float dist = Mathf.Sqrt (Mathf.Pow(target.x - pos.x, 2) + Mathf.Pow(target.y - pos.y, 2));

        // reached target
        if (travel >= dist)
        {
            pos.Set(target.x, target.y);
            source.Set(target.x, target.y);
            state = State.STAND;
            parent.PlayerReachedTarget();
        }
        else
        {
            switch (facing)
            {
                case Facing.UP:
                    pos.y += travel;
                    break;
                case Facing.DOWN:
                    pos.y -= travel;
                    break;
                case Facing.LEFT:
                    pos.x -= travel;
                    break;
                case Facing.RIGHT:
                    pos.x += travel;
                    break;
            }
        }
    }

    private void PollArrowInput ()
    {
        bool up = Input.GetKey(KeyCode.UpArrow);
        bool down = Input.GetKey(KeyCode.DownArrow);
        bool left = Input.GetKey(KeyCode.LeftArrow);
        bool right = Input.GetKey(KeyCode.RightArrow);

        // reversing
        if (IsMoving ())
        {
            if (facing == Facing.UP && down && !up)
            {
                target.Set(source.x, source.y);
                facing = Facing.DOWN;
            }
            else if (facing == Facing.DOWN && up && !down)
            {
                target.Set(source.x, source.y);
                facing = Facing.UP;
            }
            else if (facing == Facing.LEFT && right && !left)
            {
                target.Set(source.x, source.y);
                facing = Facing.RIGHT;
            }
            else if (facing == Facing.RIGHT && left && !right)
            {
                target.Set(source.x, source.y);
                facing = Facing.LEFT;
            }
        }
        // corner turning
        else
        {
            if (up && !down && !left && !right)
            {
                facing = Facing.UP;
                target.Set(source.x, source.y + 1);
                state = State.WALK;
            }
            else if (!up && down && !left && !right)
            {
                facing = Facing.DOWN;
                target.Set(source.x, source.y - 1);
                state = State.WALK;
            }
            else if (!up && !down && left && !right)
            {
                facing = Facing.LEFT;
                target.Set(source.x - 1, source.y);
                state = State.WALK;
            }
            else if (!up && !down && !left && right)
            {
                facing = Facing.RIGHT;
                target.Set(source.x + 1, source.y);
                state = State.WALK;
            }
        }
    }

    private void PollDashInput ()
    {
        if (Input.GetKey(KeyCode.X))
        {
            switch (facing)
            {
                case Facing.UP:
                    target.Set(source.x, source.y + dashLength);
                    break;
                case Facing.DOWN:
                    target.Set(source.x, source.y - dashLength);
                    break;
                case Facing.LEFT:
                    target.Set(source.x - dashLength, source.y);
                    break;
                case Facing.RIGHT:
                    target.Set(source.x + dashLength, source.y);
                    break;
            }
            state = State.DASHING;
        }
    }

    public bool IsMoving ()
    {
        return target.x != pos.x || target.y != pos.y;
    }

    private void UpdateSprite ()
    {
        Sprite[] sprites = walkFrontSprites;
        bool invert = false;
        switch (facing)
        {
            case Facing.UP:
                sprites = walkBackSprites;
                break;
            case Facing.RIGHT:
                invert = true;
                sprites = walkSideSprites;
                break;
            case Facing.LEFT:
                sprites = walkSideSprites;
                break;
        }

        walkTimer += Time.deltaTime;
        if (walkTimer > timePerFrame && IsMoving ())
        {
            walkTimer = 0f;
            walkFrameIndex = (walkFrameIndex + 1) % 4;
            spriteRenderer.sprite = sprites[walkFrameIndex];
            spriteRenderer.flipX = invert;
        }
        else if (!IsMoving ())
        {
            spriteRenderer.sprite = sprites[0];
        }
    }

    private void CheckBoundsAndLeave ()
    {
        pos.x = Mathf.Clamp(pos.x, 0, 41);
        pos.y = Mathf.Clamp(pos.y, 0, 24);

        if (pos.x == 41 || pos.y == 0 || pos.y == 24)
        {
            target.Set(pos.x, pos.y);
            source.Set(pos.x, pos.y);
            state = State.STAND;
            parent.PlayerReachedTarget();
        }
        else if (pos.x == 0 && facing == Facing.LEFT)
        {
            target.Set(pos.x, pos.y);
            source.Set(pos.x, pos.y);
            state = State.STAND;
            parent.PlayerLeave();
        }
    }


    // Structure \\

    public enum Facing
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public enum State
    {
        STAND,
        WALK,
        DYING,
        DASHING,
        LEAVING
    }


}
