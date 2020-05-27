using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace SpawnMobXY
{
	[ApiVersion(2, 1)]
	public class Plugin  : TerrariaPlugin
	{
		public override string Name { get { return "SpawnMobXY"; } }
		public override string Author { get { return "Patrikk"; } }
		public override string Description { get { return "Extends spawnmob and spawnboss commands."; } }
		public override Version Version { get { return new Version(1, 0); } }

		public static List<string> bosses = new List<string>() { "brain", "destroyer", "fishron", "eater", "eye", "golem", "king slime", "plantera", "prime", "queen bee", "skeletron", "twins", "wof", "moon lord" };

		public Plugin(Main game)
			: base(game)
		{
		 
		}

		#region Initialize/Dispose
		public override void Initialize()
		{
			ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
		}
		protected override void Dispose(bool Disposing)
		{
			if (Disposing)
			{
				ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
			   
			}
			base.Dispose(Disposing);
		}
		#endregion

		private void OnInitialize(EventArgs args)
		{
			Action<Command> Add = c =>
			{
				TShockAPI.Commands.ChatCommands.RemoveAll(
				c2 => c2.Names.Select(s => s.ToLowerInvariant()).Intersect(c.Names.Select(s => s.ToLowerInvariant())).Any());
				TShockAPI.Commands.ChatCommands.Add(c);
			};

			Add(new Command(Permissions.spawnboss, SpawnBoss, "spawnboss", "sb") { HelpText = "Allows console and user to spawn bosses to coordinates." });
			Add(new Command(Permissions.spawnmob, SpawnMob, "spawnmob", "sm") { HelpText = "Allows console and user to spawn mobs to coordinates." });
			
		}

		public static void SpawnBoss(CommandArgs args)
		{

			if (args.Parameters.Count == 1 && args.Parameters[0] == "list")
			{
				args.Player.SendInfoMessage("Available bosses: {0}", string.Join(", ", bosses));
				return;
				
			}
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}spawnboss <boss type> [amount] [-health] [-x] [-y]", TShock.Config.CommandSpecifier);
				return;
			}

            string healthStr = "", xStr = "", yStr = "";
            Mono.Options.OptionSet options = new Mono.Options.OptionSet()
            {
                { "h|health=", v => healthStr = v},
                { "x=", v => xStr = v},
                { "y=", v => yStr = v},
            };
            List<string> parameters = options.Parse(args.Parameters);

            int health = -1;
            if (int.TryParse(healthStr, out health) && health < 0)
            {
                args.Player.SendErrorMessage("Health amount must be greater than 0!");
                return;
            }

            if (!args.Player.RealPlayer && (string.IsNullOrEmpty(xStr) || string.IsNullOrEmpty(yStr)))
            {
                args.Player.SendErrorMessage("You are required to set coordinates from console! ");
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}spawnboss <boss type> [amount] [-health] [-x] [-y]", TShock.Config.CommandSpecifier);
                return;
            }

            int amount = 1;
			if (args.Parameters.Count >= 2 && (!int.TryParse(args.Parameters[1], out amount) || amount <= 0))
			{
				args.Player.SendErrorMessage("Invalid boss amount!");
				return;
			}
			int TileX = args.Player.TileX;
			int TileY = args.Player.TileY;
			if (!int.TryParse(xStr, out TileX) || !int.TryParse(yStr, out TileY))
			{
				args.Player.SendErrorMessage("Invalid X and Y coordinates! Format: -x 1234 -y 1234");
				return;
			}

            if (TileX < 0 || TileX >= Main.maxTilesX || TileY < 0 || TileY >= Main.maxTilesY)
            {
                args.Player.SendErrorMessage("Given coordinates are invalid!");
                return;
            }

			NPC npc = new NPC();
			switch (args.Parameters[0].ToLower())
			{
				case "*":
				case "all":
					int[] npcIds = { 4, 13, 35, 50, 125, 126, 127, 134, 222, 245, 262, 266, 370, 398 };
					TSPlayer.Server.SetTime(false, 0.0);
					foreach (int i in npcIds)
					{
						npc.SetDefaults(i);
						SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					}
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned all bosses {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned all bosses {0} time(s).", amount);
					return;
				case "brain":
				case "brain of cthulhu":
					npc.SetDefaults(266);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
				   if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned the Brain of Cthulhu {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned the Brain of Cthulhu {0} time(s).", amount);
						return;
				case "destroyer":
					npc.SetDefaults(134);
					TSPlayer.Server.SetTime(false, 0.0);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned the Destroyer {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned the Destroyer {0} time(s).", amount);
					return;
				case "duke":
				case "duke fishron":
				case "fishron":
					npc.SetDefaults(370);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned Duke Fishron {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned Duke Fishron {0} time(s).", amount);
					return;
				case "eater":
				case "eater of worlds":
					npc.SetDefaults(13);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned the Eater of Worlds {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned the Eater of Worlds {0} time(s).", amount);
					return;
				case "eye":
				case "eye of cthulhu":
					npc.SetDefaults(4);
					TSPlayer.Server.SetTime(false, 0.0);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned the Eye of Cthulhu {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned the Eye of Cthulhu {0} time(s).", amount);
					return;
				case "golem":
					npc.SetDefaults(245);
					TSPlayer.Server.SetTime(false, 0.0);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
                        TSPlayer.All.SendSuccessMessage("{0} has spawned Golem {1} time(s).", args.Player.Name, amount);
                    else
                        args.Player.SendSuccessMessage("Spawned Golem {0} time(s).", amount);
                    return;
				case "king":
				case "king slime":
					npc.SetDefaults(50);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned King Slime {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned King Slime {0} time(s).", amount);
					return;
				case "plantera":
					npc.SetDefaults(262);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned Plantera {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned Plantera {0} time(s).", amount);
					return;
				case "prime":
				case "skeletron prime":
					npc.SetDefaults(127);
					TSPlayer.Server.SetTime(false, 0.0);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned Skeletron Prime {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned Skeletron Prime {0} time(s).", amount);
					return;
				case "queen":
				case "queen bee":
					npc.SetDefaults(222);
					TSPlayer.Server.SetTime(false, 0.0);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned Queen Bee {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned Queen Bee {0} time(s).", amount);
					return;
				case "skeletron":
					npc.SetDefaults(35);
					TSPlayer.Server.SetTime(false, 0.0);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned Skeletron {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned Skeletron {0} time(s).", amount);
					return;
				case "twins":
					TSPlayer.Server.SetTime(false, 0.0);
					npc.SetDefaults(125);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					npc.SetDefaults(126);
					SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health);
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned the Twins {1} time(s).", args.Player.Name, amount);
					else
						args.Player.SendSuccessMessage("Spawned the Twins {0} time(s).", amount);
					return;
				case "wof":
				case "wall of flesh":
					if (!args.Player.RealPlayer)
					{
						args.Player.SendErrorMessage("You cannot spawn Wall of Flesh from server console!");
						return;
					}
					if (Main.wofNPCIndex >= 0)
					{
						args.Player.SendErrorMessage("There is already a Wall of Flesh!");
						return;
					}
					if (args.Player.Y / 16f < Main.maxTilesY - 205)
					{
						args.Player.SendErrorMessage("You must spawn the Wall of Flesh in hell!");
						return;
					}
					NPC.SpawnWOF(new Vector2(args.Player.X, args.Player.Y));
					if (!args.Silent)
						TSPlayer.All.SendSuccessMessage("{0} has spawned the Wall of Flesh.", args.Player.Name);
					else
						args.Player.SendSuccessMessage("Spawned the Wall of Flesh.", amount);
					return;
                case "moon":
                case "moon lord":
                    npc.SetDefaults(398);
                    TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
                    if (!args.Silent)
                        TSPlayer.All.SendSuccessMessage("{0} has spawned the Moon Lord {1} time(s).", args.Player.Name, amount);
                    else
                        args.Player.SendSuccessMessage("Spawned the Moonlord.", amount);
                    return;
                default:
                    args.Player.SendErrorMessage("Invalid boss type!");
                    return;
            }
		}

        public static void SpawnMob(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}spawnmob <mob type> [amount] [-health] [-x] [-y]", TShock.Config.CommandSpecifier);
				return;
			}

			int amount = 1;
			if (args.Parameters.Count >= 2 && (!int.TryParse(args.Parameters[1], out amount) || amount <= 0))
			{
				args.Player.SendErrorMessage("Invalid mob amount!");
				return;
			}

            string healthStr = "", xStr = "", yStr = "";
            Mono.Options.OptionSet options = new Mono.Options.OptionSet()
            {
                { "h|health=", v => healthStr = v},
                { "x=", v => xStr = v},
                { "y=", v => yStr = v},
            };
            List<string> parameters = options.Parse(args.Parameters);

            int health = -1;
            if (int.TryParse(healthStr, out health) && health < 0)
            {
                args.Player.SendErrorMessage("Health amount must be greater than 0!");
                return;
            }

            if (!args.Player.RealPlayer && (string.IsNullOrEmpty(xStr) || string.IsNullOrEmpty(yStr)))
            {
                args.Player.SendErrorMessage("You are required to set coordinates from console! ");
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}spawnmob <mob type> [amount] [-health] [-x] [-y]", TShock.Config.CommandSpecifier);
                return;
            }

            int TileX = args.Player.TileX;
            int TileY = args.Player.TileY;
            if (!string.IsNullOrEmpty(xStr) || !string.IsNullOrEmpty(xStr))
            {
                if (!int.TryParse(xStr, out TileX) || !int.TryParse(yStr, out TileY))
                {
                    args.Player.SendErrorMessage("Invalid X and Y coordinates! Format: -x 1234 -y 1234");
                    return;
                }
            }

            if (TileX < 0 || TileX >= Main.maxTilesX || TileY < 0 || TileY >= Main.maxTilesY)
            {
                args.Player.SendErrorMessage("Given coordinates are invalid!");
                return;
            }

            amount = Math.Min(amount, Main.maxNPCs);

			var npcs = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
			if (npcs.Count == 0)
			{
				args.Player.SendErrorMessage("Invalid mob type!");
			}
			else if (npcs.Count > 1)
			{
				args.Player.SendMultipleMatchError(npcs.Select(n => n.FullName));
			}
			else
			{
				var npc = npcs[0];
				if (npc.type >= 1 && npc.type < Main.maxNPCTypes && npc.type != 113)
				{
                    SpawnNPC(npc.type, npc.FullName, amount, TileX, TileY, health, 35, 10);

					if (args.Silent)
					{
						args.Player.SendSuccessMessage("Spawned {0} {1} time(s).", npc.FullName, amount);
					}
					else
					{
						TSPlayer.All.SendSuccessMessage("{0} has spawned {1} {2} time(s).", args.Player.Name, npc.FullName, amount);
					}
				}
				else if (npc.type == 113)
				{
					if (Main.wofNPCIndex >= 0 || (args.Player.Y / 16f < (Main.maxTilesY - 205)))
					{
						args.Player.SendErrorMessage("Can't spawn Wall of Flesh!");
						return;
					}
					NPC.SpawnWOF(new Vector2(args.Player.X, args.Player.Y));
					if (args.Silent)
					{
						args.Player.SendSuccessMessage("Spawned Wall of Flesh!");
					}
					else
					{
						TSPlayer.All.SendSuccessMessage("{0} has spawned a Wall of Flesh!", args.Player.Name);
					}
				}
				else
				{
					args.Player.SendErrorMessage("Invalid mob type!");
				}
			}
		}
        private static void SpawnNPC(int type, string fullName, int amount, int tileX, int tileY, int health = -1, int tileXRange = 100, int tileYRange = 50)
        {
            if (health == 0)
                TSPlayer.Server.SpawnNPC(type, fullName, amount, tileX, tileY, tileXRange, tileYRange);
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    TShock.Utils.GetRandomClearTileWithInRange(tileX, tileY, tileXRange, tileYRange, out tileX, out tileY);
                    int npcIndex = NPC.NewNPC(tileX * 16, tileY * 16, type);
                    var npc = Main.npc[npcIndex];
                    npc.life = health;
                    npc.lifeMax = health;

                    TSPlayer.All.SendData(PacketTypes.NpcUpdate, fullName, npcIndex);
                }
            }
        }
    }
}
