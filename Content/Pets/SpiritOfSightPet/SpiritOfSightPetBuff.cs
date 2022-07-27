using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Insanity.Content.Pets.SpiritOfSightPet
{
	// You can find a simple pet example in Insanity\Content\Pets\ExamplePet
	public class SpiritOfSightPetBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Spirit of Sight");
			Description.SetDefault("A miniature Spirit Of Sight is following you");

			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) { // This method gets called every frame your buff is active on your player.
			player.buffTime[buffIndex] = 18000;

			int projType = ModContent.ProjectileType<SpiritOfSightPetProjectile>();

			// If the player is local, and there hasn't been a pet projectile spawned yet - spawn it.
			if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0) {
				var entitySource = player.GetSource_Buff(buffIndex);

				Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
			}
		}
	}
}
