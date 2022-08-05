using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Insanity.Content.Projectiles.BaseProjectileClasses
{
    public abstract class BaseExplosionProjectile : ModProjectile
    {
        public virtual float MinScale => 1f;
        public virtual float MaxScale => 2f;

        public virtual Color ProjColor => Color.White;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LunarFlare];
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.scale = Main.rand.NextFloat(MinScale, MaxScale);

                Projectile.position = Projectile.Center;
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = Projectile.position;
            }

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ProjColor * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y = num * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, y, texture2D13.Width, num);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(drawColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
