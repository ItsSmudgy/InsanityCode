using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using System.Collections.Generic;
using ReLogic.Content;

namespace Insanity.Content.NPCs
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadHead]
	public class Critter_Collector : ModNPC
	{
		public static bool Shop1 = true;
		public static bool Shop2 = false;
		public override void SetStaticDefaults() {
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has
			DisplayName.SetDefault("Critter Collector");
			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
			NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
				// Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
				// If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
			// NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
			NPC.Happiness
				.SetBiomeAffection<JungleBiome>(AffectionLevel.Like) // Example Person prefers the forest.
				.SetBiomeAffection<HallowBiome>(AffectionLevel.Like) // Example Person prefers the forest.
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
				.SetBiomeAffection<UndergroundBiome>(AffectionLevel.Hate) // Example Person dislikes the snow.
				.SetBiomeAffection<ForestBiome>(AffectionLevel.Love) // Example Person likes the Example Surface Biome
				.SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Love) // Loves living near the dryad.
				.SetNPCAffection(NPCID.Dryad, AffectionLevel.Like) // Likes living near the guide.
				.SetNPCAffection(NPCID.Princess, AffectionLevel.Like) // Likes living near the guide.
				.SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike) // Dislikes living near the merchant.
				.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate) // Hates living near the demolitionist.
			; // < Mind the semicolon!
		}

		public override void SetDefaults() {
			NPC.townNPC = true; // Sets NPC to be a Town NPC
			NPC.friendly = true; // NPC Will not attack player
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 10;
			NPC.defense = 15;
			NPC.lifeMax = 250;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;

			AnimationType = NPCID.Guide;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("This kid loves collecting critters, and would be glad to let you buy one to keep for yourself, too!"),
			});
		}

		// The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
		// Returning false will allow you to manually draw your NPC
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			// This code slowly rotates the NPC in the bestiary
			// (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers)) {
				drawModifiers.Rotation += 0.001f;

				// Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;
		}

		public override void HitEffect(int hitDirection, double damage) {
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++) {
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.JungleSpore);
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money) { // Requirements for the town NPC to spawn.
			for (int k = 0; k < 255; k++) {
				Player player = Main.player[k];
				if (!player.active) {
					continue;
				}

				// Player has to have either an ExampleItem or an ExampleBlock in order for the NPC to spawn
				if (player.inventory.Any(item => item.type == ItemID.Bunny || item.type == ItemID.Squirrel || item.type == ItemID.Bird || item.type == ItemID.Cardinal || item.type == ItemID.BlueJay || item.type == ItemID.SquirrelRed)) {
					return true;
				}
			}

			return false;
		}

		public override ITownNPCProfile TownNPCProfile() {
			return new CritterCollectorProfile();
		}

		public override List<string> SetNPCNameList() {
			return new List<string>() {
				"Noah",
				"David",
				"Jake",
				"Chris"
			};
		}

		public override void FindFrame(int frameHeight) {
			/*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int BestiaryGirl = NPC.FindFirstNPC(NPCID.BestiaryGirl);
			if (BestiaryGirl >= 0 && Main.rand.NextBool(4)) {
				chat.Add("I really wanna see the radioactive fox that bit " + Main.npc[BestiaryGirl].GivenName + ". It sounds so cool!");
				chat.Add("Have you seen what " + Main.npc[BestiaryGirl].GivenName + " is like during a full moon? She becomes so fluffy!");
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add("It's always the birds that are hard to catch...");
			chat.Add("Every time I catch a ladybug, I feel like I'm more lucky!");
			chat.Add("I don't get why critters don't run away from monsters... Are they just dumb?");
            chat.Add("I wonder how gold and gem critters were made?", 0.5);
			if (Main.hardMode)
			{
				chat.Add("I've tried catching this weird mushroom worm before, but it's really quick and keeps getting away!", 0.1);
			}
			if (NPC.downedPlantBoss)
			{
				chat.Add("I was exploring the Hallow when I saw this weird crystal butterfly. It looked pretty fragile, so I left it alone.", 0.1);
			}
			return chat; // chat is implicitly cast to a string.
		}
        public override void SetChatButtons(ref string button, ref string button2)
        {
            base.SetChatButtons(ref button, ref button2);
            button = "Critters";
			button2 = "Bugs";
		}
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
				Shop1 = true;
				Shop2 = false;
                shop = true;
            }
            else 
			{ 
				Shop1 = false;
				Shop2 = true;
				shop = true; 
			}
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
		{
			if (Shop1)
			{
				shop.item[nextSlot++].SetDefaults(ItemID.Bunny);
				shop.item[nextSlot++].SetDefaults(ItemID.Squirrel);
				shop.item[nextSlot++].SetDefaults(ItemID.SquirrelRed);
                shop.item[nextSlot++].SetDefaults(ItemID.Bird);
				shop.item[nextSlot++].SetDefaults(ItemID.Cardinal);
				shop.item[nextSlot++].SetDefaults(ItemID.BlueJay);
				shop.item[nextSlot++].SetDefaults(ItemID.Penguin);
				shop.item[nextSlot++].SetDefaults(ItemID.Duck);
				shop.item[nextSlot++].SetDefaults(ItemID.Grebe);
				shop.item[nextSlot++].SetDefaults(ItemID.Seagull);
				shop.item[nextSlot++].SetDefaults(ItemID.Owl);
				shop.item[nextSlot++].SetDefaults(ItemID.Mouse);
				shop.item[nextSlot++].SetDefaults(ItemID.Rat);
				shop.item[nextSlot++].SetDefaults(ItemID.Frog);
                shop.item[nextSlot++].SetDefaults(ItemID.Turtle);
				shop.item[nextSlot++].SetDefaults(ItemID.TurtleJungle);
                shop.item[nextSlot++].SetDefaults(ItemID.Goldfish);
                shop.item[nextSlot++].SetDefaults(ItemID.Pupfish);
				shop.item[nextSlot++].SetDefaults(ItemID.Seahorse);
			}
            else if (Shop2)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.Worm);
				if (Main.hardMode)
				{
					shop.item[nextSlot++].SetDefaults(ItemID.EnchantedNightcrawler);
				}
				if (NPC.downedFishron)
				{
					shop.item[nextSlot++].SetDefaults(ItemID.TruffleWorm);
				}
				shop.item[nextSlot++].SetDefaults(ItemID.Snail);
				if (Main.hardMode)
				{
					shop.item[nextSlot++].SetDefaults(ItemID.MagmaSnail);
					shop.item[nextSlot++].SetDefaults(ItemID.GlowingSnail);
				}
				shop.item[nextSlot++].SetDefaults(ItemID.Buggy);
				shop.item[nextSlot++].SetDefaults(ItemID.Sluggy);
				shop.item[nextSlot++].SetDefaults(ItemID.Grubby);
				shop.item[nextSlot++].SetDefaults(ItemID.Maggot);
				shop.item[nextSlot++].SetDefaults(ItemID.Grasshopper);
				shop.item[nextSlot++].SetDefaults(ItemID.WaterStrider);
                shop.item[nextSlot++].SetDefaults(ItemID.Scorpion);
				if (Main.hardMode)
				{
					shop.item[nextSlot++].SetDefaults(ItemID.BlackScorpion);
				}
				shop.item[nextSlot++].SetDefaults(ItemID.Firefly);
				if (Main.hardMode)
				{
					shop.item[nextSlot++].SetDefaults(ItemID.Lavafly);
					shop.item[nextSlot++].SetDefaults(ItemID.LightningBug);
                    shop.item[nextSlot++].SetDefaults(ItemID.FairyCritterBlue);
                    shop.item[nextSlot++].SetDefaults(ItemID.FairyCritterGreen);
					shop.item[nextSlot++].SetDefaults(ItemID.FairyCritterPink);
				}
                shop.item[nextSlot++].SetDefaults(ItemID.LadyBug);
				shop.item[nextSlot++].SetDefaults(ItemID.JuliaButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.MonarchButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.PurpleEmperorButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.RedAdmiralButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.SulphurButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.TreeNymphButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.UlyssesButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.ZebraSwallowtailButterfly);
				shop.item[nextSlot++].SetDefaults(ItemID.YellowDragonfly);
				shop.item[nextSlot++].SetDefaults(ItemID.RedDragonfly);
				shop.item[nextSlot++].SetDefaults(ItemID.BlackDragonfly);
				shop.item[nextSlot++].SetDefaults(ItemID.BlueDragonfly);
				shop.item[nextSlot++].SetDefaults(ItemID.OrangeDragonfly);
				shop.item[nextSlot++].SetDefaults(ItemID.GreenDragonfly);
				if (Main.hardMode)
				{
					shop.item[nextSlot++].SetDefaults(ItemID.HellButterfly);
				}
				if (NPC.downedEmpressOfLight)
				{
					shop.item[nextSlot++].SetDefaults(ItemID.EmpressButterfly);
				}
			}
		}

		// Make this Town NPC teleport to the King and/or Queen statue when triggered.
		public override bool CanGoToStatue(bool toKingStatue) => true;

		// Create a square of pixels around the NPC on teleport.
		public void StatueTeleport() {
			for (int i = 0; i < 30; i++) {
				Vector2 position = Main.rand.NextVector2Square(-20, 21);
				if (Math.Abs(position.X) > Math.Abs(position.Y)) {
					position.X = Math.Sign(position.X) * 20;
				}
				else {
					position.Y = Math.Sign(position.Y) * 20;
				}

				Dust.NewDustPerfect(NPC.Center + position, DustID.Grass, Vector2.Zero).noGravity = true;
			}
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 20;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 30;
			randExtraCooldown = 30;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
		 	projType = ProjectileID.PoisonDartBlowgun;
		 	attackDelay = 1;
		 }

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 12f;
			randomOffset = 2f;
		}
	}

	public class CritterCollectorProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc) {
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return ModContent.Request<Texture2D>("Insanity/Content/NPCs/Critter_Collector");

			if (npc.altTexture == 1)
				return ModContent.Request<Texture2D>("Insanity/Content/NPCs/Critter_Collector_Party");

			return ModContent.Request<Texture2D>("Insanity/Content/NPCs/Critter_Collector");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Insanity/Content/NPCs/Critter_Collector_Head");
	}
}