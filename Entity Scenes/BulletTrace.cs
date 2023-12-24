using Godot;
using System;

public partial class BulletTrace : Line2D
{
	public Node2D parent;
	public override void _Ready()
	{
		TopLevel = true; //Yay this node is now Orphan!!!
		parent = GetNode<Node2D>("..");
		AddPoint(GetNode<Marker2D>("../Marker2D").Position);
	}

	public override void _Process(double delta)
	{
		//if(Visible)
		AddPoint(parent.GlobalPosition);
		while (GetPointCount() > 10)
			RemovePoint(0);
	}
}
