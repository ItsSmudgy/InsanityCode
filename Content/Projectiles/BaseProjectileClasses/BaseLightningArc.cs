using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Insanity.Content.Projectiles.BaseProjectileClasses
{
    public abstract class BaseLightningArc : ModProjectile
    {
		public virtual bool Splits { get; }

		public abstract float Scale { get; }

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.alpha = 0;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 4;
			Projectile.timeLeft = 600;
			Projectile.scale = Scale;
		}

		public override void AI()
		{
			if (Projectile.localAI[1] == 0f && Projectile.ai[0] >= 900f)
			{
				Projectile.ai[0] -= 1000f;
				Projectile.localAI[1] = -1f;
			}
			Projectile.frameCounter++;
			Lighting.AddLight(Projectile.Center, 0.3f, 0.45f, 0.5f);
			if (Projectile.velocity == Vector2.Zero)
			{
				if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
				{
					Projectile.frameCounter = 0;
					bool flag36 = true;
					for (int num884 = 1; num884 < Projectile.oldPos.Length; num884++)
					{
						if (Projectile.oldPos[num884] != Projectile.oldPos[0])
						{
							flag36 = false;
						}
					}
					if (flag36)
					{
						Projectile.Kill();
						return;
					}
				}
				if (Main.rand.Next(Projectile.extraUpdates) == 0 && (Projectile.velocity != Vector2.Zero || Main.rand.Next((Projectile.localAI[1] == 2f) ? 2 : 6) == 0))
				{
					for (int num885 = 0; num885 < 2; num885++)
					{
						float num886 = Projectile.rotation + ((Main.rand.Next(2) == 1) ? (-1f) : 1f) * ((float)Math.PI / 2f);
						float num887 = (float)Main.rand.NextDouble() * 0.8f + 1f;
						Vector2 vector111 = new Vector2((float)Math.Cos(num886) * num887, (float)Math.Sin(num886) * num887);
						int num888 = Dust.NewDust(Projectile.Center, 0, 0, 226, vector111.X, vector111.Y);
						Main.dust[num888].noGravity = true;
						Main.dust[num888].scale = 1.2f;
					}
					if (Main.rand.Next(5) == 0)
					{
						Vector2 vector112 = Projectile.velocity.RotatedBy(1.5707963705062866) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
						int num889 = Dust.NewDust(Projectile.Center + vector112 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default(Microsoft.Xna.Framework.Color), 1.5f);
						Dust dust129 = Main.dust[num889];
						Dust dust2 = dust129;
						dust2.velocity *= 0.5f;
						Main.dust[num889].velocity.Y = 0f - Math.Abs(Main.dust[num889].velocity.Y);
					}
				}
			}
			else
			{
				if (Projectile.frameCounter < Projectile.extraUpdates * 2)
				{
					return;
				}
				Projectile.frameCounter = 0;
				float num890 = Projectile.velocity.Length();
				UnifiedRandom unifiedRandom2 = new UnifiedRandom((int)Projectile.ai[1]);
				int num891 = 0;
				Vector2 spinningpoint15 = -Vector2.UnitY;
				while (true)
				{
					int num892 = unifiedRandom2.Next();
					Projectile.ai[1] = num892;
					num892 %= 100;
					float f2 = (float)num892 / 100f * ((float)Math.PI * 2f);
					Vector2 vector113 = f2.ToRotationVector2();
					if (vector113.Y > 0f)
					{
						vector113.Y *= -1f;
					}
					bool flag37 = false;
					if (vector113.Y > -0.02f)
					{
						flag37 = true;
					}
					if (vector113.X * (float)(Projectile.extraUpdates + 1) * 2f * num890 + Projectile.localAI[0] > 40f)
					{
						flag37 = true;
					}
					if (vector113.X * (float)(Projectile.extraUpdates + 1) * 2f * num890 + Projectile.localAI[0] < -40f)
					{
						flag37 = true;
					}
					if (flag37)
					{
						if (num891++ >= 100)
						{
							Projectile.velocity = Vector2.Zero;
							if (Projectile.localAI[1] < 1f)
							{
								Projectile.localAI[1] += 2f;
							}
							break;
						}
						continue;
					}
					spinningpoint15 = vector113;
					break;
				}
				if (!(Projectile.velocity != Vector2.Zero))
				{
					return;
				}
				Projectile.localAI[0] += spinningpoint15.X * (float)(Projectile.extraUpdates + 1) * 2f * num890;
				Projectile.velocity = spinningpoint15.RotatedBy(Projectile.ai[0] + (float)Math.PI / 2f) * num890;
				Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;

				if (Splits)
                {
					if (Main.rand.Next(4) == 0 && Main.netMode != NetmodeID.MultiplayerClient && Projectile.localAI[1] == 0f)
					{
						float num893 = (float)Main.rand.Next(-3, 4) * ((float)Math.PI / 3f) / 3f;
						Vector2 vector114 = Projectile.ai[0].ToRotationVector2().RotatedBy(num893) * Projectile.velocity.Length();
						if (!Collision.CanHitLine(Projectile.Center, 0, 0, Projectile.Center + vector114 * 50f, 0, 0))
						{
							Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X - vector114.X, Projectile.Center.Y - vector114.Y, vector114.X, vector114.Y, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, vector114.ToRotation() + 1000f, Projectile.ai[1]);
						}
					}
				}
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Rectangle myRect = projHitbox;
			Rectangle targetRect = targetHitbox;
			for (int i = 0; i < Projectile.oldPos.Length && (Projectile.oldPos[i].X != 0f || Projectile.oldPos[i].Y != 0f); i++)
			{
				myRect.X = (int)Projectile.oldPos[i].X;
				myRect.Y = (int)Projectile.oldPos[i].Y;
				if (myRect.Intersects(targetRect))
				{
					return true;
				}
			}
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = Vector2.Zero;
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 end = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
			Texture2D tex3 = Terraria.GameContent.TextureAssets.Extra[33].Value;
			Projectile.GetAlpha(lightColor);
			Vector2 vector67 = new Vector2(Projectile.scale) / 2f;
			for (int num278 = 0; num278 < 3; num278++)
			{
				switch (num278)
				{
					case 0:
						vector67 = new Vector2(Projectile.scale) * 0.6f;
						DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(115, 204, 219, 0) * 0.5f;
						break;
					case 1:
						vector67 = new Vector2(Projectile.scale) * 0.4f;
						DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(113, 251, 255, 0) * 0.5f;
						break;
					default:
						vector67 = new Vector2(Projectile.scale) * 0.2f;
						DelegateMethods.c_1 = new Microsoft.Xna.Framework.Color(255, 255, 255, 0) * 0.5f;
						break;
				}
				DelegateMethods.f_1 = 1f;
				for (int num279 = Projectile.oldPos.Length - 1; num279 > 0; num279--)
				{
					if (!(Projectile.oldPos[num279] == Vector2.Zero))
					{
						Vector2 start = Projectile.oldPos[num279] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
						Vector2 end2 = Projectile.oldPos[num279 - 1] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
						Utils.DrawLaser(Main.spriteBatch, tex3, start, end2, vector67, DelegateMethods.LightningLaserDraw);
					}
				}
				if (Projectile.oldPos[0] != Vector2.Zero)
				{
					DelegateMethods.f_1 = 1f;
					Vector2 start2 = Projectile.oldPos[0] + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
					Utils.DrawLaser(Main.spriteBatch, tex3, start2, end, vector67, DelegateMethods.LightningLaserDraw);
				}
			}
			return false;
		}
	}
}
