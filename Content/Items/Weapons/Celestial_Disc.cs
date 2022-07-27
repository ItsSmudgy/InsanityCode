using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Insanity.Content.Projectiles;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
    internal class Celestial_Disc : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Disc"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.mana = 8;
            Item.damage = 96;
            Item.knockBack = 3.2f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Celestial_Disc_Proj>();
            Item.shootSpeed = 12f;
            Item.noUseGraphic = true;
            Item.consumable = false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LightDisc, 5);
            recipe.AddIngredient(ItemID.FragmentStardust, 10);
            recipe.AddIngredient(ItemID.FragmentVortex, 10);
            recipe.AddIngredient(ItemID.FragmentNebula, 10);
            recipe.AddIngredient(ItemID.FragmentSolar, 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
