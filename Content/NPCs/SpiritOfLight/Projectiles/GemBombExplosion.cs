using Insanity.Content.Projectiles.BaseProjectileClasses;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Drawing;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class GemBombExplosion : BaseExplosionProjectile
    {
        public override float MinScale => 3f;
        public override float MaxScale => 3.15f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Dazzling Explosion");
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 value6 = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(-7f, 7f);
                Vector2 center = Projectile.Center;
                ParticleOrchestraSettings particleOrchestraSettings = new ParticleOrchestraSettings();
                particleOrchestraSettings.PositionInWorld = center;
                particleOrchestraSettings.MovementVector = value6;
                ParticleOrchestraSettings settings = particleOrchestraSettings;
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, Projectile.owner);
                particleOrchestraSettings = new ParticleOrchestraSettings();
                particleOrchestraSettings.PositionInWorld = center;
                particleOrchestraSettings.MovementVector = value6 * 2f;
                settings = particleOrchestraSettings;
                ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, Projectile.owner);
            }
        }
    }
}
