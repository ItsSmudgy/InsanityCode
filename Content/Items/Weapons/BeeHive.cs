using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class BeeHive : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bee Hive");
			Tooltip.SetDefault("Ya like jazz?");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 8;
			Item.height = 8;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = 1;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 3;
			Item.UseSound = SoundID.LiquidsHoneyWater;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Bee;
			Item.shootSpeed = 10;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BeeHive, 1);
			recipe.AddIngredient(ItemID.BeeGun, 1);
			recipe.AddIngredient(ItemID.Hive, 50);
			recipe.AddIngredient(ItemID.BottledHoney, 15);
			recipe.AddTile(TileID.HoneyDispenser);
			recipe.Register();
		}
	}
}