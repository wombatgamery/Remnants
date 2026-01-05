using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;

namespace Remnants.Content.Biomes
{
    public class SulfuricVentsSky : CustomSky
    {
        private bool isActive = false;

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            //if (maxDepth >= 0 && minDepth < 0)
            //{
            //    float intensity = RemSystem.exhaustIntensity * 0.5f;
            //    spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), new Color(0, 0, 0, intensity));
            //}
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive;
        }
    }
}