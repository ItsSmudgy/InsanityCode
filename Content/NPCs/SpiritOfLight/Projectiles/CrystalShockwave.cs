using Insanity.Content.Projectiles.BaseProjectileClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class CrystalShockwave : BaseVisualExplosionProjectile
    {
        public override string Texture => "Insanity/Assets/Textures/BloomPulse";
        public override int TimeLeft => 60;
        public override float MaxScale => Projectile.ai[1];
        public override Color GetShockwaveColor => Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Shockwave");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 96;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }
    }
}
