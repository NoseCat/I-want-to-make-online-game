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
		var status = _peer.CreateServer(_port, 10);
		if (status != Error.Ok)
		{
			GD.PrintErr("Server could not be created:");
			GD.PrintErr($"Port: {_port}");
			GD.PrintErr(status.ToString());
			return;
		}
		_peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Server started SUCCESSFULLY.");
		SendPlayerInformation(GetNode<LineEdit>("LineEdit").Text, 1);
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

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
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

		RpcId(1, "SendPlayerInformation", GetNode<LineEdit>("LineEdit").Text, _playerId);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInformation(string name, int id)
	{
		PlayerInfo playerInfo = new PlayerInfo(){Name = name, Id = id};
		if(!GameManager.Players.Contains(playerInfo))
		{
			GameManager.Players.Add(playerInfo);
		}
		if(Multiplayer.IsServer())
		{
			foreach(var item in GameManager.Players)
			{
				Rpc("SendPlayerInformation", item.Name, item.Id);
			}
		}
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
		Rpc("LoadGame");
	}
	#endregion
}
