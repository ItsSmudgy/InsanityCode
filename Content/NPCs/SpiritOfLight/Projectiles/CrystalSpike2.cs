using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class CrystalSpike2 : ModProjectile
    {
        public override string Texture => "Insanity/Content/NPCs/SpiritOfLight/Projectiles/CrystalSpike";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Spike");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int num = 6;
            int num2 = 10;
            int num3 = 30;
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = Main.rand.NextFloat(0.35f, 0.65f);
                Projectile.netUpdate = true;
            }

            Projectile.ai[0]++;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.frame = Main.rand.Next(5);
                SoundStyle sound = SoundID.DeerclopsIceAttack;
                sound.Volume = 0.2f;
                SoundEngine.PlaySound(sound, Projectile.Center);
            }

            if (Projectile.ai[0] < num)
            {
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.2f, 0f, 1f);
                Projectile.scale = Projectile.Opacity * Projectile.ai[1];
            }

            if (Projectile.ai[0] > num3 - num2)
            {
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity - 0.2f, 0f, 1f);
            }

            if (Projectile.ai[0] >= num3)
            {
                Projectile.Kill();
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint7 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200f * Projectile.scale, 22f * Projectile.scale, ref collisionPoint7))
            {
                return true;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value182 = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle value183 = value182.Frame(1, 5, 0, Projectile.frame);
            Vector2 origin11 = new Vector2(16f, (float)(value183.Height / 2));
            Color alpha15 = Projectile.GetAlpha(lightColor);
            Vector2 vector33 = new Vector2(Projectile.scale);
            float lerpValue5 = Utils.GetLerpValue(30f, 25f, Projectile.ai[0], true);
            vector33.Y *= lerpValue5;
            Vector4 vector34 = lightColor.ToVector4();
            Color val7 = new Color(67, 17, 17);
            Vector4 vector35 = val7.ToVector4();
            vector35 *= vector34;
            Main.EntitySpriteDraw(TextureAssets.Extra[98].Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) - Projectile.velocity * Projectile.scale * 0.5f, null, Projectile.GetAlpha(new Color(vector35.X, vector35.Y, vector35.Z, vector35.W)) * 1f, Projectile.rotation + (float)Math.PI / 2f, TextureAssets.Extra[98].Value.Size() / 2f, Projectile.scale * 0.9f, SpriteEffects.None, 0);
            Color color46 = Projectile.GetAlpha(Color.White) * Utils.Remap(Projectile.ai[0], 0f, 20f, 0.5f, 0f);
            color46.A = (byte)0;
            for (int num189 = 0; num189 < 4; num189++)
            {
                Vector2 val11 = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Vector2 spinningpoint20 = Projectile.rotation.ToRotationVector2();
                double radians9 = (float)Math.PI / 2f * (float)num189;
                Vector2 val2 = default(Vector2);
                Main.EntitySpriteDraw(value182, val11 + spinningpoint20.RotatedBy(radians9, val2) * 2f * vector33, value183, color46, Projectile.rotation, origin11, vector33, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(value182, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), value183, alpha15, Projectile.rotation, origin11, vector33, SpriteEffects.None, 0);
            return false;
        }
    }
}
