using Godot;

public partial class Coin : Area2D
{

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            // Increment the coin count
            player.Coins++;
            QueueFree();
        }
    }

}
