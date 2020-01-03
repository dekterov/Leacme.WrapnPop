using Godot;
using System;

public class BubblePopped : Spatial {

	private PackedScene fB = GD.Load<PackedScene>("res://scenes/BubbleFull.tscn");

	public override void _Ready() {
		var expireTimer = new Timer() { Autostart = true, OneShot = true, WaitTime = (float)GD.RandRange(3, 8) };
		AddChild(expireTimer);
		expireTimer.Connect("timeout", this, nameof(OnTimerExpire));
	}

	private void OnTimerExpire() {
		var bubp = (Spatial)fB.Instance();
		bubp.Scale *= 0.95f;
		bubp.GetNode<MeshInstance>("BubbleFullPhysics/BubbleFullCollision/BubbleFull").MaterialOverride = new SpatialMaterial {
			Roughness = 0.2f,
			FlagsTransparent = true,
			AlbedoColor = Color.FromHsv(0, 0, 1, 0.5f)
		};
		bubp.GlobalTransform = GlobalTransform;
		GetParent().AddChild(bubp);
		QueueFree();
	}

}
