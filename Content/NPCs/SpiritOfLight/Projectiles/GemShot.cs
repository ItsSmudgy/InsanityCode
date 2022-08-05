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
    public class GemShot : ModProjectile
    {
        public override string Texture => "Insanity/Assets/Textures/BlankSprite";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gem Bolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.alpha = 255;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 20;

            for (int i = 0; i < 1; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2); ;
                Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.03f, 0.20f), Main.rand.Next(60, 100), false, true));
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.15f, 0.30f), Main.rand.Next(40, 50), false, true));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawProjectileTrail(Projectile.whoAmI, Mod.Assets.Request<Texture2D>("Assets/Textures/BloomTrailSmall").Value, Projectile.GetAlpha(new Color(255, 141, 211)), Projectile.rotation, true, Projectile.scale, false, true, true);
            return true;
        }
    }
}
