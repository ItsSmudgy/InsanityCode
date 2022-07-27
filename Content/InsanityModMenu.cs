using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Insanity.Backgrounds;

namespace Insanity.Content
{
    public class InsanityTitleScreen : ModMenu
    {
        //star class
        public class Star
        {
            public int Time;

            public int MaxTime;

            public int Identity;

            public float Scale;

            public Color DrawColor;

            public Vector2 Center;

            public Vector2 Velocity;

            //construcctor for creating particles
            public Star(int maxTime, int identity, float scale, Vector2 velocity, Color drawColor, Vector2 startingPosition)
            {
                MaxTime = maxTime;
                Identity = identity;
                Scale = scale;
                Velocity = velocity;
                DrawColor = drawColor;
                Center = startingPosition; //this makes sure that the particle spawns at wherever you set the starting point
            }
        }

        public static List<Star> Stars = new List<Star>();
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<InsanityMenuBackgroundStyle>();
        public override Asset<Texture2D> SunTexture => Main.Assets.Request<Texture2D>("Images/MagicPixel");
        public override Asset<Texture2D> MoonTexture => Main.Assets.Request<Texture2D>("Images/MagicPixel");
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("Insanity/Assets/Textures/Backgrounds/Logo");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/The-Beginning-of-Insanity");

        public override string DisplayName => "Insanity";

        public override void OnSelected()
        {
            SoundEngine.PlaySound(Terraria.ID.SoundID.NPCHit5);
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            //[The Particle Code]
            //spawn particles
            Vector2 startPos = new Vector2((float)Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), (float)Main.screenHeight * 1.05f);
            for (int i = 0; i < 2; i++)
            {
                //random spawn chance :OPTIONAL:
                if (Main.rand.Next(4) == 0)
                {
                    //feel free to change these
                    Vector2 vel = -Vector2.UnitY * Main.rand.NextFloat(2f, 4f);
                    Color color = Color.White; //this is terraria's Rainbow Color, you can change this to Color.White to have it display the base texture color
                    float scale = Main.rand.NextFloat(0.35f, 1.25f);
                    int lifetime = Main.rand.Next(500, 600);
                    Stars.Add(new Star(lifetime, Stars.Count, scale, vel, color, startPos));
                }
            }
            Texture2D backgroundTexture = ModContent.Request<Texture2D>("Insanity/Assets/Textures/Backgrounds/Background").Value;
            Vector2 zero = Vector2.Zero;
            float screenWidth = Main.screenWidth / backgroundTexture.Width;
            float screenHeight = Main.screenHeight / backgroundTexture.Height;
            float num3 = screenWidth;
            if (screenWidth != screenHeight)
            {
                if (screenHeight > screenWidth)
                {
                    num3 = screenHeight;
                    zero.X -= (backgroundTexture.Width * num3 - Main.screenWidth) * 0.5f;
                }
                else
                {
                    zero.Y -= (backgroundTexture.Width * num3 - Main.screenHeight) * 0.5f;
                }
            }
            spriteBatch.Draw(backgroundTexture, zero, null, Color.White, 0f, Vector2.Zero, num3 * 1.25f, SpriteEffects.None, 0f);
            for (int j = 0; j < Stars.Count; j++)
            {
                Stars[j].Time++;
                Star star = Stars[j];
                star.Center += star.Velocity; //this is needed to make sure the particle moves with the velocity changes
            }
            Stars.RemoveAll((Star s) => s.Time >= s.MaxTime || s.Center.Y <= Main.screenHeight * -1.05f);
            Texture2D starTexture = Main.Assets.Request<Texture2D>(("Images/Star_" + Main.rand.Next(5)).ToString()).Value; //gets a random star vanilla star texture
            for (int k = 0; k < Stars.Count; k++)
            {
                Vector2 center = Stars[k].Center;
                spriteBatch.Draw(starTexture, center, null, Stars[k].DrawColor, 0f, starTexture.Size() / 2, Stars[k].Scale, SpriteEffects.None, 0f); //draws the stars
            }

            Vector2 position = new Vector2(Main.screenWidth / 2f, 100f);
            spriteBatch.Draw(Logo.Value, position, null, Color.White, logoRotation, Logo.Size() / 2, logoScale, SpriteEffects.None, 0f);
            //draw whatever else you need for the menu here or above all this 
            //NOTE: MAKE SURE TO RETURN FALSE IF YOU'RE GOING TO BE DRAWING A COMPLETELEY DIFFERENT BACKGROUND
            return false;
        }
    }
}