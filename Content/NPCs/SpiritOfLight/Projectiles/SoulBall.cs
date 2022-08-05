using Insanity.Common.Systems.ParticleSystem;
using Insanity.Common.Utilities;
using Insanity.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class SoulBall : ModNPC
    {
        public override string Texture => "Terraria/Images/Projectile_871";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb of Light");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;         
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.alpha = 255;

            NPC.dontTakeDamage = true;
        }

        float rotationSpeed;

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(rotationSpeed);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            rotationSpeed = reader.ReadSingle();
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 15;
            }

            if (center == null)
            {
                NPC.life = 0;
                return;
            }

            NPC.spriteDirection = center.direction;
            NPC.timeLeft = 2;
            NPC.ai[1] += 2f * (float)Math.PI / 600f * NPC.localAI[1];
            NPC.ai[1] %= 2f * (float)Math.PI;
            NPC.Center = center.Center + NPC.localAI[0] * new Vector2((float)Math.Cos(NPC.ai[1]), (float)Math.Sin(NPC.ai[1]));

            if (NPC.ai[3] == 0f)
            {
                NPC.netUpdate = true;
                NPC.ai[3] = 1f;
                rotationSpeed = Main.rand.NextFloat(10, 180);
            }
            NPC.rotation += (float)Math.PI / rotationSpeed;

            int time = center.life < center.lifeMax * 0.65f ? 45 : 60;
            if (++NPC.ai[2] % time == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 vel = NPC.DirectionTo(player.Center);
                vel.Normalize();

                int numProj = Utils.SelectRandom(Main.rand, 4, 6, 8);
                for (int i = 0; i < numProj; i++)
                {
                    int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX.RotatedBy(((float)Math.PI * 2 / numProj * i) + NPC.rotation) * 9f, ModContent.ProjectileType<SoulBallShot>(), NPC.damage / 2, 0f, Main.myPlayer);
                    Main.projectile[p].hostile = true;
                    Main.projectile[p].friendly = false;
                }             
            }

            for (int i = 0; i < 1; i++)
            {
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2); ;
                Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(0.25f, 0.95f);
                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.35f, 0.85f), Main.rand.Next(60, 100), false, true));
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life > 0)
            {
                int numDust = Main.rand.Next(10, 20);
                for (int i = 0; i < numDust; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GemRuby, speed * 10, Scale: 1f);
                    d.noGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < 50; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                    Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GemRuby, speed * 15, Scale: Main.rand.NextFloat(1f, 3f));
                    d.noGravity = true;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
    }
}
