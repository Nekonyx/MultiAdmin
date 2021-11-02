using System;
using MultiAdmin.Features.Attributes;

namespace MultiAdmin.Features
{
	[Feature]
	internal class FreezeKiller : Feature, IEventTick, IEventServerStart, IEventServerStop
	{
		public const string PingCommand = "nekowerenostrangerstoloveyouknowtherulesandsodoi";

		private static int _counter;
		private bool isActive;

		public FreezeKiller(Server server) : base(server)
		{
		}

		public static void ResetCounter()
		{
			_counter = 0;
		}

		public override string GetFeatureName()
		{
			return "Freeze Killer";
		}

		public override string GetFeatureDescription()
		{
			return "Periodically checks if the server is frozen or not and kills it if it stops responding.";
		}

		public override void Init()
		{
		}

		public void OnServerStart()
		{
			SetState(true);
		}

		public void OnServerStop()
		{
			SetState(false);
		}

		public void OnTick()
		{
			if (!isActive)
			{
				return;
			}

			_counter++;

			// Ping
			Ping();

			// Every 10 seconds
			if (_counter % 10 == 0)
			{
				Server.Write($"The server has not responded for {_counter} seconds.", ConsoleColor.Yellow);
			}

			// After 30 seconds
			if (_counter == 30)
			{
				Server.Write("The server is not responding to requests. Trying to restart...", ConsoleColor.Red);
				Kill();
			}
		}

		public override void OnConfigReload()
		{
		}

		private void SetState(bool active)
		{
			_counter = 0;
			isActive = active;
		}

		private void Ping()
		{
			if (!isActive || Server.SessionSocket == null || !Server.SessionSocket.Connected)
			{
				return;
			}

			Server.SendMessage(PingCommand);
		}

		private void Kill()
		{
			SetState(false);
			Server.RestartServer();
		}
	}
}
