using Insanity.Content.Pets.SpiritOfMightPet;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Insanity.Content.Pets.SpiritOfMightPet
{
	public class SpiritOfMightPetItem : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Malfunctioning Might Probe");
			Tooltip.SetDefault("Summons a friendly Might Probe to follow you!");
			
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ZephyrFish); // Copy the Defaults of the Zephyr Fish Item.
			Item.rare = ItemRarityID.Master;
			Item.master = true;
			Item.shoot = ModContent.ProjectileType<SpiritOfMightPetProjectile>(); // "Shoot" your pet projectile.
			Item.buffType = ModContent.BuffType<SpiritOfMightBuff>(); // Apply buff upon usage of the Item.
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) {
				player.AddBuff(Item.buffType, 3600);
			}
		}
	}
}
