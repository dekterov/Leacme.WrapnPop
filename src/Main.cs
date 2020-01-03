// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();

	private PackedScene fB = GD.Load<PackedScene>("res://scenes/BubbleFull.tscn");
	private PackedScene pB = GD.Load<PackedScene>("res://scenes/BubblePopped.tscn");

	private List<AudioStream> pops = new List<AudioStream> {
		GD.Load<AudioStream>("res://assets/pop1.ogg"),
		GD.Load<AudioStream>("res://assets/pop2.ogg"),
		GD.Load<AudioStream>("res://assets/pop3.ogg"),
		GD.Load<AudioStream>("res://assets/pop4.ogg"),
		GD.Load<AudioStream>("res://assets/pop5.ogg") };

	private Spatial GetRaycastedObjectUnderMouse(float rayLengh = 100) {
		var mPos = GetViewport().GetMousePosition();
		var origin = GetNode<Camera>("cam").ProjectRayOrigin(mPos);
		var dest = origin + GetNode<Camera>("cam").ProjectRayNormal(mPos) * rayLengh;
		var hitObj = GetWorld().DirectSpaceState.IntersectRay(origin, dest);
		if (hitObj.Count > 0) {
			return (Spatial)hitObj["collider"];
		} else return null;
	}

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		GetNode<WorldEnvironment>("sky").Environment.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		InitSound();
		AddChild(Audio);

		for (int i = -5; i < 15; i++) {
			for (int j = 0; j < 10; j++) {
				var bub = (Spatial)fB.Instance();
				bub.Scale *= 0.95f;
				bub.GetNode<MeshInstance>("BubbleFullPhysics/BubbleFullCollision/BubbleFull").MaterialOverride = new SpatialMaterial {
					Roughness = 0.5f,
					FlagsTransparent = true,
					AlbedoColor = Color.FromHsv(0, 0, 1, 0.8f)
				};
				if ((i % 2) == 0) {
					bub.Translation = new Vector3(i, 0, j);
				} else {
					bub.Translation = new Vector3(i, 0, j + 0.5f);
				}

				AddChild(bub);
			}
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventScreenTouch te && te.Pressed) {

			var rayedObj = GetRaycastedObjectUnderMouse();
			if (rayedObj != null) {
				var bubp = (Spatial)pB.Instance();
				bubp.GetNode<MeshInstance>("BubblePopped").MaterialOverride = new SpatialMaterial {
					Roughness = 0.5f,
					FlagsTransparent = true,
					AlbedoColor = Color.FromHsv(0, 0, 1, 0.8f)
				};
				bubp.Scale = rayedObj.Scale;
				bubp.GlobalTransform = rayedObj.GlobalTransform;
				AddChild(bubp);
				pops[new Random().Next(pops.Count)].Play(Audio);
				rayedObj.QueueFree();
			}
		}
	}

}
