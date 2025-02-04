using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Projectiles.Enemy
{
	public class IceSpike : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 8 * 2;
			Projectile.height = 24 * 2;
			Projectile.aiStyle = -1;             //The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = false;         //Can the projectile deal damage to enemies?
			Projectile.hostile = false;         //Can the projectile deal damage to the player?
			Projectile.penetrate = -1;           //How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 60 * 3;          //The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
		}

		public override void OnSpawn(IEntitySource source)
		{
			if (WorldGen.SolidTile((int)(Projectile.position.X + 8) / 16, (int)(Projectile.position.Y / 16), true))
            {
				Projectile.active = false;
            }
			else
            {
				while (!WorldGen.SolidTile((int)(Projectile.position.X + 8) / 16, (int)(Projectile.position.Y / 16), true))
				{
					Projectile.position.Y++;
					if ((int)(Projectile.position.Y / 16) >= Main.maxTilesY - 40)
					{
						Projectile.active = false;
					}
				}
				Projectile.frame = Main.rand.Next(3);
			}
        }

		int timer = 0;

        public override void AI()
		{
			Dust dust;

			timer++;
			if (timer >= 60)
            {
				if (timer == 60)
				{
                    Projectile.alpha = 0;
                    Projectile.position.Y -= 48;
                    Projectile.hostile = true;

                    for (int i = 0; i < 20; i++)
                    {
                        dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Ice);
                        dust.velocity = Main.rand.NextVector2Circular(2, 2);
                        dust.velocity.Y -= 2;
                    }

                    SoundEngine.PlaySound(SoundID.NPCDeath15, Projectile.position);
                }

				dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch);
				dust.noGravity = true;
                dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y + Projectile.height - 2.5f), Projectile.width, 0, DustID.Cloud);
                dust.noGravity = true;
                dust.velocity *= 0.25f;
            }
			else
			{
				dust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 6, Projectile.position.Y - 2), 4, 4, DustID.IceTorch);
				dust.noGravity = true;
				dust.velocity.Y = -Main.rand.NextFloat(5f);
            }
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.Chilled, 10 * 60);
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 20; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch);
				dust.noGravity = true;
				dust.velocity = Main.rand.NextVector2Circular(2, 2);
				dust.velocity.Y -= 2;
			}
        }

   //     public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
   //     {
			//overPlayers.Add(index);
   //     }
    }
}