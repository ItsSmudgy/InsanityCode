using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insanity.Common.Systems.ParticleSystem;
using Insanity.Content.Particles;
using System;
using System.IO;
using Insanity.Common.Utilities;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class RadiantGemShard : ModProjectile
    {
        public override string Texture => "Insanity/Content/NPCs/SpiritOfLight/Projectiles/CrystalBombShard";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Crystal Shard");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 2100;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
            Projectile.alpha -= 15;
            if (Projectile.alpha <= 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.velocity.X *= 0.99f;
            Projectile.velocity.Y += 0.075f;
            if (Projectile.velocity.Y > 3)
            {
                Projectile.velocity.Y = 3;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Common.Utilities.DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.White), Projectile.rotation, 1f, true, false);
            Common.Utilities.DrawHelper.DrawProjectileTrail(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.White), Projectile.rotation, true, Projectile.scale, false, true, true);
            return false;
        }
    }
}
