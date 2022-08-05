using Insanity.Content.Projectiles.BaseProjectileClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class CrystalShockwave2 : BaseVisualExplosionProjectile
    {
        public override string Texture => "Insanity/Assets/Textures/BlankSprite";
        public override int TimeLeft => 200;
        public override float MaxScale => 15f;

        public override bool UsePillarShieldShader => true;
        public override Color GetShockwaveColor => Color.Lerp(Color.White, new Color(255, 141, 211), 0.85f);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Insurgency Shockwave");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override void PostAI()
        {
            MaxRadius = 350f;
        }
    }
}
