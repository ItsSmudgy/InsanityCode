using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class WaspNest : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wasp Nest");
			Tooltip.SetDefault("Ya like death?");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 52;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 5;
			Item.width = 8;
			Item.height = 8;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = 1;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 7;
			Item.UseSound = SoundID.LiquidsHoneyLava;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Bee;
			Item.shootSpeed = 5;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BeeHive>(), 5);
			recipe.AddIngredient(ItemID.WaspGun, 1);
			recipe.AddIngredient(ItemID.Hive, 100);
			recipe.AddTile(TileID.HoneyDispenser);
			recipe.Register();
		}
	}
}