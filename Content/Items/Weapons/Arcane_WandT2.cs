using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class Arcane_WandT2 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arcane Wand");
			Tooltip.SetDefault("Tier 2");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 52;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = 4;
			Item.height = 4;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = 5;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 10;
			Item.UseSound = SoundID.Item9;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.MiniRetinaLaser;
			Item.shootSpeed = 20;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Arcane_WandT1>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Advanced_Arcane_Dust>(), 20);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
			recipe.AddIngredient(ItemID.SoulofFlight, 10);
			recipe.AddIngredient(ItemID.SoulofSight, 10);
			recipe.AddIngredient(ItemID.SoulofMight, 10);
			recipe.AddIngredient(ItemID.SoulofFright, 10);
			recipe.AddIngredient(ItemID.HallowedBar, 15);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}