using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class Arcane_WandT1 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arcane Wand");
			Tooltip.SetDefault("Tier 1");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 31;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 4;
			Item.height = 4;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = 5;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 8;
			Item.UseSound = SoundID.Item9;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.BulletHighVelocity;
			Item.shootSpeed = 15;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.MagicMissile, 1);
			recipe.AddIngredient(ModContent.ItemType<Basic_Arcane_Dust>(), 20);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddIngredient(ItemID.HellstoneBar, 12);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}