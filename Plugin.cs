using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ProjectM;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using Unity.Entities;
using VampireCommandFramework;
using WeaponForge.Configuration;

namespace WeaponAbilityCustomizer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("gg.deca.VampireCommandFramework")]
    [BepInDependency("gg.deca.WeaponForge")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            // Patch using Harmony
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            // Register commands with VCF
            CommandRegistry.RegisterAll(this);
        }

        [Command("ability", "Test command for weapon ability customization")]
        private void OnAbilityCommand(CommandContext context)
        {
            // Your command implementation
            Logger.LogInfo("Ability command executed");
            
            // Example: Respond to the player
            context.Reply("Weapon Ability Customizer is working!");
        }

        // Example Harmony patch for AbilityBar_AssignAbilitySystem
        [HarmonyPatch(typeof(AbilityBar_AssignAbilitySystem), nameof(AbilityBar_AssignAbilitySystem.OnUpdate))]
        private class AbilityBarPatch
        {
            private static bool Prefix(AbilityBar_AssignAbilitySystem __instance)
            {
                // Your patch implementation
                Logger.LogInfo("AbilityBar_AssignAbilitySystem.OnUpdate patched!");
                
                // Return true to allow the original method to run
                // Return false to skip the original method
                return true;
            }
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "WeaponAbilityCustomizer";
        public const string PLUGIN_NAME = "Weapon Ability Customizer";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}

