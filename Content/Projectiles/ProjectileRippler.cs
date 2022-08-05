using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Insanity.Content.Projectiles
{
    public class ProjectileRippler : ModProjectile
    {
        public ref float Width => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float ProjType => ref Projectile.ai[1];
     
        public override string Texture => "Terraria/Images/Projectile_260";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockwave Spawner");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] % Timer == 0 && Projectile.ai[0] < 120)
            {
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector2 center = new Vector2(Projectile.Center.X, Projectile.Center.Y + Projectile.height / 4);
                    int numtries = 0;
                    center.X -= (Projectile.ai[0] - 1f) * Width * i;
                    int x = (int)(center.X / 16);
                    int y = (int)(center.Y / 16);
                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                    {
                        y++;
                        center.Y = y * 16;
                    }
                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 20)
                    {
                        numtries++;
                        y--;
                        center.Y = y * 16;
                    }
                    Tile tile = Main.tile[x, y];
                    if (numtries >= 20 && tile.WallType == 0 && Main.tileSolid[tile.TileType])
                        break;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, Vector2.Zero, (int)Projectile.ai[1], Projectile.damage / 2, 10f, Main.myPlayer);
                    }
                }
            }

            if (Projectile.ai[0] >= 100)
                Projectile.Kill();
        }
    }
}
