using Godot;
using System;

public partial class boolet : RigidBody2D
{
	public Line2D Trace;
	public Sprite2D Sprite1;
	public Sprite2D Sprite2;
	//public float SpriteRotation = 0;
	public bool FirstBounce = true;
	public CollisionShape2D CollisionShape;
	public Timer LifeTime;

	[Export]
	public Vector2 motion = new Vector2(0,0);

	[Export]
	public float speed = 0;

	public uint bounce_limit = 10;
	public uint bounces = 0; 

	public override void _Ready()
	{
		Trace = GetNode<Line2D>("Line2D");
	//	CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
	//	Sprite1 = GetNode<Sprite2D>("Bullet");
	//	Sprite2 = GetNode<Sprite2D>("Bullet2");
	//	Sprite1.Rotation = SpriteRotation;
		LifeTime = GetNode<Timer>("LifeTime");
	}

	void DeleteBullet()
	{
		//play effects;
		QueueFree();
	}

	public void _on_life_time_timeout()
	{
		DeleteBullet();
	}

	public override void _PhysicsProcess(double delta)
	{
		var collisionInfo = MoveAndCollide(motion.Normalized() * speed * (float)delta);
		if(collisionInfo != null)
		{
			motion = motion.Bounce(collisionInfo.GetNormal());
			//Sprite.LookAt(motion);
			/*if(FirstBounce)
			{
				Trace.Visible = true;
				CollisionShape.Position = new Vector2(0, -0.5f);
				Sprite1.Visible = false;
				//Sprite2.Visible = true;
				FirstBounce = false;
			}*/
			bounces++;
		}
		if(bounces >= bounce_limit)
			DeleteBullet();
	}
}