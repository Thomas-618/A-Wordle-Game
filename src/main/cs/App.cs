using Godot;
using System;

public partial class App : MarginContainer
{
    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("ui_cancel")) 
		{
			GetTree().Root.PropagateNotification((int) NotificationWMCloseRequest);
			GetTree().Quit();
		}
    }
}
