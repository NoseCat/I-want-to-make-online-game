using Godot;
using System;

public partial class GunRoot : Node2D
{
	public AnimationPlayer Anima;
	public Sprite2D Sprite;
	public Marker2D Marker;
	public Marker2D Marker1;
	public Marker2D Marker2;
	public Vector2 Mouse; 
	public Node2D Parent; 
	[Export] PackedScene BulletScene;
	[Export] float BulletSpeed = 300;
	public override void _Ready()
	{
		Anima = GetNode<AnimationPlayer>("AnimationPlayer");
		Parent = GetNode<Node2D>("..");
		Sprite = GetNode<Sprite2D>("Gun");
		Marker1 = GetNode<Marker2D>("Marker2D");
		Marker2 = GetNode<Marker2D>("Marker2D2");
		//GetNode<Marker2D>("Marker2D3").Position = new Vector2(0, -GetNode<Marker2D>("Marker2D3").Position.X);
		Marker = Marker1;
	}

	private void _on_area_2d_mouse_entered()
	{
		Sprite.FlipH = true;
	}

	private void _on_area_2d_mouse_exited()
	{
		Sprite.FlipH = false;
	}

	//private void AnimaFinish(string anim_name)
	//{
	//	if(anim_name != "RESET")
	//		Anima.Play("RESET");
	//}
	
	public override void _Process(double delta)
	{
		Mouse = GetGlobalMousePosition();
		LookAt(Mouse);
		if(Mouse.X < Parent.GlobalPosition.X && !Sprite.FlipV)
		{
			Sprite.FlipV = true;
			GetNode<Marker2D>("Marker2D3").Position *= new Vector2(1,-1);//= new Vector2(GetNode<Marker2D>("Marker2D3").Position.X, -GetNode<Marker2D>("Marker2D3").Position.Y);
			Marker = Marker2;
		}
		if(Mouse.X >= Parent.GlobalPosition.X && Sprite.FlipV)
		{
			Sprite.FlipV = false;
			GetNode<Marker2D>("Marker2D3").Position *= new Vector2(1,-1); //= new Vector2(GetNode<Marker2D>("Marker2D3").Position.X, -GetNode<Marker2D>("Marker2D3").Position.Y);
			Marker = Marker1;
		}
		if(Input.IsActionJustPressed("click"))
		{
			Anima.Play("RESET");
			Anima.Play(Sprite.FlipV ? "Shot2" : "Shot");
			RigidBody2D bullet = BulletScene.Instantiate<RigidBody2D>();
			bullet.GlobalPosition = Marker.GlobalPosition;
			((boolet)bullet).BackMarker = GetNode<Marker2D>("Marker2D3").GlobalPosition;
			//((boolet)bullet).SpriteRotation = GlobalRotation;
			((boolet)bullet).motion = Mouse - bullet.Position; //this took ages...
			((boolet)bullet).speed = BulletSpeed;
			GetTree().Root.AddChild(bullet);
		}
	}
}
