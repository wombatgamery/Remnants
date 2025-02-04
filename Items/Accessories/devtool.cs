//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace Remnants.Items.Accessories
//{
//	[AutoloadEquip(EquipType.Wings)]
//	public class devtool : ModItem
//	{
//		public override void SetDefaults()
//		{
//			Item.width = 11;
//			Item.height = 8;
//			Item.accessory = true;
//			Item.maxStack = 99;
//			Item.value = 100;
//			Item.rare = 0;
//		}

//		public override void UpdateAccessory(Player player, bool hideVisual)
//		{
//			player.endurance = 1f;
//			player.lifeRegen = (int)200f;
//			player.maxRunSpeed = 12f;
//			player.moveSpeed = 12f;
//			player.pickSpeed = 0.02f;
//			player.tileSpeed = 200f;
//			player.wallSpeed = 200f;
//			player.wingTimeMax = 9999;
//			player.noKnockback = true;
//			player.noFallDmg = true;
//			player.lavaImmune = true;
//			player.maxFallSpeed = 25f;
//			player.buffImmune[BuffID.ChaosState] = true;
//			player.buffImmune[BuffID.Burning] = true;
//			player.buffImmune[BuffID.Cursed] = true;
//			player.buffImmune[BuffID.Silenced] = true;
//			player.buffImmune[BuffID.OnFire] = true;
//			player.buffImmune[BuffID.CursedInferno] = true;
//			//player.buffImmune[BuffID.] = true;
//			player.buffImmune[BuffID.Poisoned] = true;
//			player.buffImmune[BuffID.Venom] = true;
//			player.buffImmune[BuffID.Slow] = true;
//			player.buffImmune[BuffID.Darkness] = true;
//			player.buffImmune[BuffID.Blackout] = true;
//			player.buffImmune[BuffID.VortexDebuff] = true;
//			player.buffImmune[BuffID.Stoned] = true;
//			player.buffImmune[BuffID.Frozen] = true;
//			player.buffImmune[BuffID.Webbed] = true;
//			player.buffImmune[BuffID.Confused] = true;
//			player.buffImmune[BuffID.Chilled] = true;
//			player.buffImmune[BuffID.Suffocation] = true;
//			player.buffImmune[BuffID.TheTongue] = true;

//			player.accMerman = true;
//		}

//        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
//			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
//		{
//			ascentWhenFalling = 0.85f;
//			ascentWhenRising = 0.15f;
//			maxCanAscendMultiplier = 1f;
//			maxAscentMultiplier = 3f;
//			constantAscend = 0.5f;
//		}

//		public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
//		{
//			speed = 9f;
//			acceleration *= 2.5f;
//		}
//	}
//}