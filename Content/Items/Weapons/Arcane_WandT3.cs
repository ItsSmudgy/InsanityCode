using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class Arcane_WandT3 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arcane Wand");
			Tooltip.SetDefault("Tier 3");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 83;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 4;
			Item.height = 4;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = 5;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item9;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.MoonlordBullet;
			Item.shootSpeed = 25;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Arcane_WandT2>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Expert_Arcane_Dust>(), 20);
			recipe.AddIngredient(ItemID.TurtleShell, 5);
			recipe.AddIngredient(ItemID.BeetleHusk, 5);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}