using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Buffs;
using Remnants.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Weapons
{
	[LegacyName("spiritlance")]
	public class SpiritLance : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use animation-tied sound playback, so that we're able to make it be tied to use time instead in the UseItem() hook.
			ItemID.Sets.Spears[Item.type] = true; // This allows the game to recognize our new item as a spear.

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Spear);

			Item.width = 28 * 2;
			Item.height = 28 * 2;

			Item.damage = 12;
			Item.crit = -4;
			Item.knockBack = 4f;

			Item.useAnimation = 20; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useTime = 20; // The length of the item's use time in ticks (60 ticks == 1 second.)
			Item.UseSound = SoundID.Item15; // The sound that this item plays when used.

			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(gold: 1);

			Item.shootSpeed = 2f; // The speed of the projectile measured in pixels per frame.
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapons.SpiritLanceHoldout>(); // The projectile that is fired from this weapon
		}

		public override bool CanUseItem(Player player)
		{
			// Ensures no more than one spear can be thrown out, use this when using autoReuse
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

		public override bool? UseItem(Player player)
		{
			// Because we're skipping sound playback on use animation start, we have to play it ourselves whenever the item is actually used.
			if (!Main.dedServ && Item.UseSound.HasValue)
			{
				SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
			}

			return null;
		}

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Item.type, out var itemTexture, out var itemFrame);
            Vector2 origin = itemFrame.Size() / 2f;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);

            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPosition, itemFrame, Color.White, rotation, origin, scale, SpriteEffects.None, 0);

            return false;
        }
    }
}