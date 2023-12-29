using Godot;
using System;

public partial class SceneManager : Node
{
	[Export]
	private PackedScene player;

	public override void _Ready()
	{
		int index = 0;
		foreach(var item in GameManager.Players)
		{
			//CharacterBody here is the name of the class and nor CharacterBody2D
			Node2D currentPlayer = player.Instantiate<Node2D>();
			currentPlayer.Name = item.Id.ToString();
			AddChild(currentPlayer);
			foreach (Marker2D spawnpoint in GetTree().GetNodesInGroup("SpawnPoints"))
			{
				if(int.Parse(spawnpoint.Name) == index)
				{
					currentPlayer.GlobalPosition = spawnpoint.GlobalPosition;
					
				}
			}
			index++;
		}
	}
}
