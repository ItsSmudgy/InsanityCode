using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Insanity.Content.Projectiles;
using Terraria.GameContent.Creative;

namespace Insanity.Content.Items.Weapons
{
    internal class SparkWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spark Wand"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Tooltip.SetDefault("Sparkle, sparkle!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.mana = 8;
            Item.damage = 50;
            Item.knockBack = 3.2f;

            Item.useTime = 20;
            Item.useAnimation = 15;

            Item.UseSound = SoundID.Item71;

            Item.shoot = ModContent.ProjectileType<Spark>();
            Item.shootSpeed = 1f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Flamelash, 1);
            recipe.AddIngredient(ItemID.WandofSparking, 1);
            recipe.AddIngredient(ItemID.FlowerofFire, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
