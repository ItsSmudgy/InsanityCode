using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
	public class DeathsScythe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Death's Scythe"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Not to be confused with Death Sickle!");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 86;
			Item.DamageType = DamageClass.Melee;
			Item.width = 64;
			Item.height = 64;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 7;
			Item.value = 10000;
			Item.rare = 8;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ProjectileID.DemonScythe;
			Item.shootSpeed = 10;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DeathSickle, 1);
			recipe.AddIngredient(ItemID.Bone, 333);
			recipe.AddIngredient(ItemID.SpookyWood, 333);
			recipe.AddIngredient(ItemID.Skull, 1);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}