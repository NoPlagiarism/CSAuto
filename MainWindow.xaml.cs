﻿using Microsoft.Win32;
using NotifyIconLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using CSAuto.Exceptions;
using CSAuto.Utils;
namespace CSAuto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constants
        /// </summary>
        const string VER = "1.0.2a";
        const string PORT = "11523";
        const string TIMEOUT = "5.0";
        const string BUFFER = "0.1";
        const string THROTTLE = "0.5";
        const string HEARTBEAT = "10.0";
        const string INTEGRATION_FILE = "\"CSAuto Integration v" + VER + "\"\r\n{\r\n\"uri\" \"http://localhost:"+ PORT+
            "\"\r\n\"timeout\" \""+ TIMEOUT+"\"\r\n\"" +
            "buffer\"  \""+ BUFFER+"\"\r\n\"" +
            "throttle\" \""+THROTTLE+"\"\r\n\"" +
            "heartbeat\" \""+HEARTBEAT+"\"\r\n\"data\"\r\n{\r\n   \"provider\"            \"1\"\r\n   \"map\"                 \"1\"\r\n   \"round\"               \"1\"\r\n   \"player_id\"           \"1\"\r\n   \"player_state\"        \"1\"\r\n   \"player_weapons\"      \"1\"\r\n   \"player_match_stats\"  \"1\"\r\n   \"bomb\" \"1\"\r\n}\r\n}";
        /// <summary>
        /// Publics
        /// </summary>
        public GSIDebugWindow debugWind = null;
        /// <summary>
        /// Readonly
        /// </summary>
        readonly NotifyIconWrapper notifyicon = new NotifyIconWrapper();
        readonly ContextMenu exitcm = new ContextMenu();
        readonly System.Windows.Threading.DispatcherTimer appTimer = new System.Windows.Threading.DispatcherTimer();
        readonly Color BUTTON_COLOR = Color.FromArgb(76, 175, 80);
        readonly Color ACTIVE_BUTTON_COLOR = Color.FromArgb(90, 203, 94);
        readonly MenuItem startUpCheck = new MenuItem();
        readonly MenuItem saveFramesDebug = new MenuItem();
        readonly MenuItem autoAcceptMatchCheck = new MenuItem();
        readonly MenuItem autoReloadCheck = new MenuItem();
        readonly MenuItem autoBuyArmor = new MenuItem();
        readonly MenuItem autoBuyDefuseKit = new MenuItem();
        readonly MenuItem preferArmorCheck = new MenuItem();
        readonly MenuItem saveLogsCheck = new MenuItem();
        readonly MenuItem continueSprayingCheck = new MenuItem();
        readonly MenuItem autoCheckForUpdates = new MenuItem();
        readonly MenuItem autoPauseResumeSpotify = new MenuItem();
        readonly MenuItem autoBuyMenu = new MenuItem
        {
            Header = "Auto Buy"
        };
        readonly MenuItem autoReloadMenu = new MenuItem
        {
            Header = "Auto Reload"
        };
        /// <summary>
        /// Privates
        /// </summary>
        private DiscordRpc.EventHandlers handlers;
        private DiscordRpc.RichPresence presence;
        private readonly AutoResetEvent _waitForConnection = new AutoResetEvent(false);
        private string integrationPath = null;
        private HttpListener _listener;
        private bool ServerRunning = false;
        /// <summary>
        /// Members
        /// </summary>
        Point cs2Resolution = new Point();
        GameState GameState = new GameState(null);
        int frame = 0;
        bool cs2Running = false;
        bool inGame = false;
        bool cs2Active = false;
        Activity? lastActivity;
        Phase? matchState;
        Phase? roundState;
        Weapon weapon;
        int round = -1;
        public ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                InitializeDiscordRPC();
                KillDuplicates();
                //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
                MenuItem debugMenu = new MenuItem
                {
                    Header = "Debug"
                };
                MenuItem exit = new MenuItem
                {
                    Header = "Exit"
                };
                MenuItem automation = new MenuItem
                {
                    Header = "Automation"
                };
                MenuItem options = new MenuItem
                {
                    Header = "Options"
                };
                exit.Click += Exit_Click;
                MenuItem about = new MenuItem
                {
                    Header = $"{typeof(MainWindow).Namespace} - {VER}",
                    IsEnabled = false,
                    Icon = new System.Windows.Controls.Image
                    {
                        Source = ToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location))
                    }
                };
                MenuItem openDebugWindow = new MenuItem
                {
                    Header = "Open Debug Window"
                };
                openDebugWindow.Click += OpenDebugWindow_Click;
                MenuItem checkForUpdates = new MenuItem
                {
                    Header = "Check for updates"
                };
                checkForUpdates.Click += CheckForUpdates_Click;
                autoCheckForUpdates.IsChecked = Properties.Settings.Default.autoCheckForUpdates;
                autoCheckForUpdates.Header = "Check For Updates On Startup";
                autoCheckForUpdates.IsCheckable = true;
                autoCheckForUpdates.Click += AutoCheckForUpdates_Click;
                autoPauseResumeSpotify.IsChecked = Properties.Settings.Default.autoPausePlaySpotify;
                autoPauseResumeSpotify.Header = "Auto Pause/Resume Spotify";
                autoPauseResumeSpotify.IsCheckable = true;
                autoPauseResumeSpotify.Click += AutoPauseResumeSpotify_Click;
                startUpCheck.IsChecked = Properties.Settings.Default.runAtStartUp;
                startUpCheck.Header = "Start With Windows";
                startUpCheck.IsCheckable = true;
                startUpCheck.Click += StartUpCheck_Click;
                continueSprayingCheck.IsChecked = Properties.Settings.Default.ContinueSpraying;
                continueSprayingCheck.Header = "Continue Spraying (Experimental)";
                continueSprayingCheck.IsCheckable = true;
                continueSprayingCheck.Click += ContinueSprayingCheck_Click;
                saveFramesDebug.IsChecked = Properties.Settings.Default.saveDebugFrames;
                saveFramesDebug.Header = "Save Frames";
                saveFramesDebug.IsCheckable = true;
                saveFramesDebug.Click += DebugCheck_Click;
                saveLogsCheck.IsChecked = Properties.Settings.Default.saveLogs;
                saveLogsCheck.Header = "Save Logs";
                saveLogsCheck.IsCheckable = true;
                saveLogsCheck.Click += SaveLogsCheck_Click;
                autoAcceptMatchCheck.IsChecked = Properties.Settings.Default.autoAcceptMatch;
                autoAcceptMatchCheck.Header = "Auto Accept Match";
                autoAcceptMatchCheck.IsCheckable = true;
                autoAcceptMatchCheck.Click += AutoAcceptMatchCheck_Click;
                autoBuyArmor.IsChecked = Properties.Settings.Default.autoBuyArmor;
                autoBuyArmor.Header = "Auto Buy Armor";
                autoBuyArmor.IsCheckable = true;
                autoBuyArmor.Click += AutoBuyArmor_Click;
                autoBuyDefuseKit.IsChecked = Properties.Settings.Default.autoBuyDefuseKit;
                autoBuyDefuseKit.Header = "Auto Buy Defuse Kit";
                autoBuyDefuseKit.IsCheckable = true;
                autoBuyDefuseKit.Click += AutoBuyDefuseKit_Click;
                preferArmorCheck.IsChecked = Properties.Settings.Default.preferArmor;
                preferArmorCheck.Header = "Prefer armor";
                preferArmorCheck.IsCheckable = true;
                preferArmorCheck.Click += PreferArmorCheck_Click;
                autoReloadCheck.IsChecked = Properties.Settings.Default.autoReload;
                autoReloadCheck.Header = "Enabled";
                autoReloadCheck.IsCheckable = true;
                autoReloadCheck.Click += AutoReloadCheck_Click;
                debugMenu.Items.Add(saveFramesDebug);
                debugMenu.Items.Add(saveLogsCheck);
                debugMenu.Items.Add(openDebugWindow);
                autoReloadMenu.Items.Add(autoReloadCheck);
                autoReloadMenu.Items.Add(continueSprayingCheck);
                autoBuyMenu.Items.Add(preferArmorCheck);
                autoBuyMenu.Items.Add(autoBuyArmor);
                autoBuyMenu.Items.Add(autoBuyDefuseKit);
                automation.Items.Add(autoBuyMenu);
                automation.Items.Add(autoReloadMenu);
                automation.Items.Add(autoAcceptMatchCheck);
                automation.Items.Add(autoPauseResumeSpotify);
                options.Items.Add(startUpCheck);
                options.Items.Add(autoCheckForUpdates);
                exitcm.Items.Add(about);
                exitcm.Items.Add(debugMenu);
                exitcm.Items.Add(new Separator());
                exitcm.Items.Add(automation);
                exitcm.Items.Add(options);
                exitcm.Items.Add(new Separator());
                exitcm.Items.Add(checkForUpdates);
                exitcm.Items.Add(exit);
                Top = -1000;
                Left = -1000;
                exitcm.StaysOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void InitializeDiscordRPC()
        {
            string dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString());
            string dllPath = Path.Combine(dirName, "discord-rpc-win32.dll");
            using (Stream stm = Assembly.GetExecutingAssembly().GetManifestResourceStream(
              "CSAuto.discord-rpc-w32.dll"))
            {
                try
                {
                    using (Stream outFile = File.Create(dllPath))
                    {
                        const int sz = 4096;
                        byte[] buf = new byte[sz];
                        while (true)
                        {
                            int nRead = stm.Read(buf, 0, sz);
                            if (nRead < 1)
                                break;
                            outFile.Write(buf, 0, nRead);
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.WriteLine(e.ToString());
                }
            }
            this.handlers = default(DiscordRpc.EventHandlers);
            DiscordRpc.Initialize("1121012657126916157", ref this.handlers, true, null);
        }

        private void AutoCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoCheckForUpdates = autoCheckForUpdates.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void OpenDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            Notifyicon_LeftMouseButtonDoubleClick(null, null);
        }

        private void AutoPauseResumeSpotify_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoPausePlaySpotify = autoPauseResumeSpotify.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void ContinueSprayingCheck_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ContinueSpraying = continueSprayingCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void CheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CheckForUpdatesAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        async Task CheckForUpdatesAsync()
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient() { Encoding = Encoding.UTF8 };
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                Log.WriteLine("Checking for updates");
                string webInfo = await client.DownloadStringTaskAsync("https://github.com/MurkyYT/CSAuto/releases/latest");
                string latestVersion = webInfo.Split(new string[] { "https://github.com/MurkyYT/CSAuto/releases/tag/" }, StringSplitOptions.None)[1].Split('&')[0].Trim();
                Log.WriteLine($"The latest version is {latestVersion}");
                if (latestVersion == VER)
                {
                    Log.WriteLine("Latest version installed");
                    MessageBox.Show("You have the latest version!", "Check for updates (CSAuto)", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Log.WriteLine($"Newer version found {VER} --> {latestVersion}");
                    MessageBoxResult result = MessageBox.Show($"Found newer verison ({latestVersion}) would you like to download it?", "Check for updates (CSAuto)", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        Log.WriteLine("Downloading latest version");
                        System.Diagnostics.Process.Start("https://github.com/MurkyYT/CSAuto/releases/latest/download/CSAuto.exe");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Couldn't check for updates - '{ex.Message}'");
                MessageBox.Show($"Couldn't check for updates\n'{ex.Message}'", "Check for updates (CSAuto)", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveLogsCheck_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.saveLogs = saveLogsCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void PreferArmorCheck_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.preferArmor = preferArmorCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void AutoBuyDefuseKit_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoBuyDefuseKit = autoBuyDefuseKit.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void AutoBuyArmor_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoBuyArmor = autoBuyArmor.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void AutoReloadCheck_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoReload = autoReloadCheck.IsChecked;
            Properties.Settings.Default.Save();
        }
        public bool StartGSIServer()
        {
            if (ServerRunning)
                return false;

            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:" + PORT + "/");
            Thread ListenerThread = new Thread(new ThreadStart(Run));
            try
            {
                _listener.Start();
            }
            catch (HttpListenerException)
            {
                return false;
            }
            ServerRunning = true;
            ListenerThread.Start();
            return true;
        }

        /// <summary>
        /// Stops listening for HTTP POST requests
        /// </summary>
        public void StopGSIServer()
        {
            if (!ServerRunning)
                return;
            ServerRunning = false;
            _listener.Close();
            (_listener as IDisposable).Dispose();
        }

        private void Run()
        {
            while (ServerRunning)
            {
                _listener.BeginGetContext(ReceiveGameState, _listener);
                _waitForConnection.WaitOne();
                _waitForConnection.Reset();
            }
            try
            {
                _listener.Stop();
            }
            catch (ObjectDisposedException)
            { /* _listener was already disposed, do nothing */ }
        }

        private void ReceiveGameState(IAsyncResult result)
        {
            HttpListenerContext context;
            try
            {
                context = _listener.EndGetContext(result);
            }
            catch (ObjectDisposedException)
            {
                // Listener was Closed due to call of Stop();
                return;
            }
            finally
            {
                _waitForConnection.Set();
            }

            HttpListenerRequest request = context.Request;
            string JSON;

            using (Stream inputStream = request.InputStream)
            {
                using (StreamReader sr = new StreamReader(inputStream))
                {
                    JSON = sr.ReadToEnd();
                }
            }
            using (HttpListenerResponse response = context.Response)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.Close();
            }
            try
            {
                GameState = new GameState(JSON);
                Activity? activity = GameState.Player.CurrentActivity;
                Phase? currentMatchState = GameState.Match.Phase;
                Phase? currentRoundState = GameState.Round.Phase;
                Weapon currentWeapon = GameState.Player.ActiveWeapon;
                int currentRound = GameState.Round.CurrentRound;
                if (debugWind != null)
                    debugWind.UpdateText(JSON);
                if (lastActivity != activity)
                    Log.WriteLine($"Activity: {(lastActivity == null ? "None" : lastActivity.ToString())} -> {(activity == null ? "None" : activity.ToString())}");
                if (currentMatchState != matchState)
                    Log.WriteLine($"Match State: {(matchState == null ? "None" : matchState.ToString())} -> {(currentMatchState == null ? "None" : currentMatchState.ToString())}");
                if (currentRoundState != roundState)
                    Log.WriteLine($"Round State: {(roundState == null ? "None" : roundState.ToString())} -> {(currentRoundState == null ? "None" : currentRoundState.ToString())}");
                if (round != currentRound)
                    Log.WriteLine($"RoundNo: {(round == -1 ? "None" : round.ToString())} -> {(currentRound == -1 ? "None" : currentRound.ToString())}");
                //if (GetWeaponName(weapon) != GetWeaponName(currentWeapon))
                //    Log.WriteLine($"Current Weapon: {(weapon == null ? "None" : GetWeaponName(weapon))} -> {(currentWeapon == null ? "None" : GetWeaponName(currentWeapon))}");
                lastActivity = activity;
                matchState = currentMatchState;
                roundState = currentRoundState;
                round = currentRound;
                weapon = currentWeapon;
                inGame = activity != Activity.Menu;
                if (cs2Active && !GameState.Player.IsSpectating)
                {
                    if (Properties.Settings.Default.autoReload && inGame)
                    {
                        TryToAutoReload();
                    }
                    if (Properties.Settings.Default.preferArmor)
                    {
                        AutoBuyArmor();
                        AutoBuyDefuseKit();
                    }
                    else
                    {
                        AutoBuyDefuseKit();
                        AutoBuyArmor();
                    }
                    if (Properties.Settings.Default.autoPausePlaySpotify)
                    {
                        AutoPauseResumeSpotify();
                    }
                }
                UpdateDiscordRPC();

                //Log.WriteLine($"Got info from GSI\nActivity:{activity}\nCSGOActive:{csgoActive}\nInGame:{inGame}\nIsSpectator:{IsSpectating(JSON)}");

            }
            catch(Exception ex) 
            {
                Log.WriteLine("Error happend while getting GSI Info\n"+ex);
            }
        }

        private void UpdateDiscordRPC()
        {
            if (cs2Running)
            {
                presence.details = GameState.Player.CurrentActivity == Activity.Menu ? "In Menu" : $"{GameState.Match.TScore} (T) - {GameState.Match.CTScore} (CT)";
                presence.state = GameState.Player.CurrentActivity != Activity.Menu ? $"{GameState.Match.Map} ({GameState.Match.Mode}) as {GameState.Player.Team}" : "";
                presence.largeImageKey = GameState.Player.CurrentActivity == Activity.Menu ? "csgo_icon" : $"map_icon_{GameState.Match.Map}";
                presence.largeImageText = GameState.Player.CurrentActivity == Activity.Menu ? "Menu" : GameState.Match.Map;
                if (GameState.Player.CurrentActivity != Activity.Menu)
                {
                    presence.smallImageKey = "csgo_icon";
                    presence.smallImageText = "CS2";
                }
                else
                {
                    presence.smallImageKey = null;
                    presence.smallImageText = null;
                }
                DiscordRpc.UpdatePresence(ref this.presence);
            }
            else
            {
                DiscordRpc.Shutdown();
            }
        }

        private void AutoPauseResumeSpotify()
        {
            if(GameState.Player.CurrentActivity == Activity.Playing)
            {
                if (GameState.Player.Health > 0 && GameState.Player.SteamID == GameState.MySteamID && Spotify.IsPlaying())
                {
                    Spotify.Pause();
                }
                else if (!Spotify.IsPlaying() && GameState.Player.SteamID != GameState.MySteamID)
                {
                    Spotify.Resume();
                }
            }
            else if (!Spotify.IsPlaying() && GameState.Player.CurrentActivity != Activity.Textinput)
            {
                Spotify.Resume();
            }

        }

        bool IsForegroundProcess(uint pid)
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == null) return false;

            if (GetWindowThreadProcessId(hwnd, out uint foregroundPid) == (IntPtr)0) return false;

            return (foregroundPid == pid);
        }
        private void TimerCallback(object sender, EventArgs e)
        {
            if (!ServerRunning)
            {
                Log.WriteLine("Starting GSI Server");
                StartGSIServer();
            }
            try
            {
                uint pid = 0;
                Process[] prcs = Process.GetProcessesByName("cs2");
                cs2Running = prcs.Length > 0;
                if (cs2Running)
                    pid = (uint)prcs[0].Id;
                cs2Active = IsForegroundProcess(pid);
                if (cs2Active)
                {
                    cs2Resolution = new Point(
                            (int)SystemParameters.PrimaryScreenWidth,
                            (int)SystemParameters.PrimaryScreenHeight);
                    if (Properties.Settings.Default.autoAcceptMatch && !inGame)
                        AutoAcceptMatch();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine($"{ex}");
            }
            GC.Collect();
        }
        private void AutoBuyArmor()
        {
            if (!Properties.Settings.Default.autoBuyArmor || !inGame)
                return;
            int armor = GameState.Player.Armor;
            bool hasHelmet = GameState.Player.HasHelmet;
            int money = GameState.Player.Money;
            if ((matchState == Phase.Live
                && roundState == Phase.Freezetime)
                && 
                ((money >= 650 && armor <= 70)||
                (money >= 350 && armor == 100 && !hasHelmet)||
                (money >= 1000 && armor <= 70 && !hasHelmet))
                )
            {
                DisableConsole();
                Log.WriteLine("Auto buying armor");
                PressKey(Keyboard.DirectXKeyStrokes.DIK_B);
                Thread.Sleep(100);
                PressKeys(new Keyboard.DirectXKeyStrokes[]
                {
                    Keyboard.DirectXKeyStrokes.DIK_5,
                    Keyboard.DirectXKeyStrokes.DIK_1,
                    Keyboard.DirectXKeyStrokes.DIK_2,
                    Keyboard.DirectXKeyStrokes.DIK_B,
                    Keyboard.DirectXKeyStrokes.DIK_B
                });
            }
        }
        private void AutoBuyDefuseKit()
        {
            if (!Properties.Settings.Default.autoBuyDefuseKit || !inGame)
                return;
            bool hasDefuseKit = GameState.Player.HasDefuseKit;
            int money = GameState.Player.Money;
            if (matchState == Phase.Live
                && roundState == Phase.Freezetime
                && money >= 400
                && !hasDefuseKit
                && GameState.Player.Team == Team.CT)
            {
                DisableConsole();
                Log.WriteLine("Auto buying defuse kit");
                PressKey(Keyboard.DirectXKeyStrokes.DIK_B);
                Thread.Sleep(100);
                PressKeys(new Keyboard.DirectXKeyStrokes[]
                {
                    Keyboard.DirectXKeyStrokes.DIK_5,
                    Keyboard.DirectXKeyStrokes.DIK_4,
                    Keyboard.DirectXKeyStrokes.DIK_B,
                    Keyboard.DirectXKeyStrokes.DIK_B
                });
            }
        }
        private void DisableConsole()
        {
            Activity? activity = GameState.Player.CurrentActivity;
            if(activity == Activity.Textinput)
                PressKey(Keyboard.DirectXKeyStrokes.DIK_GRAVE);
        }
        private void TryToAutoReload()
        {
            bool isMousePressed = (Keyboard.GetKeyState(Keyboard.VirtualKeyStates.VK_LBUTTON) & 0x80) != 0;
            if (!isMousePressed || weapon == null)
                return;
            try
            {
                int bullets = weapon.Bullets;
                WeaponType? weaponType = weapon.Type;
                string weaponName = weapon.Name;
                if (bullets == 0)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP,
                        System.Windows.Forms.Cursor.Position.X,
                        System.Windows.Forms.Cursor.Position.Y,
                        0, 0);
                    Log.WriteLine("Auto reloading");
                    if ((weaponType == WeaponType.Rifle
                        || weaponType == WeaponType.MachineGun
                        || weaponType == WeaponType.SubmachineGun
                        || weaponName == "weapon_cz75a")
                        && (weaponName != "weapon_sg556") 
                        && Properties.Settings.Default.ContinueSpraying)
                    {
                        Thread.Sleep(100);
                        mouse_event(MOUSEEVENTF_LEFTDOWN,
                            System.Windows.Forms.Cursor.Position.X,
                            System.Windows.Forms.Cursor.Position.Y,
                            0, 0);
                        Log.WriteLine($"Continue spraying ({weaponName} - {weaponType})");
                    }

                }
            }
            catch { return; }
        }
        void PressKey(Keyboard.DirectXKeyStrokes key)
        {
            Keyboard.SendKey(key, false, Keyboard.InputType.Keyboard);
            Keyboard.SendKey(key, true, Keyboard.InputType.Keyboard);
        }
        void PressKeys(Keyboard.DirectXKeyStrokes[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                Keyboard.SendKey(keys[i], false, Keyboard.InputType.Keyboard);
                Keyboard.SendKey(keys[i], true, Keyboard.InputType.Keyboard);
            }
        }
        
        // from - https://gist.github.com/moritzuehling/7f1c512871e193c0222f
        private string GetCSGODir()
        {
            string csgoDir = Steam.GetGameDir("Counter-Strike Global Offensive");
            if (csgoDir != null)
                return $"{csgoDir}\\csgo";
            return null;
        }
        private void AutoAcceptMatchCheck_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoAcceptMatch = autoAcceptMatchCheck.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void DebugCheck_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.saveDebugFrames = saveFramesDebug.IsChecked;
            Properties.Settings.Default.Save();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        //This is a replacement for Cursor.Position in WinForms
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
            Log.WriteLine($"Left clicked at X:{xpos} Y:{ypos}");
        }
        private void StartUpCheck_Click(object sender, RoutedEventArgs e)
        {
            string appname = Assembly.GetEntryAssembly().GetName().Name;
            string executablePath = Process.GetCurrentProcess().MainModule.FileName;
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if ((bool)startUpCheck.IsChecked)
                {
                    Properties.Settings.Default.runAtStartUp = true;
                    rk.SetValue(appname, executablePath);
                }
                else
                {
                    Properties.Settings.Default.runAtStartUp = false;
                    rk.DeleteValue(appname, false);
                }
            }
            Properties.Settings.Default.Save();
        }
        private void AutoAcceptMatch()
        {
            using (Bitmap bitmap = new Bitmap(1, cs2Resolution.Y))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(
                        cs2Resolution.X / 2,
                        0),
                        Point.Empty,
                        new System.Drawing.Size(1, cs2Resolution.Y/2));
                }
                if (Properties.Settings.Default.saveDebugFrames)
                {
                    Directory.CreateDirectory($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString())}\\DEBUG\\FRAMES");
                    bitmap.Save($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString())}\\DEBUG\\FRAMES\\Frame{frame++}.jpeg", ImageFormat.Jpeg);
                }
                bool found = false;
                for (int y = bitmap.Height - 1; y >= 0 && !found; y--)
                {
                    Color pixelColor = bitmap.GetPixel(0, y);
                    if (pixelColor == BUTTON_COLOR || pixelColor == ACTIVE_BUTTON_COLOR)
                    {
                        var clickpoint = new Point(
                            cs2Resolution.X / 2,
                            y);
                        int X = clickpoint.X;
                        int Y = clickpoint.Y;
                        Log.WriteLine($"Found accept button at X:{X} Y:{Y}");
                        LeftMouseClick(X, Y);
                        found = true;
                    }
                }
            }
        }
        private void Notifyicon_RightMouseButtonClick(object sender, NotifyIconLibrary.Events.MouseLocationEventArgs e)
        {
            exitcm.IsOpen = true;
            Activate();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            this.notifyicon.Close();
            StopGSIServer();
        }
        private bool KillDuplicates()
        {
            bool success = true;
            var currentProcess = Process.GetCurrentProcess();
            var duplicates = Process.GetProcessesByName(currentProcess.ProcessName).Where(o => o.Id != currentProcess.Id);

            if (duplicates.Count() > 0)
            {
                notifyicon.Close();
                App.Current.Shutdown();
                Log.WriteLine($"Shutting down, found another CSAuto process");
            }

            return success;
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            try
            {
                Visibility = Visibility.Hidden;
                this.notifyicon.Icon = Properties.Resources.main;
                this.notifyicon.Tip = "CSAuto - CS2 Automation";
                this.notifyicon.ShowTip = true;
                this.notifyicon.RightMouseButtonClick += Notifyicon_RightMouseButtonClick;
                this.notifyicon.LeftMouseButtonDoubleClick += Notifyicon_LeftMouseButtonDoubleClick;
                this.notifyicon.Update();
                appTimer.Interval = TimeSpan.FromMilliseconds(1000);
                appTimer.Tick += new EventHandler(TimerCallback);
                appTimer.Start();
                Log.WriteLine($"CSAuto v{VER} started");
                string csgoDir = GetCSGODir();
                if (csgoDir == null)
                    throw new DirectoryNotFoundException("Couldn't find CS:GO directory");
                integrationPath = csgoDir + "\\cfg\\gamestate_integration_csauto.cfg";
                if (!File.Exists(integrationPath))
                {
                    using (FileStream fs = File.Create(integrationPath))
                    {
                        Byte[] title = new UTF8Encoding(true).GetBytes(INTEGRATION_FILE);
                        fs.Write(title, 0, title.Length);
                    }
                    Log.WriteLine("CSAuto was never launched, initializing 'gamestate_integration_csauto.cfg'");
                }
                else
                {
                    string[] lines = File.ReadAllLines(integrationPath);
                    string ver = lines[0].Split('v')[1].Split('"')[0].Trim();
                    if (ver != VER)
                    {
                        using (FileStream fs = File.Create(integrationPath))
                        {
                            Byte[] title = new UTF8Encoding(true).GetBytes(INTEGRATION_FILE);
                            fs.Write(title, 0, title.Length);
                        }
                        Log.WriteLine("Different 'gamestate_integration_csauto.cfg' was found, installing correct 'gamestate_integration_csauto.cfg'");
                    }
                }
                try
                {
                    Steam.GetLaunchOptions(730, out string launchOpt);
                    if (launchOpt != null && !HasGSILaunchOption(launchOpt))
                        Steam.SetLaunchOptions(730, launchOpt + " -gamestateintegration");
                    else if (launchOpt == null)
                        Steam.SetLaunchOptions(730, "-gamestateintegration");
                    else
                        Log.WriteLine("Already has \'-gamestateintegration\' in launch options.");
                }
                catch
                {
                    throw new WriteException("Couldn't add -gamestateintegration to launch options\n" +
                    "please refer the the FAQ at the git hub page");
                }
                if (Properties.Settings.Default.autoCheckForUpdates)
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    AutoCheckUpdate();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {
                Type type = ex.GetType();
                if (type == typeof(WriteException) ||
                    type == typeof(DirectoryNotFoundException))
                {
                    autoReloadMenu.IsEnabled = false;
                    autoBuyMenu.IsEnabled = false;
                    autoPauseResumeSpotify.IsEnabled = false;
                }
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        async Task AutoCheckUpdate()
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient() { Encoding = Encoding.UTF8 };
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                Log.WriteLine("Auto Checking for Updates");
                string webInfo = await client.DownloadStringTaskAsync("https://github.com/MurkyYT/CSAuto/releases/latest");
                string latestVersion = webInfo.Split(new string[] { "https://github.com/MurkyYT/CSAuto/releases/tag/" }, StringSplitOptions.None)[1].Split('&')[0].Trim();
                Log.WriteLine($"Auto Check Updates - The latest version is {latestVersion}");
                if (latestVersion == VER)
                {
                    Log.WriteLine("Auto Check Updates - Latest version installed");
                }
                else
                {
                    Log.WriteLine($"Auto Check Updates - Newer version found {VER} --> {latestVersion}");
                    MessageBoxResult result = MessageBox.Show($"Found newer verison ({latestVersion}) would you like to download it?", "Check for updates (CSAuto)", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        Log.WriteLine("Auto Check Updates - Downloading latest version");
                        System.Diagnostics.Process.Start("https://github.com/MurkyYT/CSAuto/releases/latest/download/CSAuto.exe");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Auto Check Updates - Couldn't check for updates - '{ex.Message}'");
            }
        }
        private bool HasGSILaunchOption(string launchOpt)
        {
            string[] split = launchOpt.Split(' ');
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].Trim() == "-gamestateintegration")
                    return true;
            }
            return false;
        }

        private void Notifyicon_LeftMouseButtonDoubleClick(object sender, NotifyIconLibrary.Events.MouseLocationEventArgs e)
        {
            //open debug menu
            if (debugWind == null)
            {
                debugWind = new GSIDebugWindow(this);
                Log.debugWind = debugWind;
                debugWind.Show();
            }
            else
            {
                debugWind.Activate();
            }
        }
    }
}
