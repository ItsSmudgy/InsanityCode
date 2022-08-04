using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.GameContent.ItemDropRules;

namespace Insanity.Content.NPCs
{
	// The minions spawned when the body spawns
	// Please read MinionBossBody.cs first for important comments, they won't be explained here again
	public class Kraken : ModNPC
	{
		// This is a neat trick that uses the fact that NPCs have all NPC.ai[] values set to 0f on spawn (if not otherwise changed).
		// We set ParentIndex to a number in the body after spawning it. If we set ParentIndex to 3, NPC.ai[0] will be 4. If NPC.ai[0] is 0, ParentIndex will be -1.
		// Now combine both facts, and the conclusion is that if this NPC spawns by other means (not from the body), ParentIndex will be -1, allowing us to distinguish
		// between a proper spawn and an invalid/"cheated" spawn
		public int ParentIndex {
			get => (int)NPC.ai[0] - 1;
			set => NPC.ai[0] = value + 1;
		}

		public bool HasParent => ParentIndex > -1;

		public int PositionIndex {
			get => (int)NPC.ai[1] - 1;
			set => NPC.ai[1] = value + 1;
		}

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Kraken");
			Main.npcFrameCount[Type] = 10;

			// By default enemies gain health and attack if hardmode is reached. this NPC should not be affected by that
			NPCID.Sets.DontDoHardmodeScaling[Type] = false;
			// Enemies can pick up coins, let's prevent it for this NPC
			NPCID.Sets.CantTakeLunchMoney[Type] = false;

			// Specify the debuffs it is immune to
			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData {
				SpecificallyImmuneTo = new int[] {
					BuffID.Wet,
					BuffID.Confused // Most NPCs have this
				}
			};
			NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            // Optional: If you don't want this NPC to show on the bestiary (if there is no reason to show a boss minion separately)
            // Make sure to remove SetBestiary code aswell
            // NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
            //	Hide = true // Hides this NPC from the bestiary
            // };
            // NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
				NPC.width = 306;
				NPC.height = 150;
				NPC.damage = 30;
				NPC.defense = 12;
				NPC.lifeMax = 5200;
				NPC.HitSound = SoundID.NPCHit1;
				NPC.DeathSound = SoundID.NPCDeath1;
				NPC.noGravity = true;
				NPC.noTileCollide = true;
				NPC.knockBackResist = 0f;
				NPC.netAlways = true;
				NPC.aiStyle = 69;
        }
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
			if (Main.hardMode || NPC.downedGolemBoss);
			{
				return SpawnCondition.Ocean.Chance * 0.15f;
			}
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			int itemType = ItemID.BubbleGun;
			var parameters = new DropOneByOne.Parameters()
			{
				ChanceNumerator = 1,
				ChanceDenominator = 3,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 1,
				MaximumItemDropsCount = 1,
			};
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				new FlavorTextBestiaryInfoElement("A giant powerful squid!")
			});
		}

		public override Color? GetAlpha(Color drawColor) {
			if (NPC.IsABestiaryIconDummy) {
			}
			return Color.White * NPC.Opacity;
		}
		public override void HitEffect(int hitDirection, double damage) {
			if (NPC.life <= 0) {
				// If this NPC dies, spawn some visuals

				int dustType = 75; // Some blue dust, read the dust guide on the wiki for how to find the perfect dust

				for (int i = 0; i < 20; i++) {
					Vector2 velocity = NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
					Dust dust = Dust.NewDustPerfect(NPC.Center, dustType, velocity, 26, Color.White, Main.rand.NextFloat(1.5f, 2.4f));

					dust.noLight = true;
					dust.noGravity = true;
					dust.fadeIn = Main.rand.NextFloat(0.3f, 0.8f);
				}
			}
		}
		public override void FindFrame(int frameHeight)
		{
			// This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
			// In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
			int startFrame = 0;
			int finalFrame = 9;
			int frameSpeed = 6;
			NPC.frameCounter += 0.5f;
			NPC.frameCounter += NPC.velocity.Length() / 10f; // Make the counter go faster with more movement speed
			if (NPC.frameCounter > frameSpeed)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y > finalFrame * frameHeight)
				{
					NPC.frame.Y = startFrame * frameHeight;
				}
			}
		}
	}
}
