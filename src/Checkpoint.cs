using Godot;

public partial class Checkpoint : Area2D
{
    private void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            player.Checkpoint = Position;
        }
    }
}
