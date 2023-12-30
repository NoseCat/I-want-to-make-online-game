using Godot;
using System;

public partial class GunRoot : Node2D
{
	public bool IsOwned = false;

	public bool syncFlipV = false;
	public Vector2 syncLabelPosition = new Vector2(1,-18);
	public float syncLabelRotation = 0;
	//public Vector2 syncMarkerPosition = new Vector2(0,0);
	//public Vector2 syncBackMarkerPosition = new Vector2(0,0);

	public Marker2D BackMarker;
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
	[Export] uint max_ammo = 9;
	public uint ammo = 9;
	public override void _Ready()
	{
		//GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(GetNode<Node2D>("..").Name));
		BackMarker = GetNode<Marker2D>("Marker2D3");

		label = GetNode<Label>("Gun/Label");
		Anima = GetNode<AnimationPlayer>("AnimationPlayer");
		Parent = GetNode<Node2D>("..");
		Sprite = GetNode<Sprite2D>("Gun");
		Marker1 = GetNode<Marker2D>("Marker2D");
		Marker2 = GetNode<Marker2D>("Marker2D2");
		//GetNode<Marker2D>("Marker2D3").Position = new Vector2(0, -GetNode<Marker2D>("Marker2D3").Position.X);
		Marker = Marker1;
	}

	private void AnimaFinish(string anim_name)
	{
		if(anim_name == "Reload" || anim_name == "Reload2")
		{
			reloading = false;
			label.Text = ammo + " / " + max_ammo;
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void Reload()
	{
		reloading = true;
		Anima.Play("RESET");
		Anima.Play(Sprite.FlipV ? "Reload" : "Reload2");
		ammo = max_ammo;
		
		//Anima.Play("Reload");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SpawnBullet()
	{
		ammo--;
		label.Text = ammo + " / " + max_ammo;
		Anima.Play("RESET");
		Anima.Play(Sprite.FlipV ? "Shot2" : "Shot");
		RigidBody2D bullet = BulletScene.Instantiate<RigidBody2D>();
		bullet.GlobalPosition = Marker.GlobalPosition;
		((boolet)bullet).BackMarker = BackMarker.GlobalPosition;
		//((boolet)bullet).SpriteRotation = GlobalRotation;
		//((boolet)bullet).motion = Mouse - bullet.Position; //this took ages...
		((boolet)bullet).motion = Marker.GlobalPosition - BackMarker.GlobalPosition;
		((boolet)bullet).speed = BulletSpeed;
		//GetTree().Root.AddChild(bullet);
		GetTree().Root.AddChild(bullet);
	}
	
	public override void _Process(double delta)
	{
		//gun rotation
		Mouse = GetGlobalMousePosition();
		if (Mouse.X < Parent.GlobalPosition.X && !Sprite.FlipV)
		{
			label.Rotation += (float)Math.PI;
			label.Position *= new Vector2(1, -1);
			label.Position += new Vector2(47 / 2, 0);
			Sprite.FlipV = true;
		}
		if (Mouse.X >= Parent.GlobalPosition.X && Sprite.FlipV)
		{
			label.Rotation -= (float)Math.PI;
			label.Position *= new Vector2(1, -1);
			label.Position -= new Vector2(47 / 2, 0);
			Sprite.FlipV = false;
	
		}
		if(GetNode<Sprite2D>("../NoseCat").FlipH) //&& Marker != Marker2)
		{
			BackMarker = GetNode<Marker2D>("Marker2D3");//= new Vector2(GetNode<Marker2D>("Marker2D3").Position.X, -GetNode<Marker2D>("Marker2D3").Position.Y);
			Marker = Marker1;
		}
		if(!GetNode<Sprite2D>("../NoseCat").FlipH) //&& Marker != Marker1)
		{
			BackMarker = GetNode<Marker2D>("Marker2D4"); //= new Vector2(GetNode<Marker2D>("Marker2D3").Position.X, -GetNode<Marker2D>("Marker2D3").Position.Y);
			Marker = Marker2;
		}
		//GD.Print(BackMarker.Position);
		//GD.Print(Marker.Position);
		if(IsOwned)
		{
			if ((ammo <= 0 && Input.IsActionPressed("click") || Input.IsActionJustPressed("Reload")) && !reloading)
			{
				Rpc("Reload");
			}
			//shot	
			if (ShotProgress == ShotSpeed)
			{
				ShotProgress = 0;
			}

			if (ShotProgress == 0 && ammo > 0 && !reloading && Input.IsActionPressed("click"))
			{
				Rpc("SpawnBullet");
			}
			if (Input.IsActionPressed("click"))
				ShotProgress++;
			else
				ShotProgress = 0;

			//syncBackMarkerPosition = BackMarker.Position;
			syncLabelRotation = label.Rotation;
			syncLabelPosition = label.Position;
			syncFlipV = Sprite.FlipV;
			//syncMarkerPosition = Marker.Position;
			LookAt(Mouse);
		}
		else
		{
			//BackMarker.Position = syncBackMarkerPosition;
			label.Rotation = syncLabelRotation;
			label.Position = syncLabelPosition;
			Sprite.FlipV = syncFlipV;
			//Marker.Position = syncMarkerPosition;
		}
	}
}
