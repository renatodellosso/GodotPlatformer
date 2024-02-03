using Godot;

public partial class HeartPickup : Area2D
{

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            GD.Print("Heart picked up!");
            player.Lives++;
            QueueFree();
        }
    }

}
