using Godot;
using System;

public partial class GunRoot : Node2D
{
	public Sprite2D Sprite;
	public Marker2D Marker;
	public Marker2D Marker1;
	public Marker2D Marker2;
	public Vector2 Mouse; 
	public Node2D Parent; 
	[Export] PackedScene BulletScene;
	[Export] float BulletSpeed = 300;
	[Export] uint BulletRange = 200;
	public override void _Ready()
	{
		Parent = GetNode<Node2D>("..");
		Sprite = GetNode<Sprite2D>("Gun");
		Marker1 = GetNode<Marker2D>("Marker2D");
		Marker2 = GetNode<Marker2D>("Marker2D2");
		Marker = Marker1;
	}

	public override void _Process(double delta)
	{
		Mouse = GetGlobalMousePosition();
		LookAt(Mouse);
		if(Mouse.X < Parent.GlobalPosition.X && !Sprite.FlipV)
		{
			Sprite.FlipV = true;
			Marker = Marker2;
		}
		if(Mouse.X >= Parent.GlobalPosition.X && Sprite.FlipV)
		{
			Sprite.FlipV = false;
			Marker = Marker1;
		}
		if(Input.IsActionJustPressed("click"))
		{
			RigidBody2D bullet = BulletScene.Instantiate<RigidBody2D>();
			bullet.GlobalPosition = Marker.GlobalPosition;
			((boolet)bullet).SpriteRotation = GlobalRotation;
			((boolet)bullet).motion = Mouse - bullet.Position; //this took ages...
			((boolet)bullet).speed = BulletSpeed;
			((boolet)bullet).range_limit = BulletRange;
			GetTree().Root.AddChild(bullet);
		}
	}
}
