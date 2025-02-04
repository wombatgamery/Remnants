using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remnants.Projectiles.Enemy;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Remnants.Tiles.Objects.Hazards
{
    public class Spitflower : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;

			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.addTile(Type);

			DustType = DustID.Poisoned;
			HitSound = SoundID.NPCHit1;

			AddMapEntry(new Color(235, 81, 54), CreateMapEntryName());
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Point16 origin = TileUtils.GetTileOrigin(i, j);
			ModContent.GetInstance<TESpitflower>().Kill(origin.X, origin.Y);
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return false;
        }

        public override bool KillSound(int i, int j, bool fail)
        {
			if (!fail)
            {
				SoundEngine.PlaySound(SoundID.NPCDeath1, new Vector2(i, j) * 16 + Vector2.One * 8);
			}
            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			if (TileUtils.TryGetTileEntityAs(i, j, out TESpitflower entity))
            {
				Tile tile = Main.tile[i, j];
				if (tile.TileFrameY == 0)
				{
					Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
					if (Main.drawToScreen)
					{
						zero = Vector2.Zero;
					}

					Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(i * 16 - (int)Main.screenPosition.X + 7, j * 16 - (int)Main.screenPosition.Y + 7) + zero + Vector2.Normalize(entity.direction) * entity.drawAhead/4, new Rectangle(0, 0, 30, 42), Lighting.GetColor(i, j), entity.rotation - MathHelper.PiOver2, new Vector2(15, 15), 1f, SpriteEffects.None, 0f);
				}
			}
		}
	}

	public class TESpitflower : ModTileEntity
	{
		public override bool IsTileValidForEntity(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			return tile.HasTile && tile.TileType == ModContent.TileType<Spitflower>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
		}

		int attackTimer;
		public Vector2 direction = Vector2.UnitY;
		public float rotation = 0;
		public float drawAhead = 0;

		public override void Update()
		{
			int targetPlayer = -1;
			float closestDistance = 64 * 16;

			Vector2 tilePos = new Vector2(Position.X * 16 + 7, Position.Y * 16 + 7);

			for (int k = 0; k < Main.maxPlayers; k++)
			{
				Player player = Main.player[k];

				if (!player.active)
				{
					break;
				}
				else if (!player.DeadOrGhost && player.Center.Y >= tilePos.Y && Collision.CanHit(Position.ToVector2() * 16, 16, 16, player.position, player.width, player.height))
				{
					float distance = Vector2.Distance(tilePos, player.Center);
					if (distance <= closestDistance)
					{
						targetPlayer = player.whoAmI;
						closestDistance = distance;
					}
				}
			}

			if (targetPlayer != -1)
			{
				Player target = Main.player[targetPlayer];
				direction = target.Center - tilePos;

				if (attackTimer <= 0)
				{
					int proj = Projectile.NewProjectile(Projectile.GetSource_None(), tilePos, Vector2.Normalize(direction) * 12, ModContent.ProjectileType<PoisonSpit>(), 17, 0f);
					NetMessage.SendData(MessageID.SyncProjectile, number: proj);

					SoundEngine.PlaySound(SoundID.Item17, Position.ToVector2() * 16 + Vector2.One * 8);

					drawAhead = 4 * 4;
					attackTimer = 60;
				}
				else attackTimer--;
			}
			else attackTimer = 60;

			if (drawAhead > 0)
            {
				drawAhead--;
			}

			rotation = MathHelper.Lerp(targetPlayer != -1 ? direction.ToRotation() : MathHelper.PiOver2, rotation, 0.8f);
		}
	}
}