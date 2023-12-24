using Godot;
using System;

public partial class GunRoot : Node2D
{
	public Sprite2D Sprite;
	[Export] PackedScene BulletScene;
	[Export] float BulletSpeed = 300;
	[Export] uint BulletRange = 200;
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Gun");
	}

	public override void _Process(double delta)
	{
		LookAt(GetGlobalMousePosition());
		if(Input.IsActionJustPressed("click"))
		{
			RigidBody2D bullet = BulletScene.Instantiate<RigidBody2D>();
			bullet.Rotation = GlobalRotation;
			bullet.GlobalPosition = GlobalPosition;
			((boolet)bullet).motion = GetGlobalMousePosition() - bullet.Position; //this took ages...
			((boolet)bullet).speed = BulletSpeed;
			((boolet)bullet).range_limit = BulletRange;
			GetTree().Root.AddChild(bullet);
		}
	}
}
