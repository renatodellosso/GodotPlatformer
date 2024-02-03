using Godot;

public partial class Player : CharacterBody2D
{
    private const float SPEED = 200.0f;
    private const float JUMP_VELOCITY = -400.0f;
    private const float CAMERA_SPEED = 3f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public Vector2 Checkpoint { get; set; }

    private Camera2D camera;

    private int lives;
    public int Lives
    {
        get => lives;
        set
        {
            GD.Print("Setting Lives to " + value);

            lives = value;
            if (lives <= 0)
                GetTree().ReloadCurrentScene();
            else
            {
                // Regenerate the heart icons in the HUD
                HBoxContainer hearts = GetNode<HBoxContainer>("Hud/Heart Row");
                while (hearts.GetChildCount() > lives)
                {
                    // We can't use QueueFree() because it takes a frame to free the node
                    hearts.GetChild(hearts.GetChildCount() - 1).Free();
                }

                // Add the heart icons in the HUD
                while (hearts.GetChildCount() < lives)
                {
                    TextureRect heart = HeartIcon.Instantiate<TextureRect>();
                    hearts.AddChild(heart);
                }
            }
        }
    }

    private int coins;
    public int Coins
    {
        get => coins;
        set
        {
            GD.Print("Setting Coins to " + value);
            coins = value;
            GetNode<Label>("Hud/CoinLabel").Text = $"x{coins}";
        }
    }

    [Export]
    public PackedScene HeartIcon { get; set; }

    public override void _Ready()
    {
        Checkpoint = Position;
        camera = GetNode<Camera2D>("Camera");

        Lives = 3;
        Coins = 0;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Add the gravity
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

        // Handle Jump
        if (Input.IsActionJustPressed("jump") && IsOnFloor())
            velocity.Y = JUMP_VELOCITY;

        // Get the input direction and handle the movement/deceleration
        // As good practice, you should replace UI actions with custom gameplay actions
        Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            GetNode<Sprite2D>("Sprite2D").FlipH = direction.X > 0;

            velocity.X = direction.X * SPEED;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, SPEED);
        }

        // Move camera
        Vector2 targetCamPos = Position + Velocity * (float)delta;
        camera.GlobalPosition = new()
        {
            X = Mathf.MoveToward(camera.GlobalPosition.X, targetCamPos.X, CAMERA_SPEED),
            Y = Mathf.MoveToward(camera.GlobalPosition.Y, targetCamPos.Y, CAMERA_SPEED)
        };

        Velocity = velocity;
        MoveAndSlide();

        // Handle collisions
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            if (collision.GetCollider() is StaticBody2D body && body.IsInGroup("killbox"))
            {
                Lives--;
                Position = Checkpoint;
                break;
            }
        }
    }
}
