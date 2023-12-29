using Godot;
using System;

public partial class GunRoot : Node2D
{
	public bool reloading = false;
	public Label label;
	public AnimationPlayer Anima;
	public Sprite2D Sprite;
	public Marker2D Marker;
	public Marker2D Marker1;
	public Marker2D Marker2;
	public Vector2 Mouse; 
	public Node2D Parent; 
	[Export] uint ShotSpeed = 10;
	public uint ShotProgress = 0;
	public bool Shoot = false;
	[Export] PackedScene BulletScene;
	[Export] float BulletSpeed = 300;
	[Export] uint max_ammo = 16;
	public uint ammo = 16;
	public override void _Ready()
	{
		//GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(GetNode<Node2D>("..").Name));
		label = GetNode<Label>("Gun/Label");
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

	private void AnimaFinish(string anim_name)
	{
		if(anim_name == "Reload" || anim_name == "Reload2")
		{
			ammo = max_ammo;
			reloading = false;
			label.Text = ammo + " / " + max_ammo;
		}
	}
	
	public override void _Process(double delta)
	{
//		if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
//		{
			if ( (ammo <= 0 && Input.IsActionPressed("click") || Input.IsActionJustPressed("Reload")) && !reloading)
			{
				reloading = true;
				Anima.Play("RESET");
				Anima.Play(Sprite.FlipV ? "Reload" : "Reload2");
				//Anima.Play("Reload");
			}

			//gun rotation
			Mouse = GetGlobalMousePosition();
			LookAt(Mouse);
			if(Mouse.X < Parent.GlobalPosition.X && !Sprite.FlipV)
			{
				label.Rotation += (float)Math.PI;
				label.Position *= new Vector2(1,-1);
				label.Position += new Vector2(47/2,0);
				Sprite.FlipV = true;
				GetNode<Marker2D>("Marker2D3").Position *= new Vector2(1,-1);//= new Vector2(GetNode<Marker2D>("Marker2D3").Position.X, -GetNode<Marker2D>("Marker2D3").Position.Y);
				Marker = Marker2;
			}
			if(Mouse.X >= Parent.GlobalPosition.X && Sprite.FlipV)
			{
				label.Rotation -= (float)Math.PI;
				label.Position *= new Vector2(1,-1);
				label.Position -= new Vector2(47/2,0);
				Sprite.FlipV = false;
				GetNode<Marker2D>("Marker2D3").Position *= new Vector2(1,-1); //= new Vector2(GetNode<Marker2D>("Marker2D3").Position.X, -GetNode<Marker2D>("Marker2D3").Position.Y);
				Marker = Marker1;
			}

			//shot	
			if(ShotProgress == ShotSpeed)
			{
				ShotProgress = 0;
			}

			if(ShotProgress == 0 && ammo > 0 && !reloading && Input.IsActionPressed("click"))
			{
				ammo--;
				label.Text = ammo + " / " + max_ammo;
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
			if (Input.IsActionPressed("click"))
				ShotProgress++;
			else
				ShotProgress = 0;
//		}
	}
}
