using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Remnants.Content.Gores
{
    public class ToxicFog : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.alpha = 255;
            ChildSafety.SafeGore[gore.type] = true;
        }

        public override bool Update(Gore gore)
        {
            gore.timeLeft -= GoreID.Sets.DisappearSpeed[1202];
            if (gore.timeLeft <= 0)
            {
                gore.active = false;
                return false;
            }
            bool collision = false;
            Point point = (gore.position + new Vector2(6f * gore.scale, 3f * gore.scale)).ToTileCoordinates();
            //Dust.NewDustPerfect(gore.position + new Vector2(6f * gore.scale, 3f * gore.scale), 6);
            if (gore.timeLeft <= 240 || WorldGen.SolidTile(Main.tile[point.X, point.Y]))
            {
                collision = true;
            }
            if (!collision)
            {
                if (gore.alpha > 225 && Main.rand.NextBool(4))
                {
                    gore.alpha--;
                }
            }
            else
            {
                //if (Main.rand.NextBool(2))
                //{
                //    gore.alpha++;
                //}
                gore.alpha++;
                if (gore.alpha >= 255)
                {
                    gore.active = false;
                    return false;
                }
            }
            gore.position += gore.velocity;

            return false;
        }
    }
}