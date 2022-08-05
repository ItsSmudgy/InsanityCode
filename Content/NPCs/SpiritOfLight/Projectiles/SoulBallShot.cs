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
    public class SoulBallShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_520";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of Light");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.04f;
            if (Projectile.alpha > 0)
                Projectile.alpha -= 20;

            for (int i = 0; i < 1; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2); ;
                Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.03f, 0.15f), Main.rand.Next(60, 70), false, true));
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
            DrawHelper.DrawProjectileTrail(Projectile.whoAmI, Mod.Assets.Request<Texture2D>("Assets/Textures/BloomTrailSmall").Value, Projectile.GetAlpha(Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f))), Projectile.rotation, true, Projectile.scale, false, true, true);
            return false;
        }
    }
}
