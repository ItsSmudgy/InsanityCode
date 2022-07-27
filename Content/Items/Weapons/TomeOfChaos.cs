using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Insanity.Content.Projectiles;

namespace Insanity.Content.Items.Weapons
{
	public class TomeOfChaos : ModItem
	{
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Tome of Gun");
            Tooltip.SetDefault("Mama I just killed a man.");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			int v1 = 999999999;
			Item.damage = v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1 + v1;
			Item.DamageType = DamageClass.Magic; // Makes the damage register as magic. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type.
			Item.width = 34;
			Item.height = 40;
			Item.useTime = 0;
			Item.useAnimation = 0;
			Item.useStyle = ItemUseStyleID.Shoot; // Makes the player use a 'Shoot' use style for the Item.
			Item.noMelee = true; // Makes the item not do damage with it's melee hitbox.
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Master;
			Item.UseSound = SoundID.Item36;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.ChlorophyteBullet;
			Item.shootSpeed = 30; // How fast the item shoots the projectile.
			Item.crit = 100; // The percent chance at hitting an enemy with a crit, plus the default amount of 4.
		}
	}
}
