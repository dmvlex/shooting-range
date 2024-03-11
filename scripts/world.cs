using Godot;
using System;

public partial class world : Node3D
{
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionPressed("Fullscreen"))
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else if (Input.IsActionPressed("Windowed"))
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
        else if (Input.IsActionPressed("Exit"))
        {
            GetTree().Quit();
        }
    }
}
