using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BepInPluginSample
{
    [BepInPlugin("Game.Lilly.Plugin", "Lilly", "1.0")]
    public class Sample : BaseUnityPlugin
    {
        #region GUI
        public static ManualLogSource logger;

        static Harmony harmony;

        public ConfigEntry<BepInEx.Configuration.KeyboardShortcut> isGUIOnKey;
        public ConfigEntry<BepInEx.Configuration.KeyboardShortcut> isOpenKey;

        private ConfigEntry<bool> isGUIOn;
        private ConfigEntry<bool> isOpen;
        private ConfigEntry<float> uiW;
        private ConfigEntry<float> uiH;

        public int windowId = 542;
        public Rect windowRect;

        public string title = "";
        public string windowName = ""; // 변수용 
        public string FullName = "Plugin"; // 창 펼쳤을때
        public string ShortName = "P"; // 접었을때

        GUILayoutOption h;
        GUILayoutOption w;
        public Vector2 scrollPosition;
        #endregion

        #region 변수
        // =========================================================

        private static ConfigEntry<bool> destroyPlayer;
        private static ConfigEntry<bool> gameOver;
        private static ConfigEntry<bool> playerHealtHit;
        private static ConfigEntry<int> ammoMulti;
        // private static ConfigEntry<float> xpMulti;

        // =========================================================
        #endregion

        public void Awake()
        {
            #region GUI
            logger = Logger;
            Logger.LogMessage("Awake");

            isGUIOnKey = Config.Bind("GUI", "isGUIOnKey", new KeyboardShortcut(KeyCode.Keypad0));// 이건 단축키
            isOpenKey = Config.Bind("GUI", "isOpenKey", new KeyboardShortcut(KeyCode.KeypadPeriod));// 이건 단축키

            isGUIOn = Config.Bind("GUI", "isGUIOn", true);
            isOpen = Config.Bind("GUI", "isOpen", true);
            isOpen.SettingChanged += IsOpen_SettingChanged;
            uiW = Config.Bind("GUI", "uiW", 300f);
            uiH = Config.Bind("GUI", "uiH", 600f);

            if (isOpen.Value)
                windowRect = new Rect(Screen.width - 65, 0, uiW.Value, 800);
            else
                windowRect = new Rect(Screen.width - uiW.Value, 0, uiW.Value, 800);

            IsOpen_SettingChanged(null, null);
            #endregion

            #region 변수 설정
            // =========================================================

            destroyPlayer = Config.Bind("game", "destroyPlayer", false);
            gameOver = Config.Bind("game", "gameOver", false);
            playerHealtHit = Config.Bind("game", "PlayerHealtHit", false);
            ammoMulti = Config.Bind("game", "ammoMulti", 10);

            // =========================================================
            #endregion
        }

        #region GUI
        public void IsOpen_SettingChanged(object sender, EventArgs e)
        {
            logger.LogInfo($"IsOpen_SettingChanged {isOpen.Value} , {isGUIOn.Value},{windowRect.x} ");
            if (isOpen.Value)
            {
                title = isGUIOnKey.Value.ToString() + "," + isOpenKey.Value.ToString();
                h = GUILayout.Height(uiH.Value);
                w = GUILayout.Width(uiW.Value);
                windowName = FullName;
                windowRect.x -= (uiW.Value - 64);
            }
            else
            {
                title = "";
                h = GUILayout.Height(40);
                w = GUILayout.Width(60);
                windowName = ShortName;
                windowRect.x += (uiW.Value - 64);
            }
        }
        #endregion

        public void OnEnable()
        {
            Logger.LogWarning("OnEnable");
            // 하모니 패치
            try // 가급적 try 처리 해주기. 하모니 패치중에 오류나면 다른 플러그인까지 영향 미침
            {
                harmony = Harmony.CreateAndPatchAll(typeof(Sample));
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        public void Update()
        {
            #region GUI
            if (isGUIOnKey.Value.IsUp())// 단축키가 일치할때
            {
                isGUIOn.Value = !isGUIOn.Value;
            }
            if (isOpenKey.Value.IsUp())// 단축키가 일치할때
            {
                if (isGUIOn.Value)
                {
                    isOpen.Value = !isOpen.Value;
                }
                else
                {
                    isGUIOn.Value = true;
                    isOpen.Value = true;
                }
            }
            #endregion
        }

        #region GUI
        public void OnGUI()
        {
            if (!isGUIOn.Value)
                return;

            // 창 나가는거 방지
            windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + 4, Screen.width - 4);
            windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + 4, Screen.height - 4);
            windowRect = GUILayout.Window(windowId, windowRect, WindowFunction, windowName, w, h);
        }
        #endregion

        public virtual void WindowFunction(int id)
        {
            #region GUI
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();// 가로 정렬
                                        // 라벨 추가
                                        //GUILayout.Label(windowName, GUILayout.Height(20));
                                        // 안쓰는 공간이 생기더라도 다른 기능으로 꽉 채우지 않고 빈공간 만들기
            if (isOpen.Value) GUILayout.Label(title);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { isOpen.Value = !isOpen.Value; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { isGUIOn.Value = false; }
            GUI.changed = false;

            GUILayout.EndHorizontal();// 가로 정렬 끝

            if (!isOpen.Value) // 닫혔을때
            {
            }
            else // 열렸을때
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
                #endregion

                #region 여기에 GUI 항목 작성
                // =========================================================

                if (GUILayout.Button($"destroyPlayer {destroyPlayer.Value}")) { destroyPlayer.Value = !destroyPlayer.Value; }
                if (GUILayout.Button($"gameOver {gameOver.Value}")) { gameOver.Value = !gameOver.Value; }
                
                if (GUILayout.Button($"playerHealtHit {playerHealtHit.Value}")) { playerHealtHit.Value = !playerHealtHit.Value; }

                GUILayout.Label("--- ---");

                GUILayout.BeginHorizontal();
                GUILayout.Label($"ammoMulti {ammoMulti.Value}");
                if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20))) { ammoMulti.Value += 1; }
                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { ammoMulti.Value -= 1; }
                GUILayout.EndHorizontal();

                if (playerWeaponControllers != null)
                {
                    foreach (PlayerWeaponController item in playerWeaponControllers)
                    {
                        if (item!=null)
                        {                           
                        GUILayout.Label($"{item.name} , {item.currentAmmo} , {item.maxAmmo} , {item.fireRate} ");
                        if (GUILayout.Button("+10000")) { item.currentAmmo += 10000; }
                        }
                        else
                        {
                            GUILayout.Label("PlayerWeaponController null");
                        }
                        //GUILayout.Label($"{item.weaponSlider.name} , {item.weaponSlider.value} , {item.weaponSlider.minValue} , {item.weaponSlider.maxValue}");
                    }
                }
                else
                {
                    GUILayout.Label("playerWeaponControllers null");
                }

                if (playerLaserScript != null)
                {
                    GUILayout.Label($"{playerLaserScript.name} , {playerLaserScript.currentAmmo} , {playerLaserScript.maxAmmo} , {playerLaserScript.fireRate} ");
                    if (GUILayout.Button("+10000")) { playerLaserScript.currentAmmo += 10000; }
                }
                else
                {
                    GUILayout.Label("playerLaserScript null");
                }
                
                if (shieldActivatorScript != null)
                {
                    GUILayout.Label($"{shieldActivatorScript.name} , {shieldActivatorScript.currentAmmo} , {shieldActivatorScript.maxAmmo} , {shieldActivatorScript.fireRate}  , {shieldActivatorScript.timeOfShield} ");
                    if (GUILayout.Button("+10000")) { shieldActivatorScript.currentAmmo += 10000; }                                                           
                    if (GUILayout.Button($"timeOfShield set 1000")) { shieldActivatorScript.timeOfShield = 1000; }
                    if (GUILayout.Button($"timeOfShield set 2")) { shieldActivatorScript.timeOfShield = 2; }                    
                }
                else
                {
                    GUILayout.Label("shieldActivatorScript null");
                }

                GUILayout.Label("--- ---");

                // =========================================================
                #endregion

                #region GUI
                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
            #endregion
        }


        public void OnDisable()
        {
            Logger.LogWarning("OnDisable");
            harmony?.UnpatchSelf();
        }

        #region Harmony
        // ====================== 하모니 패치 샘플 ===================================

        [HarmonyPatch(typeof(PlayerHealth), "Hit")]
        [HarmonyPrefix]
        public static void HitPre(PlayerHealth __instance, ref float damage)
        {
            if (damage>100f)
            {
                logger.LogWarning($"PlayerHealth.Hit {damage}");
            }
            if (!playerHealtHit.Value)
            {
                if (damage > 0)
                {
                    damage = 0;
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerWeaponController), "PowerUp")]
        [HarmonyPrefix]
        public static void PowerUp(PlayerWeaponController __instance, ref int addBullets)
        {
            logger.LogWarning($"PlayerWeaponController.PowerUp {addBullets}");

            __instance.currentAmmo=__instance.maxAmmo;
            //addBullets *= ammoMulti.Value;
        }

        static List<PlayerWeaponController> playerWeaponControllers = new List<PlayerWeaponController>();
        static PlayerLaserScript playerLaserScript=null;
        static ShieldActivatorScript shieldActivatorScript =null;

        private static void SetClear()
        {
            playerWeaponControllers.Clear();
            playerLaserScript = null;
            shieldActivatorScript = null;
        }

        [HarmonyPatch(typeof(Done_GameController), "Awake")]
        [HarmonyPostfix]
        public static void AwakePost(Done_GameController __instance)
        {
            logger.LogWarning($"Done_GameController.Awake");
            SetClear();
        }
        
        [HarmonyPatch(typeof(Done_GameController), "BossIsDead")]
        [HarmonyPostfix]
        public static void BossIsDead(Done_GameController __instance)
        {
            logger.LogWarning($"Done_GameController.BossIsDead");
            SetClear();
        }      

        [HarmonyPatch(typeof(Done_GameController), "GameOver")]
        [HarmonyPrefix]
        public static bool GameOver(Done_GameController __instance)
        {
            logger.LogWarning($"Done_GameController.GameOver");
            if (gameOver.Value)
            {
                SetClear();
                return true;
            }
            return false;
        }

        [HarmonyPatch(typeof(PlayerWeaponController), "Start")]
        [HarmonyPostfix]
        public static void StartPost(PlayerWeaponController __instance)
        {
            playerWeaponControllers.Add(__instance);

            __instance.maxAmmo *= ammoMulti.Value;
            __instance.currentAmmo = __instance.maxAmmo;
            logger.LogWarning($"PlayerWeaponController.Start {__instance.currentAmmo} , {__instance.maxAmmo}");
        }
        
        [HarmonyPatch(typeof(PlayerLaserScript), "Start")]
        [HarmonyPostfix]
        public static void StartPost(PlayerLaserScript __instance)
        {
            playerLaserScript=__instance;

            __instance.maxAmmo *= ammoMulti.Value;
            __instance.currentAmmo = __instance.maxAmmo;
            logger.LogWarning($"PlayerWeaponController.Start {__instance.currentAmmo} , {__instance.maxAmmo}");
        }
                
        [HarmonyPatch(typeof(ShieldActivatorScript), "Start")]
        [HarmonyPostfix]
        public static void StartPost(ShieldActivatorScript __instance)
        {
            shieldActivatorScript = __instance;

            __instance.maxAmmo *= ammoMulti.Value;
            __instance.currentAmmo = __instance.maxAmmo;
            logger.LogWarning($"PlayerWeaponController.Start {__instance.currentAmmo} , {__instance.maxAmmo}");
            logger.LogWarning($"PlayerWeaponController.Start {__instance.fireRate} , {__instance.timeOfShield}");
        }


        [HarmonyPatch(typeof(Done_GameController), "Start")]
        [HarmonyPostfix]
        public static void StartPost(Done_GameController __instance)
        {
            logger.LogWarning($"Done_GameController.Start");
        }

        [HarmonyPatch(typeof(Done_PlayerController), "Start")]
        [HarmonyPostfix]
        public static void StartPost(Done_PlayerController __instance)
        {
            logger.LogWarning($"Done_PlayerController.Start");
        }
        
        [HarmonyPatch(typeof(DestroyPlayer), "OnTriggerEnter")]
        [HarmonyPrefix]
        public static bool OnTriggerEnter(DestroyPlayer __instance, Collider other)
        {
            logger.LogWarning($"DestroyPlayer.OnTriggerEnter {other.tag}");
            if (!destroyPlayer.Value)
            {
                if ( other.tag == "Player")
                {
                    return false;
                }
            }
            return true;
        }        
        [HarmonyPatch(typeof(CheckPlayersShield), "OnTriggerEnter")]
        [HarmonyPrefix]
        public static bool OnTriggerEnter(CheckPlayersShield __instance, Collider other)
        {
            logger.LogWarning($"CheckPlayersShield.OnTriggerEnter {other.tag} , {other.gameObject.tag}");
            if (!destroyPlayer.Value)
            {
                if (other.gameObject.tag == "ShieldOfPlayer")
                    {
                    return false;
                }
            }
            return true;
        }
                /*
        [HarmonyPatch(typeof(DestroyPlayer), "Start")]
        [HarmonyPrefix]
        public static bool Start(DestroyPlayer __instance)
        {
            logger.LogWarning($"DestroyPlayer.Start");
            if (!destroyPlayer.Value)
            {
                return false;
            }
            return true;
        }
        */

        /*
        [HarmonyPatch(typeof(AEnemy), "DamageMult", MethodType.Setter)]
        [HarmonyPrefix]
        public static void SetDamageMult(ref float __0)
        {
            if (!eMultOn.Value)
            {
                return;
            }
            __0 *= eDamageMult.Value;
        }
        */
        // =========================================================
        #endregion
    }
}
