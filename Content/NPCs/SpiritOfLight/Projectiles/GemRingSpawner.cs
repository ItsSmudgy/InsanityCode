using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insanity.Common.Systems.ParticleSystem;
using Insanity.Content.Particles;
using System;
using Insanity.Common.Utilities;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class GemRingSpawner : ModProjectile
    {
        public override string Texture => "Insanity/Assets/Textures/BloomLight";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gem Bolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] == Projectile.ai[1])
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] >= 120)
            {
                Projectile.velocity *= 0.9f;
            }

            if (Projectile.ai[0] == 1f)
            {
                InsanityUtils.SpawnProjectileRotatersForProjectile(Projectile.whoAmI, Projectile.width + 50, 2f, ModContent.ProjectileType<GemRingOrbiter>(), Projectile.damage, 8, true);
                if (Main.masterMode)
                {
                    InsanityUtils.SpawnProjectileRotatersForProjectile(Projectile.whoAmI, Projectile.width + 100, 2f, ModContent.ProjectileType<GemRingOrbiter>(), Projectile.damage, 8, false);
                }
            }

            if (Main.rand.NextBool(15))
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2); ;
                    Color color = Color.Lerp(Color.Pink, Color.Red, 0.35f);
                    Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                    ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.45f, 0.85f), Main.rand.Next(60, 70), false, true));
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                Color color = Color.Lerp(Color.Pink, Color.Red, 0.35f);
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.15f, 0.30f), Main.rand.Next(60, 80), false, true));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawProjectileTexture(Projectile.whoAmI, Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value, Projectile.GetAlpha(Color.Lerp(Color.Pink, Color.Red, 0.35f)), Projectile.rotation, useAdditiveBlend: true, useAlphaBlend: true);
            DrawHelper.DrawProjectileTrail(Projectile.whoAmI, Mod.Assets.Request<Texture2D>("Assets/Textures/BloomTrailSmall").Value, Projectile.GetAlpha(Color.Lerp(Color.Pink, Color.Red, 0.35f)), Projectile.rotation, true, Projectile.scale, false, true, true);
            return true;
        }
    }
}
