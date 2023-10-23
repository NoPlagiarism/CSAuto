﻿using System.Collections.Generic;

namespace CSAuto.Languages
{
    class English
    {
        static Dictionary<string, string> translation = new Dictionary<string, string>()
        {
            ["language_english"] = "English",
            ["language_russian"] = "Russian",
            ["menu_debug"] = "Debug",
            ["menu_language"] = "Language",
            ["menu_mobile"] = "Phone notifications",
            ["menu_exit"] = "Exit",
            ["menu_automation"] = "Automation",
            ["menu_general"] = "General",
            ["menu_options"] = "Settings",
            ["menu_opendebug"] = "Open Debug Window",
            ["menu_autocheckforupdates"] = "Check for updates on startup",
            ["menu_checkforupdates"] = "Check for updates",
            ["menu_enterip"] = "Enter IP address",
            ["menu_discordrpc"] = "Discord RPC",
            ["menu_enabled"] = "Enabled",
            ["menu_autocheckupdates"] = "Auto Check For Updates",
            ["menu_autospotify"] = "Auto Pause/Resume Spotify",
            ["menu_startup"] = "Start With Windows",
            ["menu_continuespray"] = "Continue Spraying (Experimental)",
            ["menu_savedebugframes"] = "Save Frames",
            ["menu_savedebuglogs"] = "Save Logs",
            ["menu_autoaccept"] = "Auto Accept Match",
            ["menu_autobuyarmor"] = "Auto Buy Armor",
            ["menu_autobuydefuse"] = "Auto Buy Defuse Kit",
            ["menu_preferarmor"] = "Prefer armor",
            ["menu_autobuy"] = "Auto Buy",
            ["menu_discord"] = "Discord",
            ["menu_autoreload"] = "Auto Reload",
            ["title_debugwind"] = "CSAuto - Debug Window",
            ["title_update"] = "Check for updates (CSAuto)",
            ["title_restartneeded"] = "Restart app",
            ["title_error"] = "Error",
            ["inputtitle_mobileip"] = "Mobile Phone IP Address",
            ["inputtext_mobileip"] = "Enter the IP address you see in the app:",
            ["msgbox_latestversion"] = "You have the latest version!",
            ["msgbox_newerversion1"] = "Found newer verison",
            ["msgbox_newerversion2"] = "would you like to download it?",
            ["msgbox_restartneeded"] = "You must restart the program to apply these changes",
            ["error_update"] = "Couldn't check for updates. Try again later",
            ["error_startup1"] = "An error ocurred",
            ["error_startup2"] = "Try to download the latest version from github.",
            ["menu_notifications"] = "Notifications",
            ["menu_acceptednotification"] = "Accepted match",
            ["menu_mapnotification"] = "Loaded on map",
            ["menu_lobbynotification"] = "Loaded in lobby",
            ["menu_connectednotification"] = "Computer connected",
            ["menu_crashednotification"] = "Game crashed",
            ["server_computer"] = "Computer",
            ["server_online"] = "is online",
            ["server_loadedmap"] = "Loaded on map",
            ["server_mode"] = "in mode",
            ["server_loadedlobby"] = "Loaded in lobby!",
            ["server_gamecrash"] = "The game crashed!",
            ["server_acceptmatch"] = "Accepted a match!",
            ["menu_bombnotification"] = "Bomb information",
            ["server_timeleft"] = "Bomb seconds left:",
            ["server_bombexplode"] = "The bomb exploded",
            ["server_bombdefuse"] = "Bomb has been defused",
            ["exception_steamnotfound"] = "Couldn't find Steam Path",
            ["exception_nonetworkadapter"] = "No network adapters with an IPv4 address in the system!",
            ["exception_csgonotfound"] = "Couldn't find CS:GO directory",
            ["menu_entersteamkey"] = "Enter Steam Web API Key",
            ["inputtitle_steamkey"] = "Steam Web API Key",
            ["inputtext_steamkey"] = "Please enter your Steam Web API Key",
            ["menu_lobbycount"] = "Show players lobby count",
            ["menu_entertelegramid"] = "Enter Telegram Chat ID",
            ["inputtitle_telegramid"] = "Telegram ID",
            ["inputtext_telegramid"] = "Please enter your Telegram chat id, use the faq if you dont know how",
            ["menu_mobileappenabled"] = "Mobile App Enabled",
            ["error_telegrammessage"] = "There was an error when sending the Telegram message, make sure your chat id is correct",
            ["inputtext_botlinkbutton"] = "Bot Link",
            ["menu_darktheme"] = "Dark Theme",
            ["menu_changelog"] = "Changelog",
            ["title_color"] = "Color: ",
            ["inputtext_linkbutton"] = "Bot Link",
            ["server_bombplanted"] = "Bomb has been planted",
            ["menu_discordcustomization"] = "Customize Discord RPC",
            ["title_discordstate"] = "State template",
            ["title_discorddetails"] = "Details template",
            ["menu_discordlobby"] = "Lobby RPC",
            ["menu_discordingame"] = "In Game RPC",
            ["title_discordrpctemplates"] = "Templates:",
            ["discord_friendcode"] = "{FriendCode} - Your friencode",
            ["discord_gamemode"] = "{Gamemode} - The current match mode",
            ["discord_map"] = "{Map} - The current match map",
            ["discord_teamscore"] = "{TeamScore} - Your team score",
            ["discord_myteam"] = "{MyTeam} - Your current team",
            ["discord_roundstate"] = "{RoundState} - Current round state",
            ["discord_matchstate"] = "{MatchState} - Current match state",
            ["discord_enemyscore"] = "{EnemyScore} - Enemy team score",
            ["discord_enemyteam"] = "{EnemyTeam} - Enemy team name",
            ["discord_tscore"] = "{TScore} - T side score",
            ["discord_ctscore"] = "{CTScore} - CT side score",
            ["discord_steamid"] = "{SteamID} - Your steam id",
            ["discord_result"] = "Result",
            ["discord_restartneeded"] = "You may need to restart the app for changes to apply",
            ["menu_oldautobuy"] = "Disable Chat/Console before auto buy",
            ["inputtext_testmessagetelegram"] = "Send test message",
            ["discord_label"] = "Button Label",
            ["discord_url"] = "Button Url",
            ["menu_discordbuttons"] = "Discord RPC Buttons",
            ["discord_removebutton"] = "Remove Button",
            ["discord_addbutton"] = "Add Button",
            ["inputtext_label"] = "Enter Label",
            ["inputtext_enterlabel"] = "Please enter your desired label",
            ["inputtext_url"] = "Enter URL",
            ["inputtext_enterurl"] = "Please enter your desired URL",
            ["error_entervalid"] = "Please enter valid inputs",
            ["error_max1discord"] = "There is a max amount of 1 button (Discord Limitations)",
            ["error_appcrashed"] = "An unexpected error has accured!, i will open Error_Log.txt, please open an issue on github with the log Attached!",
            ["error_discordselecttemplate"] = "Please select a button template",
            ["discord_createtemplate"] = "Use template",
            ["error_loadcolors"] = "Couldn't load colors for button, be aware that auto accept wont work!",
            ["menu_sendacceptbutton"] = "Send capture to telegram on accept",
            ["error_loadchangelog"] = "Couldn't load the changelog, please check your internet connection",
            ["title_warning"] = "Warning",
            ["error_createfiles"] = "Can't create files, logs and debug stuff wont be saved",
            ["discord_name"] = "{Name} - Current player name",
            ["discord_kills"] = "{Kills} - Current player kills",
            ["discord_deaths"] = "{Deaths} - Current player deaths",
            ["discord_mvps"] = "{MVPS} - Current player mvps",
        };
        public static string Get(string category)
        {
            if (translation.ContainsKey(category)) return translation[category]; else return category;
        }
    }
}
