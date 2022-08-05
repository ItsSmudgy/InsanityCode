using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Insanity.Common.Utilities;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Insanity.Content.NPCs.SpiritOfLight.Projectiles;
using Terraria.Audio;
using IL.Terraria.Graphics.CameraModifiers;
using Insanity.Content.Particles;
using Insanity.Common.Systems.ParticleSystem;
using Terraria.GameContent.Drawing;

namespace Insanity.Content.NPCs.SpiritOfLight
{
    public class SpiritOfLight : ModNPC
    {
        //Shit that still needs to be done
        //Final Laser
        //Scene Effect and custom sky
        //Maybe a few more attacks if I feel like it

        public enum SpiritOfLightAttacks
        {
            SpawnAnimation = -5,
            PhaseTransition = -4,
            DesperationPhase = -3,
            DeathAnimation = -2,
            RestingPhase = -1,
            RegularMovement,
            RotatingGems,
            BasicGemShots,
            GemDashes,
            GemBombs,
            SwordBarrages,
            SoulBalls,
            MonsterIllusions,
            StarDance,
            Starfall,
            LaserRain,
            GemRings,
            SideDashes,
            DiagonalDashes,
            GroundPound,
            PrismaticBarrage,
            RadiantCrystal,
            InsurgencyRays,  
            DanceOfLances
        }

        private SpiritOfLightAttacks[] PhaseOneAttackPattern = new SpiritOfLightAttacks[]
        {
            SpiritOfLightAttacks.RegularMovement,
            SpiritOfLightAttacks.RotatingGems,
            SpiritOfLightAttacks.BasicGemShots,
            SpiritOfLightAttacks.GemDashes,
            SpiritOfLightAttacks.SwordBarrages,
            SpiritOfLightAttacks.GemBombs,
            SpiritOfLightAttacks.SideDashes,
            SpiritOfLightAttacks.RegularMovement,
            SpiritOfLightAttacks.PrismaticBarrage,
            SpiritOfLightAttacks.GroundPound,
        };

        private SpiritOfLightAttacks[] PhaseTwoAttackPattern = new SpiritOfLightAttacks[]
        {
            SpiritOfLightAttacks.RegularMovement,
            SpiritOfLightAttacks.BasicGemShots,
            SpiritOfLightAttacks.Starfall,
            SpiritOfLightAttacks.SideDashes,
            SpiritOfLightAttacks.GemBombs,
            SpiritOfLightAttacks.RegularMovement,
            SpiritOfLightAttacks.GemDashes,
            SpiritOfLightAttacks.GroundPound,
            SpiritOfLightAttacks.Starfall,
            SpiritOfLightAttacks.StarDance,
            SpiritOfLightAttacks.SideDashes,
            SpiritOfLightAttacks.PrismaticBarrage,
            SpiritOfLightAttacks.RadiantCrystal,
            SpiritOfLightAttacks.InsurgencyRays,
        };

        private SpiritOfLightAttacks CurrentAttack
        {
            get => (SpiritOfLightAttacks)(int)(NPC.ai[0]);
            set => NPC.ai[0] = (int)value;
        }

        float[] AttackTimer = new float[6];
        float[] VisualTimer = new float[5];
        bool DrawIllusions = false;
        bool PhaseTwo;
        bool Enraged;
        bool canHitPlayer;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit of Light");
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 80;
            NPC.damage = 100;
            NPC.defense = 75;
            NPC.lifeMax = 30000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.scale = Main.masterMode ? 2f : 1.75f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < 4; i++)
            {
                writer.Write(NPC.localAI[i]);
            }
            for (int i = 0; i < 7; i++)
            {
                writer.Write(AttackTimer[i]);
            }
            for (int i = 0; i < 5; i++)
            {
                writer.Write(VisualTimer[i]);
            }
            writer.Write(DrawIllusions);
            writer.Write(PhaseTwo);
            writer.Write(Enraged);
            writer.Write(canHitPlayer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < 4; i++)
            {
                NPC.localAI[i] = reader.ReadSingle();
            }
            for (int i = 0; i < 7; i++)
            {
                AttackTimer[i] = reader.ReadSingle();
            }
            for (int i = 0; i < 5; i++)
            {
                VisualTimer[i] = reader.ReadSingle();
            }
            DrawIllusions = reader.ReadBoolean();
            PhaseTwo = reader.ReadBoolean();
            Enraged = reader.ReadBoolean();
            canHitPlayer = reader.ReadBoolean();
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) { return canHitPlayer; }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (player != null || player.active || !player.dead)
            {
                NPC.TargetClosest(true);
            }
            if (player.dead)
            {
                NPC.EncourageDespawn(10);
                NPC.ai[0] = -1;
            }

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.04f;
            PhaseTwo = NPC.life < NPC.lifeMax * 0.65f;
            Enraged = !player.ZoneHallow;

            switch (CurrentAttack)
            {
                case SpiritOfLightAttacks.RegularMovement:
                    BasicMovementPhase(player);
                    break;

                case SpiritOfLightAttacks.RotatingGems:
                    DoRotatingGems(player);
                    break;

                case SpiritOfLightAttacks.BasicGemShots:
                    DoBasicGemShots(player, PhaseTwo, Main.masterMode ? 120 : Main.expertMode ? 60 : 30);
                    break;

                case SpiritOfLightAttacks.GemDashes:
                    DoGemDashes(player, 1, PhaseTwo, Enraged, Main.masterMode ? 120 : 180);
                    break;

                case SpiritOfLightAttacks.GemBombs:
                    DoGemBombs(player, PhaseTwo);
                    break;

                case SpiritOfLightAttacks.SwordBarrages:
                    DoSwordBarrage(player, PhaseTwo);
                    break;

                case SpiritOfLightAttacks.SoulBalls:
                    DoSoulBalls(player);
                    break;

                case SpiritOfLightAttacks.StarDance:
                    DoCelestialAttacks(1, player, PhaseTwo, Enraged);
                    break;

                case SpiritOfLightAttacks.Starfall:
                    DoCelestialAttacks(2, player, PhaseTwo, Enraged);
                    break;

                case SpiritOfLightAttacks.SideDashes:
                    DoGemDashes(player, 2, PhaseTwo, Enraged, Main.masterMode ? 120 : 180);
                    break;

                case SpiritOfLightAttacks.DiagonalDashes:
                    DoGemDashes(player, 3, PhaseTwo, Enraged, Main.masterMode ? 120 : 180);
                    break;

                case SpiritOfLightAttacks.GroundPound:
                    DoGroundPound(player, PhaseTwo, Enraged);
                    break;

                case SpiritOfLightAttacks.PrismaticBarrage:
                    DoPrismaticBarrage(player, PhaseTwo, Enraged);
                    break;

                case SpiritOfLightAttacks.RadiantCrystal:
                    DoRadiantCrystal(player, PhaseTwo, Enraged);
                    break;

                case SpiritOfLightAttacks.InsurgencyRays:
                    DoProvidenceBeams(player, PhaseTwo, Enraged);
                    break;
            }
        }

        private void NextAttack()
        {
            int num = (int)AttackTimer[5];
            NPC.ai[0] = (float)PhaseOneAttackPattern[num % PhaseOneAttackPattern.Length];
            if (NPC.life < NPC.lifeMax * 0.65f)
            {
                NPC.ai[0] = (float)PhaseTwoAttackPattern[num % PhaseTwoAttackPattern.Length];
            }

            AttackTimer[5] += 1f;
            for (int i = 0; i < 5; i++)
            {
                AttackTimer[i] = 0;
            }
            for (int i = 0; i < 5; i++)
            {
                VisualTimer[i] = 0;
            }
            for (int i = 0; i < 4; i++)
            {
                NPC.localAI[i] = 0;
            }
            NPC.ai[1] = 0f;
            NPC.ai[2] = 0f;
            NPC.ai[3] = 0f;
            NPC.netUpdate = true;
            canHitPlayer = true;
        }

        //Attacks
        public void DoSpawnAnimation()
        {

        }

        public void DoPhaseTransition()
        {

        }

        public void DoDeathAnimation()
        {
            
        }

        public void DoDesperationPhase()
        {

        }

        public void DoRestingPhase()
        {

        }

        public void BasicMovementPhase(Player player)
        {
            float speed = Main.expertMode ? 6f : 5f;
            Vector2 pos = player.Center - Vector2.UnitY * 160f;
            NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, speed);

            AttackTimer[0]++;
            if (AttackTimer[0] >= 180)
            {
                NextAttack();
            }
        }

        public void DoBasicGemShots(Player player, bool phaseTwo, int shootTimer)
        {
            float amountOfTeleports = phaseTwo ? 6 : 4;
            int time = Main.expertMode ? 10 : 15;
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.07f, 0f, 1f);
            }
            else
            {
                if (NPC.ai[1] == 0f)
                {
                    AttackTimer[1]++;
                    Vector2 teleportPosition = player.Center + new Vector2(-600, -300);
                    if (AttackTimer[1] == 1)
                    {
                        NPC.Center = teleportPosition;
                        NPC.velocity = NPC.DirectionTo(player.Center + Vector2.UnitX * 300f) * 10f;
                        NPC.velocity.Y *= 0f;
                        NPC.netUpdate = true;
                    }

                    if (AttackTimer[1] >= 120)
                    {
                        NPC.velocity *= 0.9f;
                        NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.07f, 0f, 1f);
                    }
                    else
                    {
                        NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.07f, 0f, 1f);
                        if (AttackTimer[1] % time == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projSpeed = Main.expertMode ? 9f : 7f;
                            Vector2 projVel = (player.Center - NPC.Center);
                            projVel.Normalize();
                            InsanityUtils.NewProjectileBetter(NPC.Center, projVel * projSpeed, ModContent.ProjectileType<GemShot>(), NPC.damage / 2, 0f);
                        }
                    }

                    if (AttackTimer[1] >= 150)
                    {
                        if (NPC.ai[2] < amountOfTeleports - 1)
                        {
                            AttackTimer[1] = 0f;
                            NPC.ai[1] = 1f;
                            NPC.ai[2]++;
                        }
                        else
                        {
                            NPC.ai[1] = 2f;
                            AttackTimer[1] = 0f;
                        }
                        NPC.netUpdate = true;
                    }
                }

                if (NPC.ai[1] == 1f)
                {
                    AttackTimer[1]++;
                    Vector2 teleportPosition = player.Center + new Vector2(600, 300);
                    if (AttackTimer[1] == 1)
                    {
                        NPC.Center = teleportPosition;
                        NPC.velocity = NPC.DirectionTo(player.Center - Vector2.UnitX * 300f) * 10f;
                        NPC.velocity.Y *= 0f;
                        NPC.netUpdate = true;
                    }

                    if (AttackTimer[1] >= 120)
                    {
                        NPC.velocity *= 0.9f;
                        NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.07f, 0f, 1f);
                    }
                    else
                    {
                        NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.07f, 0f, 1f);
                        if (AttackTimer[1] % time == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projSpeed = Main.expertMode ? 9f : 7f;
                            Vector2 projVel = (player.Center - NPC.Center);
                            projVel.Normalize();
                            InsanityUtils.NewProjectileBetter(NPC.Center, projVel * projSpeed, ModContent.ProjectileType<GemShot>(), NPC.damage / 2, 0f);
                        }
                    }

                    if (AttackTimer[1] >= 150)
                    {
                        if (NPC.ai[2] < amountOfTeleports - 1)
                        {
                            AttackTimer[1] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2]++;
                        }
                        else
                        {
                            NPC.ai[1] = 2f;
                            AttackTimer[1] = 0f;
                        }
                        NPC.netUpdate = true;
                    }
                }

                if (NPC.ai[1] == 2f)
                {
                    AttackTimer[1]++;
                    Vector2 teleportPosition = player.Center - Vector2.UnitY * 200;
                    if (AttackTimer[1] == 1)
                    {
                        NPC.Center = teleportPosition;
                        NPC.netUpdate = true;
                    }

                    if (AttackTimer[1] >= 120)
                    {
                        NextAttack();
                    }
                    else
                    {
                        NPC.velocity *= 0.9f;
                        NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.07f, 0f, 1f);
                    }
                }
            }
        }

        public void DoGemDashes(Player player, int type, bool phaseTwo, bool enraged, int dashTime)
        {
            //normal dashes
            if (type == 1)
            {
                AttackTimer[0]++;
                if (AttackTimer[0] < 60)
                {
                    NPC.velocity *= 0.9f;
                }
                else
                {
                    if (AttackTimer[0] == 60)
                    {
                        NPC.localAI[0] = phaseTwo ? Main.rand.Next(5, 11) : Main.rand.Next(3, 7);
                    }

                    if (AttackTimer[1] == 0f)
                    {
                        canHitPlayer = false;
                        AttackTimer[2]++;
                        if (AttackTimer[2] % 30 == 0 && NPC.localAI[1] < NPC.localAI[0])
                        {
                            SoundEngine.PlaySound(SoundID.Item6, NPC.Center);
                            Vector2 teleportPos = player.Center + Main.rand.NextVector2CircularEdge(400, 400);
                            NPC.Center = teleportPos;
                            NPC.localAI[1]++;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                /*int num = InsanityUtils.NewProjectileBetter(NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<Content.Projectiles.ProjectileRippler>(), NPC.damage / 2, 0f);
                                if (Main.projectile.IndexInRange(num))
                                {
                                    Main.projectile[num].localAI[0] = 5f;
                                    Main.projectile[num].localAI[1] = 15f;
                                    Main.projectile[num].ai[1] = ModContent.ProjectileType<CrystalSpikeSpawner>();
                                }*/

                                int num2 = InsanityUtils.NewProjectileBetter(NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CrystalShockwave>(), 0, 0f);
                                if (Main.projectile.IndexInRange(num2))
                                {
                                    Main.projectile[num2].ai[1] = 5f;
                                }
                            }

                            for (int i = 0; i < 30; i++)
                            {
                                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2); ;
                                Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                                Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(7f, 14f);
                                ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.35f, 0.85f), Main.rand.Next(100, 120), false, true));
                            }
                            NPC.netUpdate = true;
                        }

                        if (NPC.localAI[1] >= NPC.localAI[0])
                        {
                            AttackTimer[3]++;
                            if (AttackTimer[3] >= 120)
                            {
                                AttackTimer[1] = 1f;
                                AttackTimer[2] = 0f;
                                NPC.netUpdate = true;
                            }

                            if (phaseTwo)
                            {
                                VisualTimer[0] = MathHelper.Clamp(VisualTimer[0] + 0.07f, 0f, 1f);
                                VisualTimer[1] = MathHelper.Clamp(VisualTimer[1] + 0.07f, 0f, 1f);
                            }
                        }
                    }
                    if (AttackTimer[1] == 1f)
                    {
                        canHitPlayer = true;
                        if (NPC.ai[1] == 0f)
                        {
                            float dashSpeed = Main.masterMode ? 24f : Main.expertMode ? 18f : 14f;
                            NPC.velocity = NPC.DirectionTo(player.Center) * dashSpeed;
                            NPC.ai[1] = 1f;
                            NPC.netUpdate = true;
                        }

                        if (NPC.ai[1] == 1f)
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] >= 45)
                            {
                                NPC.velocity *= 0.9f;
                            }
                            else
                            {
                                NPC.rotation = NPC.velocity.X * 0.02f;
                                NPC.spriteDirection = NPC.direction;
                                for (int i = 0; i < 2; i++)
                                {
                                    Vector2 value6 = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(-7f, 8f);
                                    Vector2 center = NPC.Center;
                                    ParticleOrchestraSettings particleOrchestraSettings = new ParticleOrchestraSettings();
                                    particleOrchestraSettings.PositionInWorld = center;
                                    particleOrchestraSettings.MovementVector = value6;
                                    ParticleOrchestraSettings settings = particleOrchestraSettings;
                                    ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, NPC.target);
                                    particleOrchestraSettings = new ParticleOrchestraSettings();
                                    particleOrchestraSettings.PositionInWorld = center;
                                    particleOrchestraSettings.MovementVector = value6 * 2f;
                                    settings = particleOrchestraSettings;
                                    ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, NPC.target);
                                }
                            }

                            int time = phaseTwo ? 75 : 90;
                            if (NPC.ai[2] >= time)
                            {
                                NPC.spriteDirection = -NPC.direction;
                                NPC.ai[3]++;
                                if (NPC.ai[3] >= NPC.localAI[0])
                                {
                                    if (phaseTwo)
                                    {
                                        AttackTimer[4]++;
                                        if (AttackTimer[4] >= 60)
                                        {
                                            NextAttack();
                                        }
                                        else
                                        {
                                            VisualTimer[0] = MathHelper.Clamp(VisualTimer[0] - 0.07f, 0f, 1f);
                                            VisualTimer[1] = MathHelper.Clamp(VisualTimer[1] - 0.07f, 0f, 1f);
                                        }
                                    }
                                    else
                                    {
                                        NextAttack();
                                    }
                                }
                                else
                                {
                                    NPC.ai[1] = 0f;
                                    NPC.ai[2] = 0f;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                    } 
                }
            }
            else //side/diagonal cause copying code 3 times isnt a good practice
            {
                //type 2 = Side Dashes
                //type 3 = Diagonal Dashes
                AttackTimer[0]++;
                if (AttackTimer[0] < 60)
                {
                    NPC.velocity *= 0.9f;
                }
                else
                {
                    if (NPC.ai[1] == 0f)
                    {
                        int timer = phaseTwo ? dashTime - 60 : dashTime;
                        AttackTimer[1]++;
                        if (AttackTimer[1] <= timer)
                        {
                            Vector2 pos = player.Center + new Vector2(NPC.Center.X > player.Center.X ? 400 : -400, type == 3 ? (NPC.Center.Y > player.Center.Y ? 400 : -400) : 0f);
                            Movement(pos, 0.25f, Main.masterMode ? 16 : 12);
                        }

                        if (AttackTimer[1] >= timer)
                        {
                            NPC.ai[1] = 1f;
                            AttackTimer[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }

                    else if (NPC.ai[1] == 1f)
                    {
                        float dashSpeed = enraged ? 28f :  Main.expertMode ? 24f : 20f;
                        NPC.velocity = NPC.DirectionTo(player.Center) * dashSpeed;
                        NPC.velocity.Y *= 0f;
                        NPC.ai[1] = 2f;
                        NPC.netUpdate = true;
                    }

                    else if (NPC.ai[1] == 2f)
                    {
                        NPC.ai[2]++;
                        if (NPC.ai[2] >= 80)
                        {
                            NPC.velocity *= 0.9f;
                        }
                        else
                        {
                            NPC.rotation = NPC.velocity.X * 0.02f;
                            if (type == 2)
                            {
                                int timer = phaseTwo ? (8) : (6);
                                if (phaseTwo)
                                {
                                    if (AttackTimer[0] % timer == 0)
                                    {
                                        int numProj = Main.rand.Next(1, 3);
                                        for (int i = 0; i < numProj; i++)
                                        {
                                            Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f / numProj * i) * Main.rand.NextFloat(6f, 8f);
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                InsanityUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<PrismaticBolt>(), NPC.damage / 2, 0f);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (AttackTimer[0] % timer == 0)
                                    {
                                        Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f) * Main.rand.NextFloat(5f, 8f);
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            InsanityUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<PrismaticBolt>(), NPC.damage / 2, 0f);
                                        }
                                    }
                                }
                            }
                        }

                        int time = enraged ? 120 : Main.expertMode ? (phaseTwo ? 125 : 150) : (phaseTwo ? 140 : 160);
                        if (NPC.ai[2] >= time)
                        {
                            NPC.ai[3]++;
                            if (NPC.ai[3] >= 3)
                            {
                                NPC.ai[1] = 3f;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                                NPC.netUpdate = true;
                            }
                        }
                    }

                    if (NPC.ai[1] == 3f)
                    {
                        canHitPlayer = false;
                        AttackTimer[1]++;
                        if (AttackTimer[1] >= 120)
                        {
                            NextAttack();
                        }
                        else
                        {
                            float speed = 12f;
                            Vector2 pos = player.Center - Vector2.UnitY * 160f;
                            NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, speed);
                        }
                    }
                }
            }            
        }

        public void DoRotatingGems(Player player)
        {
            int time = Main.masterMode ? 520 : Main.expertMode ? 420 : 300;
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
                if (NPC.ai[1] == 0f)
                {
                    NPC.ai[1] = 1f;
                    int numberOfGems = Main.masterMode ? 4 : Main.expertMode ? 3 : 2;
                    InsanityUtils.SpawnNPCRotatersForNPC(NPC.whoAmI, NPC.width + 35, 4f, ModContent.NPCType<RotatingGem>(), NPC.damage / 2, numberOfGems, Main.rand.NextBool(2));
                }
            }

            if (AttackTimer[0] > 60 && AttackTimer[0] < time)
            {
                Vector2 pos = player.Center + NPC.DirectionFrom(player.Center) * 500f;
                Movement(pos, 0.25f, Main.masterMode ? 16 : 12);
            }

            if (AttackTimer[0] >= time)
            {
                NPC.velocity *= 0.9f;
                InsanityUtils.KillNPCs(ModContent.NPCType<RotatingGem>());
            }

            if (AttackTimer[0] >= time + 60)
            {
                AttackTimer[0] = 0;
                NPC.ai[1] = 0f;
                NPC.ai[0] = 0f;
                NPC.netUpdate = true;
            }
        }

        public void DoGemBombs(Player player, bool phaseTwo)
        {
            int time = 60;
            int num = 360;
            int num2 = 15;
            canHitPlayer = false;
            AttackTimer[0]++;
            if (AttackTimer[0] <= time)
            {
                Vector2 pos = player.Center - Vector2.UnitY * 300f;
                NPC.velocity = NPC.DirectionTo(pos) * 8f;
            }
            else
            {
                float movementLerp = Utils.GetLerpValue(num, num - num2, AttackTimer[0] - 60, true);
                Vector2 spinCenter = player.Center - Vector2.UnitY.RotatedBy(Math.PI * 2 / num * 2f * AttackTimer[0]) * 420f;
                NPC.velocity = Vector2.Zero.MoveTowards(spinCenter - NPC.Center, movementLerp * 25f);
                if (AttackTimer[0] % num2 == 0 && AttackTimer[0] < num)
                {
                    InsanityUtils.NewProjectileBetter(NPC.Center, Vector2.UnitX.RotatedByRandom(Math.PI * 2f) * Main.rand.NextFloat(2f, 5f), ModContent.ProjectileType<GemBomb>(), NPC.damage, 0f, -1, 0, 180);
                }
            }

            if (AttackTimer[0] >= num)
            {
                Vector2 pos = player.Center - Vector2.UnitY * 160f;
                NPC.velocity = NPC.DirectionTo(pos) * 8f;
            }

            if (AttackTimer[0] >= num + 180)
            {
                NextAttack();
                NPC.netUpdate = true;
            }
        }

        public void DoSoulBalls(Player player)
        {
            int time = Main.masterMode ? 420 : Main.expertMode ? 360 : 300;
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
                if (NPC.ai[1] == 0f)
                {
                    NPC.ai[1] = 1f;
                    int numberOfGems = Main.masterMode ? 3 : 2;
                    InsanityUtils.SpawnNPCRotatersForNPC(NPC.whoAmI, NPC.width + 35, 1f, ModContent.NPCType<SoulBall>(), NPC.damage / 2, numberOfGems, Main.rand.NextBool(2));
                }
            }

            if (AttackTimer[0] > 60 && AttackTimer[0] < time)
            {
                NPC.velocity = Vector2.Zero.MoveTowards((player.Center + player.velocity * 30f), 2f);
            }

            if (AttackTimer[0] >= time)
            {
                NPC.velocity *= 0.9f;
                InsanityUtils.KillNPCs(ModContent.NPCType<SoulBall>());
            }

            if (AttackTimer[0] >= time + 60)
            {
                AttackTimer[0] = 0;
                NPC.ai[1] = 0f;
                NPC.ai[0] = 0f;
                NPC.netUpdate = true;
            }
        }

        public void DoSwordBarrage(Player player, bool phaseTwo)
        {
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
            }

            if (AttackTimer[0] > 60 && AttackTimer[0] < 520)
            {
                Vector2 pos = player.Center + new Vector2(NPC.Center.X > player.Center.X ? 350 : -350, 0f);
                Movement(pos, 0.25f, Main.masterMode ? 16 : 12);

                if (AttackTimer[0] % 120 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 initialVel = NPC.DirectionFrom(player.Center) * 10f;
                    int numProj = Main.expertMode ? (phaseTwo ? 8 : 6) : (phaseTwo ? 7 : 5);
                    for (int i = -numProj; i < numProj; i++)
                    {
                        InsanityUtils.NewProjectileBetter(NPC.Center, initialVel.RotatedBy((Math.PI / 2) / numProj * i), ModContent.ProjectileType<EnchantedDagger>(), NPC.damage / 2, 0f, default, 20, 40);
                    }
                }
            }

            if (AttackTimer[0] >= 520)
            {
                NPC.velocity *= 0.9f;
            }

            if (AttackTimer[0] >= 580)
            {
                AttackTimer[0] = 0;
                NPC.ai[0] = 0f;
                NPC.netUpdate = true;
            }
        }

        public void DoMonsterIllusions()
        {
            //WIP
        }

        public void DoLaserRain()
        {
            //WIP
        }

        public void DoCelestialAttacks(int type, Player player, bool phaseTwo, bool enraged)
        {
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
            }
            else
            {
                //night
                if (!Main.dayTime)
                {
                    //star dance
                    if (type == 1)
                    {
                        AttackTimer[1]++;
                        if (++AttackTimer[2] >= 90)
                            AttackTimer[2] = 0;

                        NPC.localAI[0] += (float)Math.PI / 120;
                        int attackspeed = 6;
                        if (AttackTimer[2] > 30 && AttackTimer[2] < 120 && AttackTimer[1] % attackspeed == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    InsanityUtils.NewProjectileBetter(NPC.Center, Vector2.UnitX.RotatedBy(NPC.localAI[0] + Math.PI * 2 / 4 * i), ModContent.ProjectileType<CelestialStar>(), 50, 0f, ai1: 0.015f);
                                }
                            }
                        }

                        if (AttackTimer[1] % 60 == 0)
                        {
                            NPC.localAI[1] = Main.rand.NextFloat(MathHelper.TwoPi);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 16; i++)
                                {
                                    InsanityUtils.NewProjectileBetter(NPC.Center, Vector2.UnitX.RotatedBy((Math.PI * 2 / 16 * i) + NPC.localAI[1]), ModContent.ProjectileType<CelestialStar>(), 50, 0f, ai1: 0.015f);
                                }
                            }
                        }

                        if (AttackTimer[1] >= 720)
                        {
                            NextAttack();
                        }
                    }

                    //starfall
                    if (type == 2)
                    {
                        AttackTimer[1]++;
                        Vector2 pos = player.Center + new Vector2(Main.rand.NextFloat(-1000, 1000), -700);
                        Vector2 vel = Vector2.UnitY * 8f;
                        int attackspeed = phaseTwo ? 3 : 6;
                        if (AttackTimer[1] % attackspeed == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num = Main.rand.Next(Main.numStars);
                            if (Main.star[num] != null && !Main.star[num].hidden && !Main.star[num].falling)
                            {
                                Main.star[num].Fall();
                            }

                            InsanityUtils.NewProjectileBetter(pos, vel, ModContent.ProjectileType<CelestialStarFalling>(), 50, 0f);
                        }

                        if (AttackTimer[1] >= 300)
                        {
                            NextAttack();
                        }
                    }
                }

                //day
                if (Main.dayTime)
                {
                    //cant think of anything for this at the moment brrrr
                    NextAttack();
                }
            }          
        }

        public void DoGroundPound(Player player, bool phaseTwo, bool enraged)
        {
            AttackTimer[0]++;
            if (NPC.ai[1] == 0f)
            {
                if (AttackTimer[0] < 60)
                {
                    NPC.velocity *= 0.9f;
                }

                if (AttackTimer[0] > 60 && AttackTimer[0] < 120)
                {
                    Vector2 pos = player.Center + new Vector2(0f, -350f);
                    Movement(pos, 0.55f, Main.masterMode ? 12 : 8);
                }

                if (AttackTimer[0] == 145)
                {
                    NPC.noTileCollide = false;
                }

                if (AttackTimer[0] > 120 && AttackTimer[0] < 150)
                {
                    NPC.velocity *= 0.96f;
                }

                if (AttackTimer[0] >= 150)
                {
                    if (NPC.velocity.Y < 18)
                    {
                        NPC.velocity.Y += 3f;
                    }
                    if (NPC.collideY)
                    {
                        NPC.ai[1] = 1f;
                        NPC.velocity.Y = -7;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int num = InsanityUtils.NewProjectileBetter(NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<Content.Projectiles.ProjectileRippler>(), NPC.damage / 2, 0f);
                            if (Main.projectile.IndexInRange(num))
                            {
                                Main.projectile[num].localAI[0] = 5f;
                                Main.projectile[num].localAI[1] = 15f;
                                Main.projectile[num].ai[1] = ModContent.ProjectileType<CrystalSpikeSpawner>();
                            }

                            int num2 = InsanityUtils.NewProjectileBetter(NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CrystalShockwave>(), 0, 0f);
                            if (Main.projectile.IndexInRange(num2))
                            {
                                Main.projectile[num2].ai[1] = 10f;
                            }
                        }

                        for (int i = 0; i < 30; i++)
                        {
                            Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2); ;
                            Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                            Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(7f, 14f);
                            ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.35f, 0.85f), Main.rand.Next(100, 120), false, true));
                        }

                        for (int i = 0; i < 30; i++)
                        {
                            Vector2 value6 = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(-12f, 13f);
                            Vector2 center = NPC.Center;
                            ParticleOrchestraSettings particleOrchestraSettings = new ParticleOrchestraSettings();
                            particleOrchestraSettings.PositionInWorld = center;
                            particleOrchestraSettings.MovementVector = value6;
                            ParticleOrchestraSettings settings = particleOrchestraSettings;
                            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, NPC.target);
                            particleOrchestraSettings = new ParticleOrchestraSettings();
                            particleOrchestraSettings.PositionInWorld = center;
                            particleOrchestraSettings.MovementVector = value6 * 2f;
                            settings = particleOrchestraSettings;
                            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, NPC.target);
                        }
                    }
                }
            }

            if (NPC.ai[1] == 1f)
            {
                if (AttackTimer[0] > 150 && AttackTimer[0] < 200)
                {
                    int num2 = 2;
                    int num3 = 15;
                    float num4 = 15f;
                    float minScale = 0.25f;
                    float maxScale = 1f;
                    AttackTimer[1]++;
                    Point sourceTileCoords = NPC.Bottom.ToTileCoordinates();
                    int num5 = (int)AttackTimer[1] / num2;
                    int spikeCount = num3 / num2;
                    if (num2 <= 1f || (AttackTimer[1] % (float)num2 == 1f && AttackTimer[1] < (float)num3))
                    {
                        float horizontalOffset = (float)num5 * num4;
                        float scale = Utils.Remap(AttackTimer[1] / (float)num3, 0f, 0.75f, minScale, maxScale);
                        TryMakingSpike(ref sourceTileCoords, -NPC.spriteDirection, spikeCount, num5, (int)horizontalOffset, scale);
                        TryMakingSpike(ref sourceTileCoords, NPC.spriteDirection, spikeCount, num5, (int)horizontalOffset, scale);
                    }
                }

                if (AttackTimer[0] >= 150)
                {
                    NPC.velocity *= 0.9f;
                    NPC.noTileCollide = true;
                }

                if (phaseTwo)
                {                  
                    if (AttackTimer[0] >= 200)
                    {
                        if (NPC.ai[2] == 2f)
                        {
                            NPC.ai[1] = 2f;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[2]++;
                            AttackTimer[0] = 60;
                            NPC.netUpdate = true;
                        }
                        AttackTimer[1] = 0;
                    }
                }
                else
                {
                    if (AttackTimer[0] >= 300)
                    {
                        NextAttack();
                    }
                }
            }  
            
            if (NPC.ai[1] == 2f)
            {
                if (AttackTimer[0] < 400)
                {
                    NPC.velocity *= 0.9f;
                }
                else
                {
                    NextAttack();
                }
            }
        }

        public void DoPrismaticBarrage(Player player, bool phaseTwo, bool enraged)
        {
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
                NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.12f, 0f, 1f);
            }
            else
            {
                if (AttackTimer[0] == 60)
                {
                    NPC.Center = player.Center - Vector2.UnitY * 300;
                }

                NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.12f, 0f, 1f);
                if (NPC.Opacity >= 1f && AttackTimer[0] < 520)
                {
                    VisualTimer[2] += 0.05f;
                    if (VisualTimer[2] >= 1f)
                    {
                        VisualTimer[2] = 1f;
                    }

                    if (++AttackTimer[1] > 120)
                        AttackTimer[1] = 0;

                    NPC.velocity = Vector2.Zero;
                    int time = phaseTwo ? (8) : (6);
                    if (phaseTwo)
                    {
                        if (AttackTimer[0] % time == 0)
                        {
                            int numProj = Main.rand.Next(1, 3);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f / numProj * i) * Main.rand.NextFloat(6f, 8f);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    InsanityUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<PrismaticBolt>(), NPC.damage / 2, 0f);
                                }
                            }
                        }

                        NPC.localAI[0] += (float)Math.PI / 100;
                        if (AttackTimer[1] > 80 && AttackTimer[1] < 120 && AttackTimer[0] % 10 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    InsanityUtils.NewProjectileBetter(NPC.Center, Vector2.UnitX.RotatedBy(NPC.localAI[0] + Math.PI * 2 / 4 * i), ModContent.ProjectileType<PrismaticBolt2>(), 50, 0f, ai1: 0.015f);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (AttackTimer[0] % time == 0)
                        {
                            Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f) * Main.rand.NextFloat(5f, 8f);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                InsanityUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<PrismaticBolt>(), NPC.damage / 2, 0f);
                            }
                        }
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 value6 = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(-10f, 11f);
                        Vector2 center = NPC.Center;
                        ParticleOrchestraSettings particleOrchestraSettings = new ParticleOrchestraSettings();
                        particleOrchestraSettings.PositionInWorld = center;
                        particleOrchestraSettings.MovementVector = value6;
                        ParticleOrchestraSettings settings = particleOrchestraSettings;
                        ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, NPC.target);
                        particleOrchestraSettings = new ParticleOrchestraSettings();
                        particleOrchestraSettings.PositionInWorld = center;
                        particleOrchestraSettings.MovementVector = value6 * 2f;
                        settings = particleOrchestraSettings;
                        ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.PrincessWeapon, settings, NPC.target);
                    }
                }
            }

            if (AttackTimer[0] >= 520)
            {
                VisualTimer[2] -= 0.05f;
                if (VisualTimer[2] <= 0f)
                {
                    VisualTimer[2] = 0f;
                }               
            }

            if (AttackTimer[0] >= 580)
            {
                NextAttack();
            }
        }

        public void DoRadiantCrystal(Player player, bool phaseTwo, bool enraged)
        {
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
            }
            else
            {
                if (AttackTimer[0] == 60)
                {
                    InsanityUtils.NewProjectileBetter(NPC.Center, Vector2.Zero, ModContent.ProjectileType<RadiantGem>(), 100, 0f);
                }

                if (AttackTimer[0] >= 1500)
                {
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity + 0.07f, 0f, 1f);
                }
                else
                {
                    NPC.Opacity = MathHelper.Clamp(NPC.Opacity - 0.07f, 0f, 1f);
                }

                if (AttackTimer[0] >= 1560)
                {
                    NextAttack();
                }
            }
        }

        public void DoProvidenceBeams(Player player, bool phaseTwo, bool enraged)
        {
            AttackTimer[0]++;
            if (AttackTimer[0] < 60)
            {
                NPC.velocity *= 0.9f;
            }
            else
            {
                if (AttackTimer[0] < 240)
                {
                    Vector2 pos = player.Center + new Vector2(NPC.Center.X > player.Center.X ? 350 : -350, 0f);
                    Movement(pos, 0.25f, Main.masterMode ? 16 : 12);
                    VisualTimer[2] = Utils.GetLerpValue(0f, 1f, AttackTimer[0] / 240, true);
                    if (VisualTimer[2] >= 1f)
                    {
                        VisualTimer[2] = 1f;
                    }

                    //charge up particles
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 val5 = NPC.Center + Main.rand.NextVector2CircularEdge(Main.rand.NextFloat(50, 300), Main.rand.NextFloat(50, 300));
                        Vector2 v = val5 - NPC.Center;
                        v = v.SafeNormalize(Vector2.Zero) * -8f;
                        Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                        ParticleManager.SpawnParticle(new BasicGlowParticle(val5, v, color, Main.rand.NextFloat(0.15f, 0.35f), Main.rand.Next(60, 70), false, true));
                    }

                    if (AttackTimer[0] % 15 == 0 && AttackTimer[0] < 180)
                    {
                        Vector2 vel = Vector2.UnitX.RotatedByRandom(Math.PI * 2f) * Main.rand.NextFloat(5f, 8f);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            InsanityUtils.NewProjectileBetter(NPC.Center, vel, ModContent.ProjectileType<PrismaticBolt>(), NPC.damage / 2, 0f);
                        }
                    }
                }
                else
                {
                    canHitPlayer = false;
                    float speed = Main.expertMode ? 3f : 2f;
                    Vector2 pos = player.Center;
                    NPC.velocity = Vector2.Zero.MoveTowards(pos - NPC.Center, speed);
                    if (AttackTimer[0] > 420)
                    {
                        VisualTimer[2] = Utils.GetLerpValue(1f, 0f, AttackTimer[0] / 150, true);
                        if (VisualTimer[2] <= 0f)
                        {
                            VisualTimer[2] = 0f;
                        }
                    }

                    //release particles
                    if (AttackTimer[0] < 420)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                            Color color = Color.Lerp(new Color(255, 141, 211), new Color(169, 27, 125), Main.rand.NextFloat(0.1f, 0.9f));
                            Vector2 vel = Vector2.UnitX.RotatedByRandom((float)Math.PI * 2f) * Main.rand.NextFloat(7f, 14f);
                            ParticleManager.SpawnParticle(new BasicGlowParticle(spawnPos, vel, color, Main.rand.NextFloat(0.75f, 1.5f), Main.rand.Next(100, 120), false, true));
                        }
                    }
                }

                if (AttackTimer[0] == 240)
                {
                    int num2 = InsanityUtils.NewProjectileBetter(NPC.Center, Vector2.Zero, ModContent.ProjectileType<CrystalShockwave2>(), 0, 0f);
                    if (Main.projectile.IndexInRange(num2))
                    {
                        Main.projectile[num2].ai[1] = 10f;
                    }

                    for (int i = -1; i <= 1; i+= 2)
                    {
                        Vector2 velocity = (player.Center - NPC.Center).SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(160) * (float)(-i));
                        int ray = InsanityUtils.NewProjectileBetter(NPC.Center, velocity, ModContent.ProjectileType<InsurgencyRay>(), NPC.damage / 2, 0f);
                        if (Main.projectile.IndexInRange(ray))
                        {
                            Main.projectile[ray].ai[0] = i * (float)((MathHelper.ToRadians(160)) / 80 * 0.4f);
                            Main.projectile[ray].ai[1] = NPC.whoAmI;
                            Main.projectile[ray].netUpdate = true;
                        }
                    }
                    NPC.netUpdate = true;
                }

                if (AttackTimer[0] >= 480)
                {
                    NextAttack();
                }
            }
        }

        public void DoEmpressLances()
        {

        }

        //Movement + other stuffs
        private void Movement(Vector2 targetPosition, float speedModifier, float speedCap = 12f)
        {
            if (NPC.Center.X < targetPosition.X)
            {
                NPC.velocity.X += speedModifier;
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += speedModifier * 2f;
            }
            else
            {
                NPC.velocity.X -= speedModifier;
                if (NPC.velocity.X > 0)
                    NPC.velocity.X -= speedModifier * 2f;
            }

            if (NPC.Center.Y < targetPosition.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2f;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2f;
            }

            if (Math.Abs(NPC.velocity.X) > speedCap)
                NPC.velocity.X = speedCap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > speedCap)
                NPC.velocity.Y = speedCap * Math.Sign(NPC.velocity.Y);
        }

        
        private void TryMakingSpike(ref Point sourceTileCoords, int dir, int count, int index, int xOffset, float scale)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            int num = NPC.damage / 2;
            int num2 = sourceTileCoords.X + (int)(xOffset / 16 * dir);
            int num3 = TryMakingSpike_FindBestY(ref sourceTileCoords, num2);
            if (WorldGen.ActiveAndWalkableTile(num2, num3))
            {
                Vector2 position = new Vector2((float)(num2 * 16 + 8), (float)(num3 * 16) - scale);
                Vector2 velocity = Utils.RotatedBy(-Vector2.UnitY, (float)(index * dir) * 0.7f * ((float)Math.PI / 4f / (float)count));
                int num4 = InsanityUtils.NewProjectileBetter(position, velocity, ModContent.ProjectileType<CrystalSpike>(), num, 0f);
                if (Main.projectile.IndexInRange(num4))
                {
                    Main.projectile[num4].ai[1] = scale;
                }
            }
        }

        private int TryMakingSpike_FindBestY(ref Point sourceTileCoords, int x)
        {
            int num = sourceTileCoords.Y;
            NPCAimedTarget targetData = NPC.GetTargetData();
            if (!targetData.Invalid)
            {
                Rectangle hitbox = targetData.Hitbox;
                Vector2 vector = new Vector2((float)((Rectangle)(hitbox)).Center.X, (float)((Rectangle)hitbox).Bottom);
                int num7 = (int)(vector.Y / 16f);
                int num2 = Math.Sign(num7 - num);
                int num3 = num7 + num2 * 15;
                int? num4 = null;
                float num5 = float.PositiveInfinity;
                for (int i = num; i != num3; i += num2)
                {
                    if (WorldGen.ActiveAndWalkableTile(x, i))
                    {
                        float num6 = Utils.ToWorldCoordinates(new Point(x, i), 8f, 8f).Distance(vector);
                        if (!num4.HasValue || !(num6 >= num5))
                        {
                            num4 = i;
                            num5 = num6;
                        }
                    }
                }
                if (num4.HasValue)
                {
                    num = num4.Value;
                }
            }
            for (int j = 0; j < 20; j++)
            {
                if (num < 10)
                {
                    break;
                }
                if (!WorldGen.SolidTile(x, num))
                {
                    break;
                }
                num--;
            }
            for (int k = 0; k < 20; k++)
            {
                if (num > Main.maxTilesY - 10)
                {
                    break;
                }
                if (WorldGen.ActiveAndWalkableTile(x, num))
                {
                    break;
                }
                num++;
            }
            return num;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D trail = Mod.Assets.Request<Texture2D>("Content/NPCs/SpiritOfLight/SpiritOfLight_Trail").Value;
            DrawHelper.DrawNPCTrail(NPC.whoAmI, trail, NPC.GetAlpha(Color.Lerp(new Color(255, 141, 211), Color.Transparent, VisualTimer[1])), NPC.rotation, false, 0f, true, true);
            DrawHelper.DrawNPCTexture(NPC.whoAmI, texture, NPC.GetAlpha(Color.Lerp(drawColor, Color.Transparent, VisualTimer[1])), NPC.rotation, NPC.scale, true, true);
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float num245 = 0f;
            float num246 = Main.NPCAddHeight(NPC);
            Vector2 halfSize = NPC.frame.Size() / 2;
            Color alpha3 = NPC.GetAlpha(new Color(255, 141, 211));
            float num115 = VisualTimer[1];

            alpha3.R = ((byte)(alpha3.R * num115));
            alpha3.G = ((byte)(alpha3.G * num115));
            alpha3.B = ((byte)(alpha3.B * num115));
            alpha3.A = ((byte)(alpha3.A * num115));
            for (int num116 = 0; num116 < 4; num116++)
            {
                Vector2 position7 = NPC.position;
                float num117 = Math.Abs(NPC.Center.X - Main.player[Main.myPlayer].Center.X);
                float num118 = Math.Abs(NPC.Center.Y - Main.player[Main.myPlayer].Center.Y);
                if (num116 == 0 || num116 == 2)
                {
                    position7.X = Main.player[Main.myPlayer].Center.X + num117;
                }
                else
                {
                    position7.X = Main.player[Main.myPlayer].Center.X - num117;
                }
                position7.X -= NPC.width / 2;
                if (num116 == 0 || num116 == 1)
                {
                    position7.Y = Main.player[Main.myPlayer].Center.Y + num118;
                }
                else
                {
                    position7.Y = Main.player[Main.myPlayer].Center.Y - num118;
                }
                position7.Y -= NPC.height / 2;
                Main.EntitySpriteDraw(trail, new Vector2(position7.X - screenPos.X + (float)(NPC.width / 2) - (float)trail.Width * NPC.scale / 2f + halfSize.X * NPC.scale, position7.Y - screenPos.Y + (float)NPC.height - (float)trail.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + num246 + num245 + NPC.gfxOffY), (Rectangle?)NPC.frame, alpha3, NPC.rotation, halfSize, NPC.scale, effects, 0);
            }
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color colorLerp = Color.Lerp(Color.White, Color.Transparent, VisualTimer[0]);
            Texture2D glow = Mod.Assets.Request<Texture2D>("Content/NPCs/SpiritOfLight/SpiritOfLight_Glow").Value;
            DrawHelper.DrawNPCTexture(NPC.whoAmI, glow, NPC.GetAlpha(colorLerp), NPC.rotation, NPC.scale, true, true);

            Texture2D glow2 = Mod.Assets.Request<Texture2D>("Content/NPCs/SpiritOfLight/SpiritOfLight_Trail").Value;
            Color colorLerp2 = Color.Lerp(Color.Transparent, new Color(255, 141, 211), VisualTimer[2]);
            DrawHelper.DrawNPCTexture(NPC.whoAmI, glow2, NPC.GetAlpha(colorLerp2), NPC.rotation, NPC.scale, true, true, true, true);
        }
    }
}
