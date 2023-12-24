using Godot;
using System;

public partial class boolet : RigidBody2D
{
	public Sprite2D Sprite;
	public float SpriteRotation = 0;
	[Export]
	public Vector2 motion = new Vector2(0,0);

	[Export]
	public float speed = 0;

	public uint bounce_limit = 10;
	public uint bounces = 0; 

	[Export]
	public uint range_limit = 0;
	public uint range = 0;

	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Rock");
		Sprite.Rotation = SpriteRotation;
	}

	void DeleteBullet()
	{
		//play effects;
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		range++;
		var collisionInfo = MoveAndCollide(motion.Normalized() * speed * (float)delta);
		if(collisionInfo != null)
		{
			motion = motion.Bounce(collisionInfo.GetNormal());
			Sprite.LookAt(motion);
			bounces++;
		}
		if(bounces >= bounce_limit)
			DeleteBullet();
		if(range >= range_limit)
			DeleteBullet();
	}
}