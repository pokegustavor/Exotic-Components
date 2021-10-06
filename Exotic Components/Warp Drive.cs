using PulsarModLoader.Content.Components.WarpDrive;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
namespace Exotic_Components
{
    class Warp_Drive
    {
        class UltimateExplorer : WarpDriveMod
        {
            public override string Name => "Ultimate Explorer";

            public override string Description => "A powerfull warpdrive using a similar technology to a flagship drive. While it won't warp you outside of the galaxy, it will warp anywhere inside of it (as long as you have the coordinates)";

            public override int MarketPrice => 70000;

            public override bool Experimental => true;

            public override Texture2D IconTexture => (Texture2D)Resources.Load("Icons/75_Warp");

			public override float ChargeSpeed => 0.8f;

            public override float WarpRange => 0;

            public override float EnergySignature => 100;

            public override int NumberOfChargesPerFuel => 3;

            //public override float MaxPowerUsage_Watts => 15000f;
			public override string GetStatLineLeft(PLShipComponent InComp)
			{
				return string.Concat(new string[]
				{
				PLLocalize.Localize("Charge Rate", false),
				"\n",
				PLLocalize.Localize("Range", false),
				"\n",
				PLLocalize.Localize("Charges Per Fuel", false)
				});
			}
			public override string GetStatLineRight(PLShipComponent InComp)
            {
				PLWarpDrive me = InComp as PLWarpDrive;
				me.CalculatedMaxPowerUsage_Watts = 15000f;
				return string.Concat(new string[]
				{
				(me.ChargeSpeed * me.LevelMultiplier(0.25f, 1f)).ToString("0"),
				"\n",
				"Galaxy",
				"\n",
				me.NumberOfChargingNodes.ToString("0")
				});
			}
        }
		public class UltimateExplorerMK2 : WarpDriveMod
		{
			public override string Name => "Ultimate Explorer MK2";

			public override string Description => "An overclocked version of the Ultimate Explorer that now charges way faster and has an extra program charge (We are not responsible for any malfuntion caused by this overclock)";

			public override int MarketPrice => 80000;

			public override bool Experimental => true;

			public override Texture2D IconTexture => (Texture2D)Resources.Load("Icons/75_Warp");

			public override float ChargeSpeed => 3f;

			public override float WarpRange => 0;

			public override float EnergySignature => 100;

			public override int NumberOfChargesPerFuel => 4;

			//public override float MaxPowerUsage_Watts => 17000f;
			public override string GetStatLineLeft(PLShipComponent InComp)
			{
				return string.Concat(new string[]
				{
				PLLocalize.Localize("Charge Rate", false),
				"\n",
				PLLocalize.Localize("Range", false),
				"\n",
				PLLocalize.Localize("Charges Per Fuel", false)
				});
			}
			public override string GetStatLineRight(PLShipComponent InComp)
			{
				PLWarpDrive me = InComp as PLWarpDrive;
				me.CalculatedMaxPowerUsage_Watts = 15000f;
				return string.Concat(new string[]
				{
				(me.ChargeSpeed * me.LevelMultiplier(0.25f, 1f)).ToString("0"),
				"\n",
				"Galaxy",
				"\n",
				me.NumberOfChargingNodes.ToString("0")
				});
			}
			public static float LastFailure = Time.time;
            public override void OnWarp(PLShipComponent InComp)
            {
				if (!PhotonNetwork.isMasterClient) return;
				if ((InComp as PLWarpDrive).Name != "Ultimate Explorer MK2") return;
				PLSectorInfo current = PLServer.GetCurrentSector();
				PLSectorInfo destiny = PLGlobal.Instance.Galaxy.AllSectorInfos.GetValueSafe(PLEncounterManager.Instance.PlayerShip.WarpTargetID);
				if (UnityEngine.Random.Range(0, 100) <= Mathf.Min(1000 * Vector2.Distance(current.Position,destiny.Position),35) && Time.time - LastFailure > 20f) 
				{
					if(PLEncounterManager.Instance.PlayerShip.gameObject.GetComponent<Heart>() == null) 
					{
						PLEncounterManager.Instance.PlayerShip.gameObject.AddComponent<Heart>();
					}
					Heart heart = PLEncounterManager.Instance.PlayerShip.gameObject.GetComponent<Heart>();
					if (destiny.IsPartOfLongRangeWarpNetwork || destiny.VisualIndication == ESectorVisualIndication.LCWBATTLE || destiny.VisualIndication == ESectorVisualIndication.TOPSEC) return;
					heart.StartCoroutine(heart.drivefailure(current, destiny));
				}
            }

			
        }
	}

	class Heart : MonoBehaviour 
	{
		public static int destinyID = 0;
		public static bool failing = false;
		public IEnumerator drivefailure(PLSectorInfo current, PLSectorInfo destiny)
		{
			float A = current.Position.y - destiny.Position.y;
			float B = destiny.Position.x - current.Position.x;
			float C = (current.Position.x * destiny.Position.y) - (destiny.Position.x * current.Position.y);
			List<PLSectorInfo> possibleSectors = new List<PLSectorInfo>();
			foreach (PLSectorInfo sector in PLGlobal.Instance.Galaxy.AllSectorInfos.Values)
			{
				if (sector == current || sector == destiny) continue;
				/*
				if (sector.Position.x > Mathf.Min(current.Position.x, destiny.Position.x) && sector.Position.x < Mathf.Max(current.Position.x, destiny.Position.x) && sector.Position.y > Mathf.Min(current.Position.y, destiny.Position.y) && sector.Position.x < Mathf.Max(current.Position.y, destiny.Position.y))
				{
					possibleSectors.Add(sector);
				}
				*/
				float D = Math.Abs(A * sector.Position.x + B * sector.Position.y + C) / (float)Math.Sqrt(A * A + B * B);
				PulsarModLoader.Utilities.Logger.Info(sector.ID + " result: " + D);
				if (D < 0.005f && sector.Position.x > Mathf.Min(current.Position.x, destiny.Position.x) && sector.Position.x < Mathf.Max(current.Position.x, destiny.Position.x) && sector.Position.y > Mathf.Min(current.Position.y, destiny.Position.y) && sector.Position.x < Mathf.Max(current.Position.y, destiny.Position.y)) 
				{
					possibleSectors.Add(sector);
				}
			}
			if (possibleSectors.Count == 0) yield break;
			int ID = possibleSectors[(int)UnityEngine.Random.Range(0, possibleSectors.Count - 1)].ID;
			yield return new WaitForSeconds(6f);
			failing = true;
			destinyID = ID;
			PLEncounterManager.Instance.PlayerShip.WarpTargetID = ID;
			PLEncounterManager.Instance.PlayerShip.NumberOfFuelCapsules++;
			PLServer.Instance.photonView.RPC("CPEI_HandleActivateWarpDrive", PhotonTargets.MasterClient, new object[]
			{
				PLEncounterManager.Instance.PlayerShip.ShipID,
				ID,
				0
			});
			PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
			{
				"WARP DRIVE MALFUNCTION DETECTED!",
				Color.red,
				5,
				"MSN"
			});
			Warp_Drive.UltimateExplorerMK2.LastFailure = Time.time;
			yield break;
		}
	}

	[HarmonyPatch(typeof(PLWarpDrive), "OnWarpTo")]
	class FixOnWarp 
	{
		static void Postfix(PLWarpDrive __instance)
		{
			int subtypeformodded = __instance.SubType - WarpDriveModManager.Instance.VanillaWarpDriveMaxType;
			if (subtypeformodded > -1 && subtypeformodded < WarpDriveModManager.Instance.WarpDriveTypes.Count && __instance.ShipStats != null)
			{
				WarpDriveModManager.Instance.WarpDriveTypes[subtypeformodded].OnWarp(__instance);
			}
		}
	}
	[HarmonyPatch(typeof(PLGameProgressManager), "OnWarpCompleted")]
	class updateFailure 
	{
		static void Postfix() 
		{
			Heart.failing = false;
			if (PLServer.GetCurrentSector().Name == "The Core(MOD)")
			{
				InitialStore.UpdateCore();
			}
		}
	}
    [HarmonyPatch(typeof(PLShipInfo),"Update")]
    class ShipUpdate 
    {
        static void Postfix(PLShipInfo __instance) 
        {
			/*
            if(__instance.MyWarpDrive != null && __instance.MyWarpDrive.Name == "Ultimate Explorer" && PLServer.Instance.m_ShipCourseGoals.Count > 0) 
            {
                __instance.WarpTargetID = PLServer.Instance.m_ShipCourseGoals[0];
            }
			*/

			if (__instance.MyWarpDrive != null && (__instance.MyWarpDrive.Name == "Ultimate Explorer" || __instance.MyWarpDrive.Name == "Ultimate Explorer MK2") && PLServer.Instance.m_ShipCourseGoals.Count > 0)
			{
				PLSectorInfo plsectorInfo3 = PLGlobal.Instance.Galaxy.AllSectorInfos[PLServer.Instance.GetCurrentHubID()];
				PLSectorInfo plsectorInfo4 = PLGlobal.Instance.Galaxy.AllSectorInfos.GetValueSafe(PLServer.Instance.m_ShipCourseGoals[0]);
				if (PLEncounterManager.Instance.PlayerShip.WarpChargeStage != EWarpChargeStage.E_WCS_ACTIVE && plsectorInfo4 != null && plsectorInfo4.IsThisSectorWithinPlayerWarpRange() && plsectorInfo4.VisualIndication != ESectorVisualIndication.TOPSEC && plsectorInfo4.VisualIndication != ESectorVisualIndication.LCWBATTLE && (plsectorInfo4.VisualIndication == ESectorVisualIndication.COMET || PLStarmap.ShouldShowSectorBG(plsectorInfo4)))
				{
					float closestWarpTargetDot = 0f;
					Vector3 relPos_PlayerToSector = PLGlobal.GetRelPos_PlayerToSector(plsectorInfo4, plsectorInfo3);
					float num14 = Vector3.Dot(__instance.ExteriorTransformCached.forward, relPos_PlayerToSector.normalized);
					if (num14 > closestWarpTargetDot)
					{
						closestWarpTargetDot = num14;
						if (closestWarpTargetDot > 0.996f)
						{
							__instance.WarpTargetID = plsectorInfo4.ID;
						}
					}
				}
				else if(PLEncounterManager.Instance.PlayerShip.WarpChargeStage == EWarpChargeStage.E_WCS_ACTIVE && __instance.MyWarpDrive.Name == "Ultimate Explorer MK2" && Heart.failing) 
				{
					__instance.WarpTargetID = Heart.destinyID;
				}
			}
		}
    }
	[HarmonyPatch(typeof(PLGalaxy), "GetPathToSector")]
	class PathToSector 
	{
		static void Postfix(PLSectorInfo inStartSector, PLSectorInfo inEndSector, ref List<PLSectorInfo> __result) 
		{
			List<PLSectorInfo> sectorInfos = new List<PLSectorInfo>();
			if (PLServer.Instance.m_ShipCourseGoals.Count > 0 && (PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name == "Ultimate Explorer" || PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name == "Ultimate Explorer MK2"))
			{
				foreach(int ID in PLServer.Instance.m_ShipCourseGoals) 
				{
					sectorInfos.Add(PLGlobal.Instance.Galaxy.AllSectorInfos.GetValueSafe(ID));
				}
				__result.Clear();
				__result.AddRange(sectorInfos);
			}
		}
	}

	[HarmonyPatch(typeof(PLSectorInfo), "IsThisSectorWithinPlayerWarpRange")]
	class IsSectorInRange 
	{
		static void Postfix(PLSectorInfo __instance, ref bool __result) 
		{
			if (PLServer.Instance.m_ShipCourseGoals.Count > 0 && (PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name == "Ultimate Explorer" || PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name == "Ultimate Explorer MK2") && __instance.ID == PLServer.Instance.m_ShipCourseGoals[0]) 
			{
				__result = true;
			}
		}
	}

    [HarmonyPatch(typeof(PLUIOutsideWorldUI), "UpdateSectorUIs")]
    class RenderSector 
    {   
		static void Prefix(PLUIOutsideWorldUI __instance) 
		{
			Postfix(__instance);
		}
        static void Postfix(PLUIOutsideWorldUI __instance) 
        {
			if (PLServer.Instance.m_ShipCourseGoals.Count <= 0 || (PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name != "Ultimate Explorer" && PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name != "Ultimate Explorer MK2")) return;
			PLSectorInfo plsectorInfo = PLServer.GetCurrentSector();
			Vector3 vector = plsectorInfo.Position;
			PLSectorInfo plsectorInfo4;
			float num = Mathf.Clamp((PLCameraSystem.Instance.CurrentSubSystem.MainCameras[0].fieldOfView - 80f) * 0.1f, -0.4f, 2f);
			PLGlobal.Instance.Galaxy.AllSectorInfos.TryGetValue(PLServer.Instance.m_ShipCourseGoals[0],out plsectorInfo4);
			PLUIOutsideWorldUI.SectorUIElement sectorUIElementForSector = __instance.GetSectorUIElementForSector(plsectorInfo4, true);
			if (sectorUIElementForSector != null)
			{
				sectorUIElementForSector.LastProcessedFrame = Time.frameCount;
				Vector3 relPos_PlayerToPosition = PLGlobal.GetRelPos_PlayerToPosition(plsectorInfo4, vector);
				sectorUIElementForSector.Root.transform.position = PLCameraSystem.Instance.CurrentSubSystem.MainCameras[0].transform.position + PLGlobal.GetRelPosOffset_PlayerToSector(relPos_PlayerToPosition);
				sectorUIElementForSector.Root.transform.rotation = PLGlobal.SafeLookRotation(relPos_PlayerToPosition.normalized, PLCameraSystem.Instance.CurrentSubSystem.MainCameras[0].transform.up);
				if (plsectorInfo4.ID == PLEncounterManager.Instance.PlayerShip.WarpTargetID)
				{
					sectorUIElementForSector.Label.color = Color.white;
					sectorUIElementForSector.BG.color = PLInGameUI.FromAlpha(Color.Lerp(Color.black, PLGlobal.Instance.ClassColors[0], Mathf.Sin(Time.time * 9f) * 0.25f + 0.3f), 0.4f);
				}
				else
				{
					sectorUIElementForSector.Label.color = Color.white;
					sectorUIElementForSector.BG.color = PLInGameUI.FromAlpha(Color.black, 0.4f);
				}
				float num3 = Mathf.Clamp(Vector3.Dot(relPos_PlayerToPosition.normalized, PLCameraSystem.Instance.CurrentSubSystem.MainCameras[0].transform.forward), -1f, 1f);
				num3 = Mathf.Pow(num3, 9f);
				float num4 = Mathf.Clamp(Vector3.Dot(relPos_PlayerToPosition.normalized, PLEncounterManager.Instance.PlayerShip.Exterior.transform.forward), 0f, 1f);
				num4 *= num4;
				num4 *= num4;
				num4 *= num4;
				num4 *= num4;
				num4 *= num4;
				if (num3 > 0f)
				{
					sectorUIElementForSector.Root.transform.localScale = Vector3.Lerp(sectorUIElementForSector.Root.transform.localScale, Vector3.one * (3f + num4 * 3f + num), num3 * 18f * Time.deltaTime);
				}
				else
				{
					sectorUIElementForSector.Root.transform.localScale = Vector3.zero;
				}
				__instance.LastRequestWaypointSectorUIFrame = Time.frameCount;
				__instance.NextWaypointSector.transform.position = sectorUIElementForSector.Root.transform.position;
				__instance.NextWaypointSector.transform.LookAt(PLCameraSystem.Instance.CurrentSubSystem.MainCameras[0].transform);
				sectorUIElementForSector.Image.color = sectorUIElementForSector.Label.color;
				PLGlobal.SafeLabelSetText(sectorUIElementForSector.Label, plsectorInfo4.Name);
			}
		}
    }
}
