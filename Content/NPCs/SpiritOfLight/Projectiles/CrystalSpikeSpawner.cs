using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class CrystalSpikeSpawner : ModProjectile
    {
        public override string Texture => "Insanity/Assets/Textures/BlankSprite";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Spike Spawner");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.Kill();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float num = Math.Sign(Projectile.velocity.X);
                Vector2 velocity = -Vector2.UnitY.RotatedBy(num * 0.7f + Main.rand.NextFloatDirection() * (float)Math.PI / 10f);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<CrystalSpike2>(), Projectile.damage, 0f);
            }
        }
    }
}
