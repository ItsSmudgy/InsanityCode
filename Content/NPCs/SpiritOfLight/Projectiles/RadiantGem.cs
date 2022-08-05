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
    public class RadiantGem : ModProjectile
    {
        public override string Texture => "Insanity/Content/NPCs/SpiritOfLight/Projectiles/GemBomb";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Gem");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
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
            Player player = Main.player[Projectile.owner];
            Projectile.alpha -= 15;
            if (Projectile.alpha <= 0)
            {
                Projectile.alpha = 0;
            }

            if (Projectile.localAI[0] == 0f)
            {
                Vector2 pos = player.Center - Vector2.UnitY * 300;
                Projectile.velocity = Vector2.Zero.MoveTowards(pos - Projectile.Center, 16);
                Projectile.ai[1]++;
                if (Projectile.ai[1] >= 120 && Projectile.ai[1] % 30 == 0)
                {
                    int projCount = Main.masterMode ? ((Projectile.localAI[0] == 30) ? 30 : 25) : ((Projectile.localAI[0] == 60) ? 15 : 10);
                    float num = -15f;
                    float num2 = Math.Abs(num * 2f / (float)projCount - 1f);
                    float speedY = -3f;
                    for (int i = 0; i < projCount; i++)
                    {
                        float vel = Main.masterMode ? 0f : Main.rand.Next(-150, 151) * 0.01f;
                        InsanityUtils.NewProjectileBetter(Projectile.Center, new Vector2(num + num2 * (float)i + vel, speedY), ModContent.ProjectileType<RadiantGemShard>(), Projectile.damage, Projectile.knockBack, ai1: Projectile.whoAmI);
                    }
                }

                if (Projectile.ai[1] >= 420)
                {
                    Projectile.localAI[0] = 1f;
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.localAI[0] == 1f)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] <= 300)
                {
                    Vector2 pos = player.Center - Vector2.UnitY * 180;
                    Projectile.velocity = Vector2.Zero.MoveTowards(pos - Projectile.Center, 5);
                }
                else
                {
                    if (Projectile.ai[1] < 560)
                    {
                        Projectile.velocity = Vector2.Zero;
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 value6 = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(-7f, 7f);
                            Vector2 center = Projectile.Center;
                            ParticleOrchestraSettings particleOrchestraSettings = new ParticleOrchestraSettings();
                            particleOrchestraSettings.PositionInWorld = center;
                            particleOrchestraSettings.MovementVector = value6;
                            ParticleOrchestraSettings settings = particleOrchestraSettings;
                            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, Projectile.owner);
                            particleOrchestraSettings = new ParticleOrchestraSettings();
                            particleOrchestraSettings.PositionInWorld = center;
                            particleOrchestraSettings.MovementVector = value6 * 2f;
                            settings = particleOrchestraSettings;
                            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, Projectile.owner);
                        }
                    }
                    else
                    {
                        Projectile.localAI[1] += (float)Math.PI * 2 / 600;
                        for (int i = 0; i < 8; i++)
                        {
                            float offset = (float)Math.PI * 2 / 8 * i;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = Projectile.ai[1] == 620 ? ModContent.ProjectileType<RadiantGemDeathray>() : ModContent.ProjectileType<RadiantGemDeathrayTelegraph>();
                                if (Projectile.ai[1] == 560)
                                {
                                    InsanityUtils.NewProjectileBetter(Projectile.Center, Vector2.UnitX.RotatedBy(Projectile.localAI[1] + offset), type,
                                        Projectile.damage * 2, 0f, Main.myPlayer, 6 + offset, Projectile.whoAmI);
                                }
                                else if (Projectile.ai[1] == 620)
                                {
                                    InsanityUtils.NewProjectileBetter(Projectile.Center, Vector2.UnitX.RotatedBy(Projectile.localAI[1] + offset), type,
                                        Projectile.damage * 2, 0f, Main.myPlayer, 6 + offset, Projectile.whoAmI);
                                }
                            }
                        }
                    }

                    if (Projectile.ai[1] >= 1000) { Projectile.Kill(); }
                }
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
