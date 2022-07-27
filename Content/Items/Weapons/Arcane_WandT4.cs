using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class Arcane_WandT4 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arcane Wand");
			Tooltip.SetDefault("Tier 4");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 104;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 4;
			Item.height = 4;
			Item.useTime = 7;
			Item.useAnimation = 7;
			Item.useStyle = 5;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 7;
			Item.UseSound = SoundID.Item9;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.GreenLaser;
			Item.shootSpeed = 30;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Arcane_WandT3>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Master_Arcane_Dust>(), 20);
			recipe.AddIngredient(ItemID.FragmentStardust, 10);
			recipe.AddIngredient(ItemID.FragmentVortex, 10);
			recipe.AddIngredient(ItemID.FragmentNebula, 10);
			recipe.AddIngredient(ItemID.FragmentSolar, 10);
			recipe.AddIngredient(ItemID.LunarBar, 20);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}