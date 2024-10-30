using Rocket.API;
using SDG.Unturned;
using Steamworks;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Anomoly.KitsPlusv2.Utils
{
    public static class ChatUtility
    {
        private static readonly Regex ColorOpeningMatch = new Regex(@"\[color=[a-zA-Z0-9#]+\]");
        private static readonly Regex ColorClosing = new Regex(@"\[/color\]");

        private static readonly string IconURL = "https://cdn.discordapp.com/attachments/1116938944047743067/1259699673849921616/UconomyPlusIcon.png?ex=668ca23e&is=668b50be&hm=918149f2bd70d8d60180b4ae8715e55ffea049b13b07afbe0b12a088b841ed58&";

        public static string ReformatColor(this string key)
        {
            var openings = ColorOpeningMatch.Matches(key);
            foreach (Match opening in openings)
            {
                if (!opening.Success)
                    continue;

                var color = opening.Value.Substring(7, opening.Value.Length - 8);
                key = key.Remove(opening.Index, opening.Length)
                         .Insert(opening.Index, $"<color={color}>");
            }

            key = ColorClosing.Replace(key, "</color>");
            return key;
        }

        public static void Say(string message, bool rich = true)
        {
            Say(message, Color.white, rich);
        }

        public static void Say(IRocketPlayer player, string message, bool rich = true)
        {
            Say(player, message, Color.white, rich);
        }

        public static void Say(CSteamID steamId, string message, bool rich = true)
        {
            Say(steamId, message, Color.white, rich);
        }

        public static void Say(string message, Color color, bool rich = true)
        {
            Logger.Log("Broadcast: " + message, ConsoleColor.Gray);
            ChatManager.serverSendMessage(message.ReformatColor(), color, null, null, EChatMode.GLOBAL, IconURL, rich);
        }

        public static void Say(string message, Color color)
        {
            Say(message, color, false);
        }

        public static void Say(IRocketPlayer player, string message, Color color, bool rich = true)
        {
            if (player is ConsolePlayer)
            {
                Logger.Log(message, ConsoleColor.Gray);
            }
            else
            {
                Say(new CSteamID(ulong.Parse(player.Id)), message, color, rich);
            }
        }

        public static void Say(CSteamID steamId, string message, Color color, bool rich = true)
        {
            if (steamId == null || steamId.ToString() == "0")
            {
                Logger.Log(message, ConsoleColor.Gray);
            }
            else
            {

                string prefix = "[<color=#ed5f55>KitsPlus</color>]: ";
                message = prefix + message;

                SteamPlayer toPlayer = PlayerTool.getSteamPlayer(steamId);
                ChatManager.serverSendMessage(message.ReformatColor(), color, null, toPlayer, EChatMode.SAY, IconURL, rich);
            }
        }
    }
}
