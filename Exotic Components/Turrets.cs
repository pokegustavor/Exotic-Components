using PulsarModLoader.Content.Components.Turret;
using UnityEngine;
namespace Exotic_Components
{
    internal class Turrets
    {
		class SupremeRailGunMod : TurretMod
		{
			public override string Name => "Supreme RailGun";

			public override PLShipComponent PLTurret => new SupremeRailGun();
		}
    }

    class SupremeRailGun : PLBasicTurret 
    {
		public SupremeRailGun(int inLevel = 0, int inSubTypeData = 0)
		{
			this.Name = "Supreme RailGun";
			this.Desc = "Recovered Acient Human technology, this Railgun is the strongest of its kind, even if not as powerfull as the original";
			this.m_Damage = 450f;
			base.SubType = TurretModManager.Instance.GetTurretIDFromName("Supreme RailGun");
			this.m_MarketPrice = 12000;
			this.m_MaxPowerUsage_Watts = 7000f;
			this.m_ProjSpeed = 230f;
			this.FireDelay = 4.5f;
			this.Experimental = true;
			this.TurretRange = 5400f;
			base.CargoVisualPrefabID = 3;
			base.Level = inLevel;
			this.OnFire_CameraShakeAmt = 0.8f;
			this.FireTurretSoundSFX = "play_ship_generic_external_weapon_railgun_shoot";
		}
		protected override string GetTurretPrefabPath()
		{
			return "NetworkPrefabs/Component_Prefabs/RailgunTurretAncient";
		}
		public override void Fire(int inProjID, Vector3 dir)
		{
			base.Fire(inProjID, dir);
			if (this.TurretInstance != null)
			{
				foreach (object obj in this.TurretInstance.GetComponent<Animation>())
				{
					((AnimationState)obj).speed = 8f;
				}
			}
			ParticleSystem component = this.TurretInstance.OptionalGameObjects[0].GetComponent<ParticleSystem>();
			if (component != null)
			{
				component.Emit(2);
			}
			ParticleSystem component2 = this.TurretInstance.OptionalGameObjects[1].GetComponent<ParticleSystem>();
			if (component2 != null)
			{
				component2.Emit(12);
			}
		}
	}

}
