using Godot;
using System;

public partial class CharacterBody : CharacterBody2D
{
	public string name = "NoseCat"; 

	public const float Speed = 100.0f;
	public const float SpeedLimit = 750.0f;
	public const float JumpVelocity = -1000.0f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public bool JumpPressed = false;
	public bool Coyote = false;
	public bool CoyoteFix = true;
	public bool FirstJump = false;
	public bool SecondJump = false;

	private Vector2 SyncPos = new Vector2(0,0);

    public override void _Ready()
    {
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(GetNode<Node2D>("..").Name));
	}

    private void _on_jump_buffer_timeout()
	{
		JumpPressed = false;
	}
	private void _on_coyote_jump_timeout()
	{	
		Coyote = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			GetNode<Camera2D>("Camera2D").Enabled = true;

			GetNode<GunRoot>("GunRoot").IsOwned = true;
			Vector2 velocity = Velocity;
			
		//flip sprite
			if(GetGlobalMousePosition().X < GlobalPosition.X && GetNode<Sprite2D>("NoseCat").FlipH)
				GetNode<Sprite2D>("NoseCat").FlipH = false;
			if(GetGlobalMousePosition().X >= GlobalPosition.X && !GetNode<Sprite2D>("NoseCat").FlipH)
				GetNode<Sprite2D>("NoseCat").FlipH = true;

		//Gravity
			if (!IsOnFloor())
			{
				float JumpControl = gravity;
				if (Input.IsActionPressed("Jump"))
				{
					JumpControl = gravity/1.5f;	
				}
				if (velocity.Y > -250)
				{
					velocity.Y += 2 * gravity * (float)delta;
				}
				else
				{
					velocity.Y += JumpControl * (float)delta;
				}
				
			}

			if (Input.IsActionJustPressed("Jump") && !IsOnFloor() && FirstJump && !SecondJump)
			{
				SecondJump = true;
				velocity.Y = JumpVelocity;
			}

		//Jump
			if (IsOnFloor())
			{
				CoyoteFix = true;
				FirstJump = false;
				SecondJump = false;
			}
			if (Input.IsActionJustPressed("Jump"))
			{
				GetNode<Timer>("JumpBuffer").Start();
				JumpPressed = true;
			}
			if(!IsOnFloor() && CoyoteFix)
			{
				CoyoteFix = false;
				Coyote = true;
				GetNode<Timer>("CoyoteJump").Start();
			}
			if (JumpPressed && (IsOnFloor() || Coyote))
			{
				FirstJump = true;
				Coyote = false;
				CoyoteFix = false;
				JumpPressed = false;
				velocity.Y = JumpVelocity;
			}

		//Gorizontal movement
			float direction = Input.GetActionStrength("Right") - Input.GetActionStrength("Left");
			velocity.X += direction * Speed;
			if(Math.Abs(velocity.X) >= SpeedLimit)
				velocity.X = SpeedLimit * Math.Sign(velocity.X);
			float friction =  Speed /4 ; //as speed is const this just a number
			if (velocity.X != 0 && direction == 0)
				velocity.X -= Math.Sign(velocity.X) * friction;

		//other
			//GetNode<Label>("Label").Text ="FirstJump " + FirstJump + "\nSecondJump " + SecondJump + "\nIsOnFloor() " + IsOnFloor();//"Coyote: " + Coyote + "\nCoyoteFix: " + CoyoteFix;
			Velocity = velocity;
			MoveAndSlide();
		//Gun
			

		//online sync
			SyncPos = GlobalPosition;
		}else{
			//GlobalPosition = GlobalPosition.Lerp(SyncPos, .5f);
			GlobalPosition = SyncPos;
		}
	}	
}
