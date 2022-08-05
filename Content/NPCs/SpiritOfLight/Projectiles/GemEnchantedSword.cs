using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class GemEnchantedSword : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_946";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Blade");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 94;
            Projectile.height = 30;
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
            Player target = Main.player[(Player.FindClosest(Projectile.Center, 0, 0))];
            if (Projectile.alpha > 0)
                Projectile.alpha -= 20;

            Projectile.ai[0]--;
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.98f;
            }
            else
            {
                Projectile.ai[1]--;
                if (Projectile.ai[1] == 0)
                {
                    if (target != null)
                    {
                        Projectile.velocity = Projectile.DirectionTo(target.Center) * 20;
                        Projectile.netUpdate = true;
                    }
                }

                Vector2 vector = Vector2.Lerp(target.Center, Projectile.Center, 0.5f);
                float num = (target.Center - vector).ToRotation();
                if (Projectile.ai[1] == 0)
                {
                    Projectile.rotation = Projectile.AngleTo(target.Center) + (float)Math.PI / 2;
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Common.Utilities.DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Lerp(Color.Pink, Color.Red, 0.65f)), Projectile.rotation, 1f, true, false);
            Common.Utilities.DrawHelper.DrawProjectileTrail(Projectile.whoAmI, texture, Projectile.GetAlpha(Color.Lerp(Color.Pink, Color.Red, 0.65f)), Projectile.rotation, true, Projectile.scale, false, true, true);
            return false;
        }
    }
}
