using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Content.Items.Materials;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Remnants.Content.Projectiles
{
	public class FallingDreampod : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 600;
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.3f;
			//Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ShimmerTorch);
			//dust.velocity = Projectile.velocity;
			Lighting.AddLight(Projectile.Center, new Vector3(255f / 255f, 119f / 255f, 232f / 255f));

			if (Projectile.shimmerWet)
			{
                for (int i = 0; i < 100; i++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.ShimmerTorch);
                    dust.velocity = new Vector2(Main.rand.NextFloat(-5, 5), -Main.rand.NextFloat(0, 5));
                }

                Projectile.Kill();
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			for (int i = 0; i < 40; i++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.ShimmerSplash);
				dust.velocity = new Vector2(Main.rand.NextFloat(-oldVelocity.Y, oldVelocity.Y) / 2, -Main.rand.NextFloat(0, oldVelocity.Y)) / 2;
			}

			Projectile.Kill();

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			int item = Item.NewItem(new EntitySource_Loot(Projectile), Projectile.getRect(), new Item(ModContent.ItemType<DreamJelly>(), Main.rand.Next(1, 4)));
			if (Projectile.shimmerWet)
			{
				Main.item[item].velocity = Vector2.Zero;
                SoundEngine.PlaySound(SoundID.NPCDeath56, Projectile.position);
            }
            else SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            NetMessage.SendData(MessageID.SyncItem, number: item);
		}

        public override bool PreDraw(ref Color lightColor)
        {
			lightColor = Color.White;
			return true;
        }
    }
}