using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Items.Consumable
{
    public class FragmentGlowstick : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Glowstick);
            Item.shoot = ModContent.ProjectileType<Projectiles.FragmentGlowstick>();
        }

        public override void HoldItem(Player player)
        {
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 200f / 255f, 120f / 255f, 0);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(Item.type, out var itemTexture, out var itemFrame);
            Vector2 origin = itemFrame.Size() / 2f;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);

            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPosition, itemFrame, Color.White, rotation, origin, scale, SpriteEffects.None, 0);

            return false;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, 200f / 255f, 120f / 255f, 0);
        }
    }
}