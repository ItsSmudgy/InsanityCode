using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items
{
	public class Expert_Arcane_Dust : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Expert Arcane Dust"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Despite it's name, the world does not have to be expert mode for it.");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}

		public override void SetDefaults()
		{
			Item.width = 64;
			Item.height = 64;
			Item.value = 750;
			Item.rare = 1;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Advanced_Arcane_Dust>(), 10);
			recipe.AddIngredient(ItemID.SoulofSight, 5);
			recipe.AddIngredient(ItemID.SoulofMight, 5);
			recipe.AddIngredient(ItemID.SoulofFright, 5);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}