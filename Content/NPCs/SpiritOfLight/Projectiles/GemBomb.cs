using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using Insanity.Content.Particles;
using Insanity.Common.Systems.ParticleSystem;
using Insanity.Common.Utilities;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class GemBomb : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dazzling Gem");
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 46;
			Projectile.alpha = 255;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
        {
			Projectile.alpha -= 15;
			if (Projectile.alpha < 0)
            {
				Projectile.alpha = 0;
            }

			Projectile.ai[0]++;
			if (Projectile.ai[0] == Projectile.ai[1])
            {
				Projectile.Kill();
            }

			if (Projectile.localAI[0] == 0)
            {
				Projectile.localAI[0] = 1f;
				Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			}

			/*if (Projectile.ai[0] == Projectile.ai[1] - 30)
            {
				for (int i = 0; i < 6; i++)
				{
					Vector2 dir = Vector2.UnitX.RotatedBy((Math.PI * 2 / 6 * i) + Projectile.rotation);
					Vector2 vel = Vector2.Normalize(dir);
					InsanityUtils.NewProjectileBetter(Projectile.Center, vel, ModContent.ProjectileType<GemBombDeathray>(), (int)(Projectile.damage * 0.65f), 0f, default, dir.ToRotation(), Projectile.whoAmI);
				}
			}*/
        }

		public override void Kill(int timeLeft)
        {
			for (int i = 0; i < 30; i++)
			{
				Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
				Color color = Color.Lerp(Color.Pink, Color.Red, 0.65f);
				Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(2f, 5f);
				ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.15f, 0.30f), Main.rand.Next(40, 50), false, true));
			}

			InsanityUtils.NewProjectileBetter(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GemBombExplosion>(), Projectile.damage * (int)1.25f, 0f);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D bloom = Mod.Assets.Request<Texture2D>("Assets/Textures/BloomPulse").Value;
			Color bloomColor = Color.Lerp(Color.Transparent, Color.Lerp(Color.Pink, Color.Red, 0.35f), Projectile.ai[0] / Projectile.ai[1]);
			DrawHelper.DrawProjectileTexture(Projectile.whoAmI, bloom, bloomColor, Projectile.rotation, Projectile.scale * 3.15f, false, false);
			DrawHelper.DrawProjectileTexture(Projectile.whoAmI, Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value, Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, false);
			return false;
		}
	}
}
