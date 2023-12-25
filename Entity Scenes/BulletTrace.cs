using Godot;
using System;

public partial class BulletTrace : Line2D
{
	public Node2D parent;
	public override void _Ready()
	{
		TopLevel = true; //Yay this node is now Orphan!!!
	}

	public override void _Process(double delta)
	{
		AddPoint(GetNode<Node2D>("..").GlobalPosition);
		while (GetPointCount() > 10)
			RemovePoint(0);	
	}
}
