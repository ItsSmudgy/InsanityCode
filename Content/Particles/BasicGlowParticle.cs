using System;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insanity.Common.Systems.ParticleSystem;
using Terraria.ModLoader;
using ReLogic.Content;

namespace Insanity.Content.Particles
{
    public class BasicGlowParticle : Particle
    {
        public override string Texture => "Insanity/Assets/Textures/BloomLight";

        public override bool UseAdditiveBlending => true;
        public override bool UseCustomDrawing => true;

        private float Opacity;
        private Color BaseColor;
        private bool AffectedByWindSpeed;
        private bool DynamicScaleDecrease;

        public BasicGlowParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime, bool affectedByWindSpeed = false, bool dynamicScaleDecrease = false)
        {
            Position = position;
            Velocity = velocity;
            BaseColor = color;
            Scale = scale;
            MaxTime = maxTime;
            AffectedByWindSpeed = affectedByWindSpeed;
            DynamicScaleDecrease = dynamicScaleDecrease;
        }

        public override void Update()
        {
            Lighting.AddLight(Position, Color.ToVector3() * Scale);
            Opacity = Utils.GetLerpValue(MaxTime, LifeTime / MaxTime, LifeTime, true);
            Color = BaseColor * Opacity;

            Velocity *= 0.99f;
            if (AffectedByWindSpeed)
                Velocity.X = Velocity.X + Main.windSpeedCurrent * 0.07f;
            if (DynamicScaleDecrease)
                Scale *= Utils.GetLerpValue(MaxTime, LifeTime / MaxTime, LifeTime, true);

            if (Scale <= 0 || LifeTime >= MaxTime)
                Kill();
        }

        public override void CustomDrawing(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
            Texture2D bloom = ModContent.Request<Texture2D>("Insanity/Assets/Textures/BloomCircle", (AssetRequestMode)2).Value;
            spriteBatch.Draw(bloom, Position - Main.screenPosition, null, Color, Rotation, bloom.Size() / 2, Scale / 5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color, Rotation, texture.Size() / 2f, Scale, SpriteEffects.None, 0f);
        }
    }
}
