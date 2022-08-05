using Insanity.Common.Systems.ParticleSystem;
using Insanity.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insanity.Content.Projectiles.BaseProjectileClasses
{
    public abstract class BaseDeathrayProjectile : ModProjectile
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

        public virtual bool UseAdditiveBlending => true;
        public virtual bool UseAlphaBlend => true;

        public virtual bool SpawnParticles => true;
        public virtual float ParticleSpeed => Main.rand.NextFloat(1, 10);
        public virtual Vector2 ParticleVelocity => Vector2.UnitX.RotatedBy(Math.PI * 2) * ParticleSpeed;
        public virtual int ParticleAmount => 2;
        public virtual float ParticleScale => 3f;
        public virtual int ParticleMaxTime => 60;
        public virtual Color ParticleColor => Color.White;

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
            NPC ParentProj = Main.npc[(int)Projectile.ai[1]];
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
            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0];
            Projectile.rotation = num804 - (float)Math.PI / 2f;
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
            if (SpawnParticles)
            {
                for (int i = 0; i < ParticleAmount; i++)
                {
                    Vector2 spawnPos = vector79;
                    Color color = ParticleColor;
                    Vector2 vel = ParticleVelocity;
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, ParticleScale, ParticleMaxTime, false, true));
                }
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

            if (UseAdditiveBlending)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }

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

            if (UseAlphaBlend)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            }
            
            return false;
        }
    }
}
