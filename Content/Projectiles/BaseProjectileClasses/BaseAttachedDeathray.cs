using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insanity.Content.Projectiles.BaseProjectileClasses
{
    public abstract class BaseAttachedDeathray : ModProjectile
    {
        public abstract Texture2D DeathrayFrontTexture { get; }
        public abstract Texture2D DeathrayMiddleTexture { get; }
        public abstract Texture2D DeathrayEndTexture { get; }
        public abstract Color DeathrayColor { get; }

        public abstract int Parent { get; }

        public abstract float Alpha { get; }
        public abstract float Scale { get; }
        public abstract float TimeLeft { get; }

        public virtual string ProjectileName => "Telegraph";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(ProjectileName);
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.alpha = (int)Alpha;
            Projectile.timeLeft = (int)TimeLeft;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            behindProjectiles.Add(index);
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            Projectile ParentProj = Main.projectile[(int)Projectile.ai[1]];
            if (!ParentProj.active || ParentProj.type != Parent)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = ParentProj.Center;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            float num801 = Scale;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= TimeLeft)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / TimeLeft) * 10f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            float num804 = ParentProj.rotation - 1.57079637f + Projectile.ai[0];
            Projectile.rotation = num804;
            num804 += 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.Vortex, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.Vortex, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D18 = DeathrayFrontTexture;
            Texture2D texture2D19 = DeathrayMiddleTexture;
            Texture2D texture2D20 = DeathrayEndTexture;
            float num207 = Projectile.localAI[1];
            Microsoft.Xna.Framework.Color color40 = Color.Lerp(Color.White, DeathrayColor, 0.95f);
            color40 = Color.Lerp(color40, Color.Transparent, Alpha);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture2D18, Projectile.Center - Main.screenPosition, null, color40, Projectile.rotation, texture2D18.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            num207 -= (float)(texture2D18.Height / 2 + texture2D20.Height) * Projectile.scale;
            Vector2 center3 = Projectile.Center;
            center3 += Projectile.velocity * Projectile.scale * texture2D18.Height / 2f;
            if (num207 > 0f)
            {
                float num208 = 0f;
                Microsoft.Xna.Framework.Rectangle value12 = new Microsoft.Xna.Framework.Rectangle(0, 16 * (Projectile.timeLeft / 3 % 5), texture2D19.Width, 16);
                while (num208 + 1f < num207)
                {
                    if (num207 - num208 < (float)value12.Height)
                    {
                        value12.Height = (int)(num207 - num208);
                    }
                    Main.EntitySpriteDraw(texture2D19, center3 - Main.screenPosition, value12, color40, Projectile.rotation, new Vector2(value12.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    num208 += (float)value12.Height * Projectile.scale;
                    center3 += Projectile.velocity * value12.Height * Projectile.scale;
                    value12.Y += 16;
                    if (value12.Y + value12.Height > texture2D19.Height)
                    {
                        value12.Y = 0;
                    }
                }
            }
            Main.EntitySpriteDraw(texture2D20, center3 - Main.screenPosition, null, color40, Projectile.rotation, texture2D20.Frame().Top(), Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}
