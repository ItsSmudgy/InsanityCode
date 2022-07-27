using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items
{
	public class Basic_Arcane_Dust : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Basic Arcane Dust"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Manmade Pixie Dust");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}

		public override void SetDefaults()
		{
			Item.width = 64;
			Item.height = 64;
			Item.value = 250;
			Item.rare = 8;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SandBlock, 10);
			recipe.AddIngredient(ItemID.FallenStar, 2);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}