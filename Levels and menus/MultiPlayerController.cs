using Godot;
using System;

public partial class MultiPlayerController : Control
{
	[Export] PackedScene Level;

	[Export] string _address = "127.0.0.1";
	[Export] int _port = 8910;

	private ENetMultiplayerPeer _peer;
	private int _playerId;

	public override void _Ready()
	{
		//Connect(Multiplayer.PeerConnected);
		GD.Print("<<< START SERVER >>>");
		Multiplayer.PeerConnected += PlayerConnected;
		Multiplayer.PeerDisconnected += PlayerDisconnected;
		Multiplayer.ConnectedToServer += ConnectionSuccessful;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		//this.Hide();
	}

#region HostConnect
	public void HostGame()
	{
		_peer = new ENetMultiplayerPeer();
		var status = _peer.CreateServer(_port, 5);
		if (status != Error.Ok)
		{
			GD.PrintErr("Server could not be created:");
			GD.PrintErr($"Port: {_port}");
			return;
		}
		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Server started SUCCESSFULLY.");
		GD.Print("Waiting for players to connect ...");
	}

	public void ConnectToServer()
	{
		_peer = new ENetMultiplayerPeer();
		var status = _peer.CreateClient(_address, _port);
		if (status != Error.Ok)
		{
			GD.PrintErr("Creating client FAILED.");
			return;
		}
		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Created client.");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void LoadGame()
	{
		Node level = Level.Instantiate<Node>();
		GetTree().Root.AddChild(level);
		Hide();
	}
	#endregion

#region MultiplayerMethods
	public void PlayerConnected(long id)
	{
 		GD.Print($"Player <{id}> connected.");
	}

	public void PlayerDisconnected(long id)
	{
		GD.Print($"Player <{id}> disconected.");
	}

	private void ConnectionFailed()
	{
		GD.Print("Connection FAILED.");
		GD.Print("Could not connect to server.");
	}

	private void ConnectionSuccessful()
	{
		GD.Print("Connection SUCCESSFULL.");

		_playerId = Multiplayer.GetUniqueId();

		GD.Print(_playerId, "Sending player information to server.");
		GD.Print(_playerId, $"Id: {_playerId}");

		RpcId(1, "SendPlayerInformation", _playerId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInformation(int id)
	{
		//if //(GameManager.Player1id == 0)
		//{

		//}
	}
	#endregion

#region buttons
	public void OnHostPressed()
	{
		HostGame();
	}

	public void OnJoinPressed()
	{
		ConnectToServer();
	}

	public void OnPlayPressed()
	{
		LoadGame();
		Rpc("LoadGame");
	}
	#endregion
	/*public override void _Process(double delta)
	{

	}*/
}
