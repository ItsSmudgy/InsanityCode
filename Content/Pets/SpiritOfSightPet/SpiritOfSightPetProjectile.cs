using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insanity.Content.Pets.SpiritOfSightPet
{
	// You can find a simple pet example in Insanity\Content\Pets\ExamplePet
	// This pet uses custom AI and drawing to make it more special (It's a Master Mode boss pet after all)
	// It behaves similarly to the Creeper Egg or Suspicious Grinning Eye pets, but takes some visual properties from Insanity's Spirit Of Might
	public class SpiritOfSightPetProjectile : ModProjectile
	{
		public ref float AlphaForVisuals => ref Projectile.ai[0];

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Spirit Of Sight Pet");

			Main.projFrames[Projectile.type] = 6;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.EyeOfCthulhuPet); // Copy the stats of the Suspicious Grinning Eye projectile

			AIType = ProjectileID.EyeOfCthulhuPet;
		}
		public override bool PreAI()
		{
			Player player = Main.player[Projectile.owner];

			player.petFlagDD2Dragon = false; // Relic from aiType

			return true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			// Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
			if (!player.dead && player.HasBuff(ModContent.BuffType<SpiritOfSightPetBuff>()))
			{
				Projectile.timeLeft = 2;
			}
		}
	}
}
