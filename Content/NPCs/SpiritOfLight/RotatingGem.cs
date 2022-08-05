using Insanity.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insanity.Content.NPCs.SpiritOfLight
{
    public class RotatingGem : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiant Gem");
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new NPCDebuffImmunityData
            {
               SpecificallyImmuneTo = new int[]
               {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    BuffID.Frostburn,
                    BuffID.Poisoned,
               }
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 40;
            NPC.defense = 15;
            NPC.lifeMax = 7500;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
            NPC.scale = 0.85f;
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
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

            if (++NPC.ai[2] % Main.rand.Next(50, 60) == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 vel = NPC.DirectionTo(player.Center);
                vel.Normalize();

                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel * 7f, ModContent.ProjectileType<Projectiles.GemShot>(), NPC.damage / 2, 0f, Main.myPlayer);
                Main.projectile[p].hostile = true;
                Main.projectile[p].friendly = false;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            DrawHelper.DrawNPCTexture(NPC.whoAmI, texture, Color.White, NPC.rotation, 1, false);
            return false;
        }
    }
}
