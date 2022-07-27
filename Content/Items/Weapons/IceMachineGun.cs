using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class IceMachineGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ice Machine Gun");
			Tooltip.SetDefault("The power of ice is in your hands.");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 64;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 25;
			Item.width = 8;
			Item.height = 8;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = 5;
			Item.knockBack = 0;
			Item.value = 10000;
			Item.rare = 8;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Blizzard;
			Item.shootSpeed = 10;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BlizzardStaff, 1);
			recipe.AddIngredient(ItemID.SnowballCannon, 1);
			recipe.AddIngredient(ItemID.IceBlock, 100);
			recipe.AddIngredient(ItemID.SnowBlock, 50);
			recipe.AddTile(TileID.IceMachine);
			recipe.Register();
		}
	}
}