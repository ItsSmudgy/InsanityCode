using Insanity.Content.Projectiles.BaseProjectileClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Insanity.Content.NPCs.SpiritOfLight.Projectiles
{
    public class GemBombDeathray : BaseAttachedDeathray
    {
        public override string Texture => "Insanity/Assets/Textures/BlankSprite";
        public override string ProjectileName => "Gem Bomb Telegraph";

        public override float Scale => 0.25f;

        public override float Alpha => 0.65f;

        public override float TimeLeft => 60;

        public override int Parent => ModContent.ProjectileType<GemBomb>();

        public override Texture2D DeathrayFrontTexture => Mod.Assets.Request<Texture2D>("Assets/Textures/BasicDeathrayTextureFront").Value;

        public override Texture2D DeathrayMiddleTexture => Mod.Assets.Request<Texture2D>("Assets/Textures/BasicDeathrayTextureMiddle").Value;

        public override Texture2D DeathrayEndTexture => Mod.Assets.Request<Texture2D>("Assets/Textures/BasicDeathrayTextureEnd").Value;

        public override Color DeathrayColor => Color.Pink;
    }
}
