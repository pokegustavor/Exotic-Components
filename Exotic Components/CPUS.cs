using HarmonyLib;
using PulsarModLoader.Content.Components.CPU;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Reflection.Emit;
using PulsarModLoader.Patches;
using static PulsarModLoader.Patches.HarmonyHelpers;
namespace Exotic_Components
{
    public class CPUS
    {
        public class The_Premonition : CPUMod
        {
            public static int lastLive = 0;

            public static float lastHull = 5000;

            public static List<PLPersistantShipInfo> others = new List<PLPersistantShipInfo>();

            public static Vector3[] crewPosition = new Vector3[5] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

            public override string Name => "The Premonition";

            public override string Description => "A special CPU that calculates millions of timelines and will bring you back when you die in a timeline. It was marked as contraband because it is \"illegal to mess with the nature of time\".";

            public override int MarketPrice => 50000;

            public override bool CanBeDroppedOnShipDeath => false;

            public override bool Contraband => true;

            public override Texture2D IconTexture => (Texture2D)Resources.Load("Icons/51_Processer");

            public override float MaxPowerUsage_Watts => 7500f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Charge:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                if (PLServer.GetCurrentSector().VisualIndication == ESectorVisualIndication.LCWBATTLE || PLServer.GetCurrentSector().VisualIndication == ESectorVisualIndication.TOPSEC || PLServer.GetCurrentSector().VisualIndication == ESectorVisualIndication.UNSEEN_MS || PLServer.GetCurrentSector().Name == "Gauntlet_Arena()")
                {
                    lastLive = 1;
                    return "Unkown Error!";
                }
                if (lastLive >= 20) return "charged";
                return ((20 - lastLive) / 2) + " jumps remains";
            }
        }
        public class ThermoBoost : CPUMod
        {

            public static float GetMaxHeat(PLShipInfoBase ship)
            {
                float maxHeat = 1.1f;
                foreach (PLCPU cpu in ship.MyStats.GetComponentsOfType(ESlotType.E_COMP_CPU).Cast<PLCPU>())
                {
                    if (cpu.SubType == CPUModManager.Instance.GetCPUIDFromName("Turret Thermo Boost"))
                    {
                        maxHeat += 0.3f * cpu.LevelMultiplier(0.31f, 1);
                    }
                }
                return maxHeat;
            }

            public override string Name => "Turret Thermo Boost";

            public override string Description => "This processor adds extra heat limit for all turrets in the ship. That is what I call efficient cooling!";

            public override int MarketPrice => 20000;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 1f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Turret extra Heat:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                return (0.3f * InComp.LevelMultiplier(0.31f, 1) * 100) + " %";
            }
        }
        public class Researcher : CPUMod
        {
            public override string Name => "Research Processor";

            public override string Description => "A processor that creates a random research material every jump to an unvisited sector. Again, don't ask how it creates chemicals from nothing.";

            public override int MarketPrice => 25000;

            public override float MaxPowerUsage_Watts => 1f;

            public override void OnWarp(PLShipComponent InComp)
            {
                int targetId = PLEncounterManager.Instance.PlayerShip.WarpTargetID;
                if (targetId == -1 || !InComp.IsEquipped) return;
                if (!PLGlobal.Instance.Galaxy.AllSectorInfos[targetId].Visited && PhotonNetwork.isMasterClient)
                {
                    int researchID = Random.Range(0, 5);
                    PLServer.Instance.photonView.RPC("AddNotification_OneString_LocalizedString", PhotonTargets.All, new object[]
                                {
                                        "+1 [STR0] added due to the Research Processor!",
                                        -1,
                                        PLServer.Instance.GetEstimatedServerMs() + 3000,
                                        true,
                                        PLPawnItem_ResearchMaterial.GetResearchTypeString(researchID)
                                });
                    PLServer.Instance.ResearchMaterials[researchID]++;
                }
            }

        }
        public class CreditLaundering : CPUMod
        {
            public override string Name => "Credits Processor";

            public override string Description => "A processor that gives credits when going to unvisited sectors. Don't worry about how or from where the credits comes from, also if the Colonial Union asks, I didn't give this to you.";

            public override int MarketPrice => 40000;

            public override bool Contraband => true;

            public override float MaxPowerUsage_Watts => 1f;

            public override void OnWarp(PLShipComponent InComp)
            {
                int targetId = PLEncounterManager.Instance.PlayerShip.WarpTargetID;
                if (targetId == -1 || !InComp.IsEquipped) return;
                if (!PLGlobal.Instance.Galaxy.AllSectorInfos[targetId].Visited && PhotonNetwork.isMasterClient)
                {
                    PLServer.Instance.photonView.RPC("AddNotification_OneString_LocalizedString", PhotonTargets.All, new object[]
                                {
                                        "[STR0]Cr added due to the Credits Processor!",
                                        -1,
                                        PLServer.Instance.GetEstimatedServerMs() + 3000,
                                        true,
                                        (10000 + (int)(10000 * InComp.LevelMultiplier(0.61f, 1))).ToString()
                                });
                    PLServer.Instance.CurrentCrewCredits += 10000 + (int)(10000 * InComp.LevelMultiplier(0.61f, 1));
                }
            }
            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Credits per jump:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                return 10000 + (int)(10000 * InComp.LevelMultiplier(0.61f, 1)) + " Cr";
            }
        }
        public class TripleCombo : CPUMod
        {
            public override string Name => "Triple Combo Processor";

            public override string Description => "This processor adds cyberdefence, jump calculation and shield charge boost, while not having an absurd energy consumption!";

            public override int MarketPrice => 20000;

            public override float Defense => 0.1f;

            public override float Speed => 1.2f;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 6900f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Jump Processor: \nCyber Defense: \nShield Charge Boost:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                return "+" + (0.7f * me.LevelMultiplier(0.5f, 1f) * 10f).ToString("0") + "\n"
                    + (0.36f * me.LevelMultiplier(0.75f, 1f)).ToString("0.0") + "\n"
                    + (1.7f * me.LevelMultiplier(0.25f, 1f)).ToString("0");
            }

            public override void AddStats(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                ObscuredFloat[] computingPower3 = me.ShipStats.ComputingPower;
                int num = 0;
                computingPower3[num] += 0.70f * me.LevelMultiplier(0.5f, 1f) * me.GetPowerPercentInput();
                me.ShipStats.CyberDefenseRating += 0.36f * me.LevelMultiplier(0.75f, 1f) * me.GetPowerPercentInput();
            }

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                PLShipStats inStats = InComp.ShipStats;
                PLCPU me = InComp as PLCPU;
                if (me.IsEquipped)
                {
                    inStats.ShieldsChargeRate += me.GetPowerPercentInput() * 1.7f * me.LevelMultiplier(0.25f, 1f);
                    inStats.ShieldsChargeRateMax += me.GetPowerPercentInput() * 1.7f * me.LevelMultiplier(0.25f, 1f);
                }
            }

            public override void Tick(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                me.m_MaxPowerUsage_Watts = 6900f * me.LevelMultiplier(0.12f, 1f);
                if (me.IsEquipped)
                {
                    me.IsPowerActive = true;
                    if (me.ShipStats == null || me.ShipStats.Ship == null || (me.ShipStats.Ship.WarpChargeStage == EWarpChargeStage.E_WCS_PREPPING && me.ShipStats.Ship.WarpChargeState_Levels[0] < 1f))
                    {
                        me.m_RequestPowerUsage_Percent = 1f;
                    }
                    else
                    {
                        me.m_RequestPowerUsage_Percent = 0.5f;
                    }
                }
            }
        }
        public class ActiveAntiVirus : CPUMod
        {
            static float lastRemoval = Time.time;
            public override string Name => "Active Anti-Virus";

            public override string Description => "This processor will remove a random virus every few seconds if given power, less power to it means slower virus removal, at least you will be protected. Also your virus definitions were updated.";

            public override int MarketPrice => 15000;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 6000f;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Virus removal delay: ";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                if (me.IsEquipped)
                {
                    float timer = (30f - 1 * me.LevelMultiplier(2, 1)) / me.GetPowerPercentInput();
                    if (timer > 120) timer = 120f;
                    if (timer < 10) timer = 10f;
                    return ((int)timer).ToString() + "s";
                }
                else
                {
                    int timer = Mathf.FloorToInt(30f - 1 * me.LevelMultiplier(2, 1));
                    if (timer < 10) timer = 10;
                    return ((int)timer).ToString() + "s";
                }
            }

            public override void Tick(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                if (me.IsEquipped)
                {
                    float timerequired = (30f - 1 * me.LevelMultiplier(2, 1)) / me.GetPowerPercentInput();
                    if (timerequired > 120) timerequired = 120f;
                    if (timerequired < 10) timerequired = 10f;
                    me.IsPowerActive = true;
                    me.m_RequestPowerUsage_Percent = 1f;
                    if (Time.time - lastRemoval > timerequired && PhotonNetwork.isMasterClient)
                    {
                        me.ShipStats.Ship.RemoveOneRandomHostileVirus();
                        lastRemoval = Time.time;
                    }
                }
            }
        }
        public class QDI_Fix : CPUMod
        {
            PLCPU me = null;
            public override string Name => "QDI-FIX-ALL";

            public override string Description => "Processor that slightly repairs all systems whenever a program is run. Maximum of 10 times per sector.";

            public override int MarketPrice => 10000;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 1f;
            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "All systems heal:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                PLCPU me = InComp as PLCPU;
                return (10f * me.LevelMultiplier(0.25f, 1f)).ToString("0.0") + "%";
            }
            public override void AddStats(PLShipComponent InComp)
            {
                me = InComp as PLCPU;
            }
            public override void WhenProgramIsRun(PLWarpDriveProgram InProgram)
            {
                if (me != null && me.SubTypeData < 10)
                {
                    if (me.ShipStats.Ship.WeaponsSystem != null)
                    {
                        PLWeaponsSystem weaponsSystem = me.ShipStats.Ship.WeaponsSystem;
                        weaponsSystem.Health += 5f * me.LevelMultiplier(0.25f, 1f);
                    }
                    if (me.ShipStats.Ship.EngineeringSystem != null)
                    {
                        PLEngineeringSystem weaponsSystem = me.ShipStats.Ship.EngineeringSystem;
                        weaponsSystem.Health += 5f * me.LevelMultiplier(0.25f, 1f);
                    }
                    if (me.ShipStats.Ship.LifeSupportSystem != null)
                    {
                        PLLifeSupportSystem lifeSupportSystem = me.ShipStats.Ship.LifeSupportSystem;
                        lifeSupportSystem.Health += 5f * me.LevelMultiplier(0.25f, 1f);
                    }
                    if (me.ShipStats.Ship.ComputerSystem != null)
                    {
                        PLComputerSystem computerSystem = me.ShipStats.Ship.ComputerSystem;
                        computerSystem.Health += 5f * me.LevelMultiplier(0.25f, 1f);
                    }
                }
            }
        }
        public class ShieldMaster : CPUMod
        {
            public override string Name => "Shield Master";

            public override string Description => "This special processor will heavily boost your shield max integrity and give a little extra charge rate, but it comes with the cost of really high power consumption, lack of power will colapse any extra shields this processor gave.";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 5000;

            public override void FinalLateAddStats(PLShipComponent InComp)
            {
                PLCPU cpu = InComp as PLCPU;
                cpu.IsPowerActive = true;
                cpu.RequestPowerUsage_Percent = 1f;
                if (cpu.GetPowerPercentInput() > 0.75)
                {
                    InComp.ShipStats.ShieldsMax += 0.2f * InComp.LevelMultiplier(0.2f, 1) * InComp.ShipStats.ShieldsMax;
                    InComp.ShipStats.ShieldsChargeRateMax += 0.05f * InComp.LevelMultiplier(0.3f, 1) * InComp.ShipStats.ShieldsChargeRateMax;
                }

            }

            public override void Tick(PLShipComponent InComp)
            {
                base.Tick(InComp);
                PLCPU cpu = (PLCPU)InComp;
                cpu.m_MaxPowerUsage_Watts = 5000f * cpu.LevelMultiplier(0.1f, 1);
            }

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Shield Percent Boost: \nShield Charge Rate:";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                return $"{20f * InComp.LevelMultiplier(0.2f, 1)}%\n{5 * InComp.LevelMultiplier(0.3f, 1)}%";
            }
        }
        public class ResearchScanner : CPUMod
        {
            public override string Name => "Research Scanner";

            public override string Description => "Did you ever get stuck trying to find a research in the middle of a blue rock? Now with this active scanner processor your scientist will never have to worry about that again!";

            public override int MarketPrice => 15000;

            public override float MaxPowerUsage_Watts => 500f;
        }
        public class Assembly : CPUMod
        {
            public override string Name => "Assembly Processor";

            public override string Description => "With this special processor full of nano technology, you will be able to replace and upgrade components anywhere! Don't question the science behind it, it works, and that is all that matters.";

            public override int MarketPrice => 45000;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 4001;
        }
        public class Immortality : CPUMod
        {
            public override string Name => "Immortality Processor";

            public override string Description => "This processor makes you and your ship literally immortals, so I hope you understand why it is worth this much, do you know how much space magic it took to create this?";

            public override int MarketPrice => 100000000;

            public override bool Contraband => true;

            public override float MaxPowerUsage_Watts => 1;
        }
        public class Upgrader : CPUMod
        {
            public override string Name => "The Upgrader";

            public override string Description => "This processor increases the max level of components and item upgrades.";

            public override int MarketPrice => 65000;

            public override bool Experimental => true;

            public override float MaxPowerUsage_Watts => 1;

            public override string GetStatLineLeft(PLShipComponent InComp)
            {
                return "Max level increase: ";
            }

            public override string GetStatLineRight(PLShipComponent InComp)
            {
                return $"{Mathf.RoundToInt((InComp.Level+1)*2.2f)}";
            }

            public override void Tick(PLShipComponent InComp)
            {
                PLCPU me = (PLCPU)InComp;
                me.MaxCompUpgradeLevelBoost = Mathf.RoundToInt((InComp.Level + 1) * 2.2f);
                me.MaxPawnItemUpgradeLevelBoost = Mathf.RoundToInt((InComp.Level + 1) * 2.2f);
            }
        }

    }

    [HarmonyPatch(typeof(PLTurret), "Tick")]
    class HeatMax
    {
        static void Prefix(PLTurret __instance)
        {
            float thermoBoost = CPUS.ThermoBoost.GetMaxHeat(__instance.ShipStats.Ship);
            if (__instance.Heat >= thermoBoost)
            {
                __instance.IsOverheated = true;
                if (__instance.SubType == 15)
                {
                    PLMusic.PostEvent("play_ship_generic_external_weapon_biohazard_overheat", __instance.TurretInstance.gameObject);
                }
                else
                {
                    PLMusic.PostEvent("play_ship_generic_external_weapon_flamelance_overheat", __instance.TurretInstance.gameObject);
                }
            }
            __instance.Heat = Mathf.Clamp(__instance.Heat, 0, thermoBoost);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> targetSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldfld,AccessTools.Field(typeof(PLTurret),"Heat")),
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Ldc_R4, 1.1f),
            };
            List<CodeInstruction> patchSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldfld,AccessTools.Field(typeof(PLTurret),"Heat")),
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Ldc_R4, 1000f),
            };
            instructions = HarmonyHelpers.PatchBySequence(instructions, targetSequence, patchSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL, false);

            targetSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLTurret),"Heat")),
                new CodeInstruction(OpCodes.Ldc_R4, 1.1f),
            };
            patchSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLTurret),"Heat")),
                new CodeInstruction(OpCodes.Ldc_R4, 1000f),
            };
            patchSequence[0].labels = instructions.ToList()[FindSequence(instructions, targetSequence, CheckMode.NONNULL) - 3].labels;
            return HarmonyHelpers.PatchBySequence(instructions, targetSequence, patchSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL, false);
        }
    }
    [HarmonyPatch(typeof(PLUITurretUI), "Update")]
    class HeatUIUpdate
    {
        static void Postfix(PLUITurretUI __instance)
        {
            PLTurret plturret = null;
            PLShipInfoBase plshipInfoBase = null;
            if (PLEncounterManager.Instance.PlayerShip == null) return;
            if (PLNetworkManager.Instance.MyLocalPawn != null)
            {
                if (PLNetworkManager.Instance.MyLocalPawn.IsDead) return;
                plshipInfoBase = PLNetworkManager.Instance.MyLocalPawn.CurrentShip;
            }
            if (plshipInfoBase == null && PLCameraSystem.Instance.GetModeString() == "Turret" && PLCameraSystem.Instance.CurrentCameraMode != null)
            {
                PLCameraMode_Turret plcameraMode_Turret = PLCameraSystem.Instance.CurrentCameraMode as PLCameraMode_Turret;
                if (plcameraMode_Turret != null)
                {
                    plshipInfoBase = plcameraMode_Turret.ShipInfo;
                }
            }
            if (plshipInfoBase != PLEncounterManager.Instance.PlayerShip) return;
            if (PLNetworkManager.Instance.MyLocalPawn != null && plshipInfoBase != null)
            {
                for (int i = 0; i < plshipInfoBase.GetCurrentTurretControllerMaxTurretIndex(); i++)
                {
                    if (plshipInfoBase.GetCurrentTurretControllerPlayerID(i) == PLNetworkManager.Instance.LocalPlayerID)
                    {
                        plturret = plshipInfoBase.GetTurretAtID(i);
                        break;
                    }
                }
            }
            if (plturret != null)
            {
                float value = plturret.Heat / CPUS.ThermoBoost.GetMaxHeat(plturret.ShipStats.Ship);
                __instance.RightUI_Fill.fillAmount = value * 0.25f;
                __instance.RightUI_FillOut.fillAmount = value * 0.25f;
            }
        }
    }

    [HarmonyPatch(typeof(PLShipStats), "TakeHullDamage")]
    class Inevitable
    {
        static void Postfix(PLShipStats __instance, float __result)
        {
            List<PLShipComponent> listofhulls = __instance.GetComponentsOfType(ESlotType.E_COMP_HULL, false);
            if (listofhulls.Count <= 0) return;
            PLHull hull = listofhulls[0] as PLHull;
            if (hull.Current > __result) return;
            List<PLShipComponent> componentsOfType2 = __instance.GetComponentsOfType(ESlotType.E_COMP_CPU, false);
            if (componentsOfType2 != null && PhotonNetwork.isMasterClient)
            {
                bool found = false;
                foreach (PLShipComponent plshipComponent in componentsOfType2)
                {
                    if (plshipComponent != null && plshipComponent.SubType == CPUModManager.Instance.GetCPUIDFromName("The Premonition") && CPUS.The_Premonition.lastLive >= 20 &&
                        PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.LCWBATTLE && PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.TOPSEC && PLServer.GetCurrentSector().VisualIndication != ESectorVisualIndication.UNSEEN_MS && plshipComponent.IsEquipped)
                    {
                        found = true;
                        if (PhotonNetwork.isMasterClient)
                        {
                            PLServer.Instance.photonView.RPC("AddNotification_OneString_LocalizedString", PhotonTargets.All, new object[]
                            {
                                        "[STR0] detected a death! Ending timeline visualisation!",
                                        -1,
                                        PLServer.Instance.GetEstimatedServerMs() + 8000,
                                        true,
                                        plshipComponent.Name_NoTranslation
                            });
                        }
                        CPUS.The_Premonition.lastLive = -2;
                        hull.Current = CPUS.The_Premonition.lastHull;
                        __instance.HullCurrent = CPUS.The_Premonition.lastHull;
                        List<PLShipComponent> listofshields = __instance.GetComponentsOfType(ESlotType.E_COMP_SHLD, false);
                        if (listofshields.Count <= 0) break;
                        (listofshields[0] as PLShieldGenerator).Current = __instance.ShieldsMax;
                    }
                    else if (plshipComponent != null && plshipComponent.SubType == CPUModManager.Instance.GetCPUIDFromName("Immortality Processor") && plshipComponent.IsEquipped && !__instance.Ship.IsAbandoned())
                    {
                        if (__instance.Ship.GetIsPlayerShip())
                        {
                            PLServer.Instance.photonView.RPC("AddNotification_OneString_LocalizedString", PhotonTargets.All, new object[]
                            {
                                        "[STR0] prevented your death!",
                                        -1,
                                        PLServer.Instance.GetEstimatedServerMs() + 8000,
                                        true,
                                        plshipComponent.Name_NoTranslation
                            });
                        }
                        if (__instance.Ship.IsDrone) 
                        {
                            plshipComponent.FlagForSelfDestruction();
                        }
                        hull.Current = __instance.HullMax;
                        __instance.HullCurrent = __instance.HullMax;
                    }
                }
                if (found)
                {
                    PLShipInfo ship = PLEncounterManager.Instance.PlayerShip;
                    ship.NumberOfFuelCapsules++;
                    PLMusic.PostEvent("play_sx_env_ship_powerdownslow", ship.gameObject);
                    ship.WarpTargetID = PLServer.GetCurrentSector().ID;
                    ship.SetInWarp(true);
                    int classID = 0;
                    foreach (Vector3 position in CPUS.The_Premonition.crewPosition)
                    {
                        PLPlayer player = PLEncounterManager.Instance.PlayerShip.GetRelevantCrewMember(classID, false);
                        if (position.y != 0 && position.x != 0 && position.z != 0 && player != null && player.GetPawn() != null && !player.GetPawn().IsDead)
                        {
                            player.GetPawn().Heal(5000);
                            player.photonView.RPC("RecallPawnToPos", PhotonTargets.All, new object[]
                            {
                        position
                            });
                        }
                        classID++;
                    }
                    List<PLFire> firelist = ship.AllFires.Values.ToList();
                    for (int i = firelist.Count - 1; i > -1; i--)
                    {
                        PLServer.Instance.photonView.RPC("RemoveFireWithID", PhotonTargets.All, new object[]
                        {
                            ship.ShipID,
                            firelist[i].FireID
                        });
                    }
                    ship.EngineeringSystem.Health = ship.EngineeringSystem.MaxHealth;
                    ship.WeaponsSystem.Health = ship.WeaponsSystem.MaxHealth;
                    ship.ComputerSystem.Health = ship.ComputerSystem.MaxHealth;
                    if (ship.LifeSupportSystem != null) ship.LifeSupportSystem.Health = ship.LifeSupportSystem.MaxHealth;
                    ship.NewShipController(-1);
                    ship.NewSensorDishController(-1);
                    ship.NewTurretController(0, -1);
                    ship.NewTurretController(1, -1);
                    ship.NewTurretController(2, -1);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLPersistantEncounterInstance), "SpawnEnemyShip")]
    class StoreCrewPosition
    {
        static void Postfix(PLShipInfoBase __result)
        {
            if (PhotonNetwork.isMasterClient)
            {
                for (int i = 0; i < 5; i++)
                {
                    PLPlayer player = PLEncounterManager.Instance.PlayerShip.GetRelevantCrewMember(i, false);
                    if (player != null && player.GetPawn() != null && !player.GetPawn().IsDead)
                    {
                        CPUS.The_Premonition.crewPosition[i] = player.GetPawn().gameObject.transform.position;
                    }
                    else
                    {
                        CPUS.The_Premonition.crewPosition[i] = new Vector3(0, 0, 0);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLCPU), "OnWarp")]
    class ManualOnWarp
    {
        static void Postfix(PLCPU __instance)
        {
            if (PhotonNetwork.isMasterClient)
            {
                int subtypeformodded = __instance.SubType - CPUModManager.Instance.VanillaCPUMaxType;
                if (subtypeformodded > -1 && subtypeformodded < CPUModManager.Instance.CPUTypes.Count && __instance.ShipStats != null)
                {
                    CPUModManager.Instance.CPUTypes[subtypeformodded].OnWarp(__instance);
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLUIOutsideWorldUI), "UpdateKeenUIElements")]
    class ReasearchLocator
    {
        static void Postfix(PLUIOutsideWorldUI __instance)
        {
            if (PLCameraSystem.Instance.GetModeString() == "SensorDish" && __instance.MyShipInfo != null && __instance.MyShipInfo.GetIsPlayerShip() && __instance.MyShipInfo.MyStats != null)
            {
                foreach (PLShipComponent component in PLEncounterManager.Instance.PlayerShip.MyStats.AllComponents)
                {
                    if (component.ActualSlotType == ESlotType.E_COMP_CPU && component.SubType == CPUModManager.Instance.GetCPUIDFromName("Research Scanner") && component.IsEquipped)
                    {
                        foreach (PLProbePickup pLProbePickup in Object.FindObjectsOfType<PLProbePickup>())
                        {
                            if (pLProbePickup != null && !pLProbePickup.PickedUp)
                            {
                                __instance.RequestKeenUIElement(pLProbePickup.transform, "Research", null);
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PLShipInfoBase), "CanSwapComponentsWithoutDepot")]
    class AssemblyAbility
    {
        static void Postfix(PLShipInfoBase __instance, ref bool __result)
        {
            foreach (PLShipComponent component in PLEncounterManager.Instance.PlayerShip.MyStats.AllComponents)
            {
                if (component.ActualSlotType == ESlotType.E_COMP_CPU && component.SubType == CPUModManager.Instance.GetCPUIDFromName("Assembly Processor") && component.IsEquipped)
                {
                    __result = true;
                    break;
                }
            }
        }
    }
}
