using PulsarModLoader.Content.Components.WarpDrive;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
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

            public override float MaxPowerUsage_Watts => 15000f;

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
			if (__instance.MyWarpDrive != null && __instance.MyWarpDrive.Name == "Ultimate Explorer" && PLServer.Instance.m_ShipCourseGoals.Count > 0)
			{
				PLSectorInfo plsectorInfo3 = PLGlobal.Instance.Galaxy.AllSectorInfos[PLServer.Instance.GetCurrentHubID()];
				PLSectorInfo plsectorInfo4 = PLGlobal.Instance.Galaxy.AllSectorInfos.GetValueSafe(PLServer.Instance.m_ShipCourseGoals[0]);
				if (plsectorInfo4 != null && plsectorInfo4.IsThisSectorWithinPlayerWarpRange() && plsectorInfo4.VisualIndication != ESectorVisualIndication.TOPSEC && plsectorInfo4.VisualIndication != ESectorVisualIndication.LCWBATTLE && (plsectorInfo4.VisualIndication == ESectorVisualIndication.COMET || PLStarmap.ShouldShowSectorBG(plsectorInfo4)))
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
			}
		}
    }
	[HarmonyPatch(typeof(PLGalaxy), "GetPathToSector")]
	class PathToSector 
	{
		static void Postfix(PLSectorInfo inStartSector, PLSectorInfo inEndSector, ref List<PLSectorInfo> __result) 
		{
			List<PLSectorInfo> sectorInfos = new List<PLSectorInfo>();
			if (PLServer.Instance.m_ShipCourseGoals.Count > 0 && PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name == "Ultimate Explorer")
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
			if (PLServer.Instance.m_ShipCourseGoals.Count > 0 && PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name == "Ultimate Explorer" && __instance.ID == PLServer.Instance.m_ShipCourseGoals[0]) 
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
			if (PLServer.Instance.m_ShipCourseGoals.Count <= 0 || PLEncounterManager.Instance.PlayerShip.MyWarpDrive.Name != "Ultimate Explorer") return;
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
