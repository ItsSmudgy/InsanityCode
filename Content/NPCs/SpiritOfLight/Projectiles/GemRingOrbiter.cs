using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insanity.Common.Systems.ParticleSystem;
using Insanity.Content.Particles;
using System;
using Insanity.Common.Utilities;
using System.IO;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class GemRingOrbiter : ModProjectile
    {
        public override string Texture => "Insanity/Content/NPCs/SpiritOfLight/Projectiles/GemBomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orbiting Gem");
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.alpha = 255;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;

            Projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Projectile center = Main.projectile[(int)Projectile.ai[0]];
            if (!center.active)
            {
                Projectile.Kill();
            }

            if (Projectile.velocity.X != 0f)
            {
                Projectile.localAI[0] = Projectile.velocity.X;
                Projectile.velocity.X = 0f;
            }
            if (Projectile.velocity.Y != 0f)
            {
                Projectile.localAI[1] = Projectile.velocity.Y;
                Projectile.velocity.Y = 0f;
            }

            Projectile.spriteDirection = center.direction;
            Projectile.timeLeft = 2;
            Projectile.ai[1] += 2f * (float)Math.PI / 600f * Projectile.localAI[1];
            Projectile.ai[1] %= 2f * (float)Math.PI;
            Projectile.Center = center.Center + Projectile.localAI[0] * new Vector2((float)Math.Cos(Projectile.ai[1]), (float)Math.Sin(Projectile.ai[1]));
            if (Projectile.alpha > 0)
                Projectile.alpha -= 20;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                Color color = Color.Lerp(Color.Pink, Color.Red, 0.35f);
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(2f, 5f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.15f, 0.30f), Main.rand.Next(40, 50), false, true));
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}
