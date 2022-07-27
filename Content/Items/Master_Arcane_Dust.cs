using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items
{
	public class Master_Arcane_Dust : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Master Arcane Dust"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Despite it's name, the world does not have to be master mode for it.");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}

		public override void SetDefaults()
		{
			Item.width = 64;
			Item.height = 64;
			Item.value = 1000;
			Item.rare = 7;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Expert_Arcane_Dust>(), 10);
			recipe.AddIngredient(ItemID.FragmentStardust, 5);
			recipe.AddIngredient(ItemID.FragmentVortex, 5);
			recipe.AddIngredient(ItemID.FragmentNebula, 5);
			recipe.AddIngredient(ItemID.FragmentSolar, 5);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}