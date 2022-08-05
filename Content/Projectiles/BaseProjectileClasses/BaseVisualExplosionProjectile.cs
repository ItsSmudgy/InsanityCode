using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Insanity.Common.Utilities;

namespace Insanity.Content.Projectiles.BaseProjectileClasses
{
    public abstract class BaseVisualExplosionProjectile : ModProjectile
    {
        //NOTES:
        //remember to use BlankSprite texture when using the pillar shield shader !!!
        public override string Texture => "Insanity/Assets/Textures/BlankSprite";

        public ref float CurrentRadius => ref Projectile.ai[0];
        public ref float MaxRadius => ref Projectile.ai[1];

        public abstract float MaxScale { get; }
        public abstract int TimeLeft { get; }
        public abstract Color GetShockwaveColor { get; }
        public virtual bool UsePillarShieldShader { get; }

        public override void AI()
        {
            if (++Projectile.localAI[0] > TimeLeft)
            {
                Projectile.Kill();
                return;
            }
            CurrentRadius = MathHelper.Lerp(CurrentRadius, MaxRadius, Projectile.localAI[0] / TimeLeft);
            Projectile.scale = MathHelper.Lerp(Projectile.scale, MaxScale, Projectile.localAI[0] / TimeLeft);
            Projectile.alpha = (int)(255f * Projectile.localAI[0] / TimeLeft);
            InsanityUtils.ExpandProjectileHitbox(Projectile, (int)(CurrentRadius * Projectile.scale), (int)(CurrentRadius * Projectile.scale));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (UsePillarShieldShader)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                Vector2 vector = new Vector2(1.5f, 1f);
                Vector2 position = Projectile.Center - Main.screenPosition + Projectile.Size * vector * 0.5f;
                DrawData value45 = new DrawData(Main.Assets.Request<Texture2D>("Images/Misc/Perlin").Value, position, (Rectangle?)new Rectangle(0, 0, Projectile.width, Projectile.height), Projectile.GetAlpha(GetShockwaveColor), Projectile.rotation, Projectile.Size, vector, SpriteEffects.None, 0);
                GameShaders.Misc["ForceField"].UseColor(GetShockwaveColor);
                GameShaders.Misc["ForceField"].Apply(value45);
                value45.Draw(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            }
            else
            {
                DrawHelper.DrawProjectileTexture(Projectile.whoAmI, TextureAssets.Projectile[Projectile.type].Value, Projectile.GetAlpha(GetShockwaveColor), Projectile.rotation, Projectile.scale, false, false, true, true);
            }
            return false;
        }
    }
}
