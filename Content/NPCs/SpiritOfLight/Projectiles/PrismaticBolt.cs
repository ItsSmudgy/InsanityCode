using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insanity.Common.Systems.ParticleSystem;
using Insanity.Content.Particles;
using System;
using Insanity.Common.Utilities;
using Terraria.GameContent;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class PrismaticBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_644";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Bolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.aiStyle = -1;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

        public override void AI()
        {
			bool flag = false;
			bool flag2 = false;
			float num = 100f;
			float num2 = 30f;
			float num3 = 0.98f;
			float value = 0.05f;
			float value2 = 0.1f;
			float scaleFactor = 30f;
			
			if ((float)Projectile.timeLeft > num)
			{
				flag = true;
			}
			else if ((float)Projectile.timeLeft > num2)
			{
				flag2 = true;
			}
			if (flag)
			{
				float num5 = (float)Math.Cos((float)Projectile.whoAmI % 6f / 6f + Projectile.position.X / 320f + Projectile.position.Y / 160f);
				Projectile.velocity *= num3;
				Projectile.velocity = Projectile.velocity.RotatedBy(num5 * ((float)Math.PI * 2f) * 0.125f * 1f / 30f);
			}
			if (flag2)
			{
				int num8 = (int)Projectile.ai[0];
				Vector2 value3 = Projectile.velocity;
				if (Projectile.hostile && Main.player.IndexInRange(num8))
				{
					Player player = Main.player[num8];
					value3 = Projectile.DirectionTo(player.Center) * scaleFactor;
				}				
				float amount = MathHelper.Lerp(value, value2, Utils.GetLerpValue(num, 30f, Projectile.timeLeft, clamped: true));
				Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, value3, amount);
			}
			Projectile.Opacity = Utils.GetLerpValue(120f, 100f, Projectile.timeLeft, clamped: true);
			Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
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
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			DrawHelper.DrawProjectileTexture(Projectile.whoAmI, texture, Projectile.GetAlpha(new Color(255, 141, 211)), Projectile.rotation, Projectile.scale, false, false);
			DrawHelper.DrawProjectileTrail(Projectile.whoAmI, texture, Projectile.GetAlpha(new Color(255, 141, 211)), Projectile.rotation, true, Projectile.scale, false, true, true);
			return false;
        }
    }
}
