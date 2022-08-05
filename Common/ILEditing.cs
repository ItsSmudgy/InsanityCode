using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Insanity.Common.Systems.ParticleSystem;

namespace Insanity.Common
{
    public static class ILEditing
    {
        public static void Initialize()
        {
            On.Terraria.Main.DrawInterface += DrawParticles;
        }

        public static void Unload()
        {
            On.Terraria.Main.DrawInterface -= DrawParticles;
        }

        private static void DrawParticles(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();

            orig(self, gameTime);
        }
    }
}
