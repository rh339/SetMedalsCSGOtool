using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamKit2;
using System.IO;
using Newtonsoft.Json;
using SteamAuth;
using SteamKit2.Internal;
using System.Threading;
using System.Security.Cryptography;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
namespace WindowsFormsApplication2
{
    class LastMatchInfo
    {
        public uint lastCheck { get; set; }
        public uint matchEnded { get; set; }
    }
    public enum RankName
    {
        NotRanked = 0,
        SilverI = 1,
        SilverII = 2,
        SilverIII = 3,
        SilverIV = 4,
        SilverElite = 5,
        SilverEliteMaster = 6,
        GoldNovaI = 7,
        GoldNovaII = 8,
        GoldNovaIII = 9,
        GoldNovaMaster = 10,
        MasterGuardianI = 11,
        MasterGuardianII = 12,
        MasterGuardianElite = 13,
        DistinguishedMasterGuardian = 14,
        LegendaryEagle = 15,
        LegendaryEagleMaster = 16,
        SupremeMasterFirstClass = 17,
        TheGlobalElite = 18
    }
    class Otlegacheck
    {
        public class MyCallback : CallbackMsg
        {
            public EResult Result { get; private set; }

            internal MyCallback(EResult res)
            {
                Result = res;
            }
        }
        static CallbackManager CallbackManager;
        static SteamClient SteamClient;
        static SteamUser SteamUser;
        static SteamFriends SteamFriends;
        static SteamGameCoordinator SteamGameCoordinator;
        private static bool isRunning;
        public static string TwoFactorCode, AuthCode;
        static string SentryFileName, LoginKeyFileName;
        static uint SteamAccountID = 0;
        static int startedAt = 0;
        static ulong matchID = 0;
        static RankName rankID = 0;
        static string action;
        static public string informationsteam;
        static bool Authcodelegit = false;
        static public bool NeedSteamGuardKey = false;
        static public bool banned = false;
        static private string steamlogin;
        static private string steampassword;
        
        public static bool IsRunning
        {
            get
            {
                return isRunning;
            }

            set
            {
                isRunning = value;
            }
        }
        static public string accountIDFormat(uint accountID)
        {
            var id = new SteamID();
            id.AccountID = accountID;
            return id.ToString();
        }
        static public void InitSteamkit()
        {
            SteamDirectory.Initialize();

            SteamClient = new SteamClient();

            CallbackManager = new CallbackManager(SteamClient);

            CallbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

            SteamUser = SteamClient.GetHandler<SteamUser>();
            CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            CallbackManager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
            CallbackManager.Subscribe<SteamUser.LoginKeyCallback>(OnLoginKey);
            CallbackManager.Subscribe<SteamUser.UpdateMachineAuthCallback>(OnMachineAuth);

            SteamFriends = SteamClient.GetHandler<SteamFriends>();

            SteamGameCoordinator = SteamClient.GetHandler<SteamGameCoordinator>();
        }
        static public void OnConnected(SteamClient.ConnectedCallback callback)
        {
            if (callback == null)
            {
                return;
            }
            informationsteam = (String.Format("Connected to Steam!"));

            byte[] sentryHash = null;

            if (File.Exists(SentryFileName))
            {
                byte[] sentryFileContent = File.ReadAllBytes(SentryFileName);
                sentryHash = CryptoHelper.SHAHash(sentryFileContent);
            }

            string loginKey = null;

            if (File.Exists(LoginKeyFileName))
            {
                loginKey = File.ReadAllText(LoginKeyFileName);
            }


            informationsteam = (String.Format("Logging in..."));
            startedAt = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            SteamUser.LogOn(new SteamUser.LogOnDetails
            {
                Username = steamlogin,
                Password = loginKey == null ? steampassword : null,
                AuthCode = loginKey == null ? AuthCode : null,
                LoginID = MsgClientLogon.ObfuscationMask,
                LoginKey = loginKey,
                TwoFactorCode = loginKey == null ? TwoFactorCode : null,
                SentryFileHash = sentryHash,
                ShouldRememberPassword = true
            });

        }

        static public void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            if (callback == null)
            {
                return;
            }

            informationsteam = (String.Format("Disconnected! Reconnecting..."));

            Thread.Sleep(TimeSpan.FromSeconds(5));

            SteamClient.Connect();
        }
        static public async void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback == null)
            {
                return;
            }
            TwoFactorCode = null;
            AuthCode = null;
            Authcodelegit = false;
            TwoFactorCode = null;

            switch (callback.Result)
            {
                case EResult.TwoFactorCodeMismatch:
                case EResult.AccountLoginDeniedNeedTwoFactor:
                    TwoFactorCode = null;
                    Authcodelegit = false;
                    while (Authcodelegit == false)
                    {
                        NeedSteamGuardKey = true;
                        informationsteam = (String.Format("Put in Guard code from mobile phone: ", callback.EmailDomain));
                        if (Form1.SteamAuthCode != null)
                        {
                            TwoFactorCode = Form1.SteamAuthCode;
                            Authcodelegit = true;
                            NeedSteamGuardKey = false;
                        }
                    }
                    return;
                case EResult.AccountLogonDenied:
                    AuthCode = null;
                    Authcodelegit = false;
                    while (Authcodelegit == false)
                    {
                        NeedSteamGuardKey = true;
                        informationsteam = (String.Format("Put in Guard code which u resived on Email {0}: ", callback.EmailDomain));
                        if (Form1.SteamAuthCode != null)
                        {
                            AuthCode = Form1.SteamAuthCode;
                            Authcodelegit = true;
                            NeedSteamGuardKey = false;
                        }
                    }
                    return;

                case EResult.InvalidPassword:
                    informationsteam = (String.Format("Unable to login to Steam: " + callback.Result));
                    if (!File.Exists(SentryFileName) && !File.Exists(LoginKeyFileName))
                    {
                        informationsteam = "Wrong password";
                        errorlog(steamlogin + ":Wrong password");
                    }
                    if (File.Exists(SentryFileName))
                    {
                        informationsteam = (String.Format("Deleting old key..."));
                        File.Delete(SentryFileName);
                    }

                    if (File.Exists(LoginKeyFileName))
                    {
                        informationsteam = (String.Format("Deleting old key..."));
                        File.Delete(LoginKeyFileName);
                    }
                    return;
                case EResult.OK:
                    informationsteam = (String.Format("Connected sucefully!"));
                    break;
                default:
                    informationsteam = (String.Format("Cant connect to steam: {0} / {1}", callback.Result, callback.ExtendedResult));
                    errorlog(steamlogin + String.Format(":Cant connect to steam: {0} / {1}", callback.Result, callback.ExtendedResult));
                    return;
            }
            informationsteam = (String.Format("Setting Medals..."));
            var kickSession = new ClientMsgProtobuf<CMsgClientKickPlayingSession>(EMsg.ClientKickPlayingSession);
            kickSession.Body.only_stop_game = false;
            SteamClient.Send(kickSession);
            var ClientToGC = new ClientGCMsgProtobuf<PlayerMedalsInfo>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_SetMyMedalsInfo);
            ClientToGC.Body.medal_global = 3;
            ClientToGC.Body.medal_arms = 3;
            ClientToGC.Body.medal_combat = 3;
            ClientToGC.Body.medal_weapon = 3;
            ClientToGC.Body.medal_team = 3;
            ClientToGC.Body.featured_display_item_defidx = 941;
            string[] medals = null;
            try
            {
                medals = File.ReadAllLines("medals.txt");
            }
            catch(Exception)
            {
                File.WriteAllText("medals.txt", "941");
                medals = File.ReadAllLines("medals.txt");
            }
            for (int i = 0; i < medals.Count(); i++)
            {
                ClientToGC.Body.display_items_defidx.Add(Convert.ToUInt16(medals[i]));
            }
            SteamGameCoordinator.Send(ClientToGC, 730);
            if (File.Exists(LoginKeyFileName))
            {
                informationsteam = ("Medals Set!");
                SteamClient.Disconnect();
                IsRunning = false;
            }            
        }
        static public void errorlog(string text)
        {
            try
            {
                string errorlog = File.ReadAllText("error.txt");
                errorlog = errorlog + String.Format(text) + Environment.NewLine;
                File.WriteAllText("error.txt", errorlog);
            }
            catch (Exception)
            {
                File.WriteAllText("error.txt", String.Format(text) + Environment.NewLine);
            }
        }
        static void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            if (callback == null)
            {
                return;
            }

            informationsteam = (String.Format("Disconnect from steam: {0}", callback.Result));
            if (callback.Result == EResult.LoggedInElsewhere)
            {
                IsRunning = false;
            }
        }


        static public void OnLoginKey(SteamUser.LoginKeyCallback callback)
        {
            if (callback == null)
            {
                return;
            }

            informationsteam = (String.Format("Updating key..."));
            File.WriteAllText(LoginKeyFileName, callback.LoginKey);

            SteamUser.AcceptNewLoginKey(callback);
            informationsteam = (String.Format("Updating key...Done!"));
            informationsteam = ("Medals Set!");
            SteamClient.Disconnect();
            IsRunning = false;
        }
        static void OnMachineAuth(SteamUser.UpdateMachineAuthCallback callback)
        {
            if (callback == null)
            {
                return;
            }

            informationsteam = (String.Format("Updating key..."));

            int fileSize;
            byte[] sentryHash;
            using (var fs = File.Open(SentryFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                fs.Seek(callback.Offset, SeekOrigin.Begin);
                fs.Write(callback.Data, 0, callback.BytesToWrite);
                fileSize = (int)fs.Length;

                fs.Seek(0, SeekOrigin.Begin);
                using (var sha = new SHA1CryptoServiceProvider())
                {
                    sentryHash = sha.ComputeHash(fs);
                }
            }
            SteamUser.SendMachineAuthResponse(new SteamUser.MachineAuthDetails
            {
                JobID = callback.JobID,

                FileName = callback.FileName,

                BytesWritten = callback.BytesToWrite,
                FileSize = fileSize,
                Offset = callback.Offset,

                Result = EResult.OK,
                LastError = 0,

                OneTimePassword = callback.OneTimePassword,

                SentryFileHash = sentryHash,
            });

            informationsteam = (String.Format("Done!"));
        }
        static public void SetMyMedals()
        {
            SteamAccountID = 0;
            steamlogin = File.ReadAllText("data/login.txt");
            steampassword = File.ReadAllText("data/password.txt");
            InitSteamkit();
            Authcodelegit = false;

            LoginKeyFileName = "data/" + steamlogin + ".key";
            SentryFileName = "data/" + steamlogin + ".sentry";
            //SteamClient.DebugNetworkListener = new NetHookNetworkListener( "debug/" );
            informationsteam = "Connecting to Steam...";

            SteamClient.Connect();
            IsRunning = true;
            while (IsRunning)
            {
                CallbackManager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }
        static public void Exit()
        {
            Thread.Sleep(500);
            Environment.Exit(0);
        }
    }
}
