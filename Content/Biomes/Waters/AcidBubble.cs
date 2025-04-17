using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Biomes.Waters
{
	public class AcidBubble : ModGore
	{
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
			gore.numFrames = 4;
			gore.behindTiles = true;
			gore.timeLeft = Gore.goreTime * 3;
		}

		public override bool Update(Gore gore)
		{
			gore.alpha = gore.position.Y < Main.worldSurface * 16.0 + 8.0
				? 0
				: 100;

			int frameDuration = 4;
			gore.frameCounter++;

			frameDuration = 16;
			if (gore.frameCounter >= frameDuration)
			{
				gore.frameCounter = 0;
				gore.frame++;
				if (gore.frame == 4)
				{
					for (int i = 0; i < Main.rand.Next(4, 7); i++)
                    {
						int dust = Dust.NewDust(gore.position, 16, 16, ModContent.Find<ModWaterStyle>("Remnants/Acid").GetSplashDust());
						Main.dust[dust].velocity = Main.rand.NextVector2Circular(4, 4);
						if (Main.dust[dust].velocity.Y > 0)
                        {
							Main.dust[dust].velocity.Y *= -1;
						}
					}
					SoundEngine.PlaySound(SoundID.Item54, new Vector2((int)gore.position.X + 8, (int)gore.position.Y + 8));
					gore.active = false;
				}
			}

			return false;
		}
	}
}