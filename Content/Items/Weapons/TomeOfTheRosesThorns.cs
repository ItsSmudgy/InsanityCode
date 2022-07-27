using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Insanity.Content.Projectiles;

namespace Insanity.Content.Items.Weapons
{
	public class TomeOfTheRosesThorns : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Rose's Thorns");
            Tooltip.SetDefault("Every rose has its thorn.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			Item.damage = 29;
			Item.DamageType = DamageClass.Magic; // Makes the damage register as magic. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type.
			Item.width = 34;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot; // Makes the player use a 'Shoot' use style for the Item.
			Item.noMelee = true; // Makes the item not do damage with it's melee hitbox.
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item17;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.SeedlerThorn;
			Item.shootSpeed = 20; // How fast the item shoots the projectile.
			Item.crit = 32; // The percent chance at hitting an enemy with a crit, plus the default amount of 4.
			Item.mana = 11; // This is how much mana the item uses.
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.JungleRose, 1);
			recipe.AddIngredient(ItemID.Stinger, 10);
			recipe.AddIngredient(ItemID.JungleSpores, 10);
			recipe.AddIngredient(ItemID.Book, 1);
			recipe.AddTile(TileID.Bookcases);
			recipe.Register();
		}
	}
}
