using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace SAE
{
    public enum BoutonPlay { Non, Oui };

    public class ScreenAccueil : GameScreen
    {
        private Game1 _myGame;
        private Texture2D _titre;
        private Texture2D _background;
        private Texture2D _play;
        private MouseState _mouseState;

        Rectangle rPlay = new Rectangle(450, 600, 150, 150);
        private BoutonPlay _clique;
        public MouseState MouseState { get => _mouseState; set => _mouseState = value; }
        public BoutonPlay Clique { get => _clique; set => _clique = value; }

        public ScreenAccueil(Game1 game) : base(game)
        {
            _myGame = game;
        }

        public override void LoadContent()
        {
            _titre = Content.Load<Texture2D>("./Images/arrowfight");
            _background = Content.Load<Texture2D>("./Images/bg");
            _play = Content.Load<Texture2D>("./Images/play");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            MouseState = Mouse.GetState();
            if (MouseState.LeftButton == ButtonState.Pressed && rPlay.Contains(MouseState.Position.X, MouseState.Position.Y))
                _clique = BoutonPlay.Oui;
        }

        public override void Draw(GameTime gameTime)
        {
            _myGame.GraphicsDevice.Clear(Color.DarkGray);
            _myGame.SpriteBatch.Begin();
            _myGame.SpriteBatch.Draw(_background, new Rectangle(0, 0, 1000, 1000), Color.YellowGreen);
            _myGame.SpriteBatch.Draw(_titre, new Vector2(50, 0), Color.White);
            _myGame.SpriteBatch.Draw(_play, new Rectangle(450, 600, 150, 150), Color.White);
            _myGame.SpriteBatch.End();
        }
    }
}
