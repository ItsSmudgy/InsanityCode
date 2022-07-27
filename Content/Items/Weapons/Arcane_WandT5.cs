using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Insanity.Content.Items;

namespace Insanity.Content.Items.Weapons
{
	public class Arcane_WandT5 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arcane Wand");
			Tooltip.SetDefault("Tier 5");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 135;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 4;
			Item.height = 4;
			Item.useTime = 10;
			Item.useAnimation = 5;
			Item.useStyle = 5;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = -12;
			Item.UseSound = SoundID.Item9;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.PurpleLaser;
			Item.shootSpeed = 15;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Arcane_WandT4>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Celestial_Arcane_Dust>(), 20);
			recipe.AddIngredient(ModContent.ItemType<Essence_of_Might>(), 25);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
	}
}