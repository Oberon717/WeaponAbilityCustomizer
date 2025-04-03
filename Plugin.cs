using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using ProjectM.Shared;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using Unity.Entities;
using VampireCommandFramework;

namespace WeaponAbilityCustomizer
{
    [BepInPlugin("com.yourusername.weaponabilitycustomizer", "Weapon Ability Customizer", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public static ManualLogSource Logger;
        private Harmony _harmony;

        // Store weapon ability mappings
        private static Dictionary<PrefabGUID, List<PrefabGUID>> _customAbilityMappings = new Dictionary<PrefabGUID, List<PrefabGUID>>();

        protected override void Awake()
        {
            Logger = Log;
            _harmony = new Harmony("com.yourusername.weaponabilitycustomizer");
            InitializeAbilityMappings();
            _harmony.PatchAll();
            
            AddCommand();
            
            Logger.LogInfo("Weapon Ability Customizer loaded!");
        }
        
        private void AddCommand() 
        {
            // Register a command to set abilities
            CommandRegistry.RegisterCommand("setweaponability", (context, args) => 
            {
                if (args.Length != 3)
                {
                    return "Usage: .setweaponability <weaponName> <slotIndex> <abilityName>";
                }
                
                string weaponName = args[0];
                int slotIndex;
                string abilityName = args[2];
                
                if (!int.TryParse(args[1], out slotIndex) || slotIndex < 0 || slotIndex > 2)
                {
                    return "Slot index must be a number between 0 and 2";
                }
                
                return SetWeaponAbility(context, weaponName, slotIndex, abilityName);
            });
            
            // Command to list all available weapons
            CommandRegistry.RegisterCommand("listweapons", (context, args) => 
            {
                return "Available weapons: Sword, Axe, Spear, Slashers, Mace, Greatsword";
            });
            
            // Command to list all available abilities
            CommandRegistry.RegisterCommand("listabilities", (context, args) => 
            {
                return "Available abilities: Whirlwind, ChaosVolley, FrostBat, AfterShock, BloodRage, BloodMend, SanguineCoil, CrimsonBeast, Swarm, BloodRite, UnstableStrike, BloodSurge, HeartStrike";
            });
        }
        
        private string SetWeaponAbility(CommandContext context, string weaponName, int slotIndex, string abilityName)
        {
            PrefabGUID weaponGUID;
            PrefabGUID abilityGUID;
            
            if (!TryGetWeaponGUID(weaponName, out weaponGUID))
            {
                return $"Unknown weapon: {weaponName}";
            }
            
            if (!TryGetAbilityGUID(abilityName, out abilityGUID))
            {
                return $"Unknown ability: {abilityName}";
            }
            
            if (!_customAbilityMappings.ContainsKey(weaponGUID))
            {
                _customAbilityMappings[weaponGUID] = new List<PrefabGUID> { abilityGUID };
            }
            else
            {
                while (_customAbilityMappings[weaponGUID].Count <= slotIndex)
                {
                    _customAbilityMappings[weaponGUID].Add(PrefabGUID.Empty);
                }
                _customAbilityMappings[weaponGUID][slotIndex] = abilityGUID;
            }
            
            return $"Set {abilityName} to slot {slotIndex} for {weaponName}";
        }

        private void InitializeAbilityMappings()
        {
            // These are example mappings - you can expand this as needed
            // Adding default mappings for sword: Whirlwind and Chaos Volley
            var swordGUID = new PrefabGUID(1609166128); // Sword GUID
            var whirlwindGUID = new PrefabGUID(1386239222); // Whirlwind ability
            var chaosVolleyGUID = new PrefabGUID(1260420980); // Chaos Volley ability
            
            _customAbilityMappings[swordGUID] = new List<PrefabGUID> { whirlwindGUID, chaosVolleyGUID };
            
            // Adding default mapping for greatsword: Blood Mend and Heart Strike
            var greatswordGUID = new PrefabGUID(863151109); // Greatsword GUID
            var bloodMendGUID = new PrefabGUID(1478572221); // Blood Mend ability
            var heartStrikeGUID = new PrefabGUID(1200448795); // Heart Strike ability
            
            _customAbilityMappings[greatswordGUID] = new List<PrefabGUID> { bloodMendGUID, heartStrikeGUID };
        }
        
        private bool TryGetWeaponGUID(string weaponName, out PrefabGUID guid)
        {
            // Dictionary mapping weapon names to GUIDs
            Dictionary<string, PrefabGUID> weaponNameToGUID = new Dictionary<string, PrefabGUID>(StringComparer.OrdinalIgnoreCase)
            {
                { "Sword", new PrefabGUID(1609166128) },
                { "Axe", new PrefabGUID(2105752108) },
                { "Spear", new PrefabGUID(1473592460) },
                { "Slashers", new PrefabGUID(432881861) },
                { "Mace", new PrefabGUID(1205505492) },
                { "Greatsword", new PrefabGUID(863151109) }
                // Add more weapons as needed
            };
            
            return weaponNameToGUID.TryGetValue(weaponName, out guid);
        }
        
        private bool TryGetAbilityGUID(string abilityName, out PrefabGUID guid)
        {
            // Dictionary mapping ability names to GUIDs
            Dictionary<string, PrefabGUID> abilityNameToGUID = new Dictionary<string, PrefabGUID>(StringComparer.OrdinalIgnoreCase)
            {
                // Standard abilities
                { "Whirlwind", new PrefabGUID(1386239222) },
                { "ChaosVolley", new PrefabGUID(1260420980) },
                { "FrostBat", new PrefabGUID(1075930716) },
                { "AfterShock", new PrefabGUID(613054282) },
                { "BloodRage", new PrefabGUID(1087307620) },
                
                // Dracula's abilities
                { "BloodMend", new PrefabGUID(1478572221) },     // Blood Mend - Healing ability
                { "SanguineCoil", new PrefabGUID(1667386352) },  // Sanguine Coil - Blood tendril ability
                { "CrimsonBeast", new PrefabGUID(1943442886) },  // Crimson Beast - Transformation ability
                { "Swarm", new PrefabGUID(1570244417) },         // Bats Swarm ability
                { "BloodRite", new PrefabGUID(1739761448) },     // Blood Rite - AoE blood explosion
                { "UnstableStrike", new PrefabGUID(1568023593) },// Unstable Strike - Powerful melee strike
                { "BloodSurge", new PrefabGUID(1298399199) },    // Blood Surge - Blood dash attack
                { "HeartStrike", new PrefabGUID(1200448795) },   // Heart Strike - Powerful single-target attack
                
                // Add more abilities as needed
            };
            
            return abilityNameToGUID.TryGetValue(abilityName, out guid);
        }
    }

    [HarmonyPatch]
    public static class AbilityPatches
    {
        [HarmonyPatch(typeof(AbilityBar_AssignAbilitySystem), "OnUpdate")]
        [HarmonyPostfix]
        public static void OnAbilityBarUpdate_Postfix(AbilityBar_AssignAbilitySystem __instance)
        {
            try
            {
                EntityManager entityManager = __instance.EntityManager;
                
                // Get all players
                var query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<PlayerCharacter>(),
                    ComponentType.ReadOnly<EquipmentData>());
                
                var playerEntities = query.ToEntityArray(Unity.Collections.Allocator.Temp);
                
                foreach (var playerEntity in playerEntities)
                {
                    ProcessPlayerWeaponAbilities(entityManager, playerEntity);
                }
                
                playerEntities.Dispose();
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error in ability patch: {ex.Message}");
            }
        }
        
        private static void ProcessPlayerWeaponAbilities(EntityManager entityManager, Entity playerEntity)
        {
            if (!entityManager.HasComponent<EquipmentData>(playerEntity))
                return;
                
            var equipmentData = entityManager.GetComponentData<EquipmentData>(playerEntity);
            var weaponEntity = equipmentData.WeaponSlot._Entity;
            
            if (weaponEntity == Entity.Null || !entityManager.HasComponent<PrefabGUID>(weaponEntity))
                return;
                
            var weaponGUID = entityManager.GetComponentData<PrefabGUID>(weaponEntity);
            
            // If we have custom mappings for this weapon, apply them
            if (Plugin._customAbilityMappings.TryGetValue(weaponGUID, out var customAbilities))
            {
                // Get the ability bar component
                if (entityManager.HasComponent<AbilityBar>(playerEntity))
                {
                    var abilityBar = entityManager.GetComponentData<AbilityBar>(playerEntity);
                    
                    // Apply custom abilities to slots
                    for (int i = 0; i < customAbilities.Count && i < 2; i++) // Only weapon abilities (usually 2)
                    {
                        // Only replace if we have a valid ability
                        if (customAbilities[i] != PrefabGUID.Empty)
                        {
                            // Logic to assign the ability based on slot index
                            // Ability slots 0 and 1 are typically weapon abilities
                            abilityBar.Abilities[i] = customAbilities[i];
                        }
                    }
                    
                    // Update the ability bar component
                    entityManager.SetComponentData(playerEntity, abilityBar);
                    
                    // Log that we applied custom abilities
                    Plugin.Logger.LogInfo($"Applied custom abilities to player for weapon {weaponGUID}");
                }
            }
        }
    }
}
