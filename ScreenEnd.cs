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


namespace SAE
{
    public enum FinDePartie { victoire, defaite, egalite };
    public enum Rejouer { Oui, Non };

    public class ScreenEnd : GameScreen
    {
        private Game1 _myGame; 
        private Texture2D _victoire;
        private Texture2D _defaite;
        private Texture2D _egalite;
        private Texture2D _replay;
        private Texture2D _background;
        private FinDePartie _fin;
        private Rejouer _reponse;
        private KeyboardState _keyboardState;

        public FinDePartie Fin { get => _fin; set => _fin = value; }
        public Rejouer Reponse { get => _reponse; set => _reponse = value; }
        public KeyboardState KeyboardState { get => _keyboardState; set => _keyboardState = value; }

        public ScreenEnd(Game1 game) : base(game)
        {
            _myGame = game;
        }

        public override void Initialize()
        {
            _reponse = Rejouer.Non;
        }

        public override void LoadContent()
        {
            _background = Content.Load<Texture2D>("./Images/bg");
            _victoire = Content.Load<Texture2D>("./Images/victoire");
            _defaite = Content.Load<Texture2D>("./Images/defaite");
            _egalite = Content.Load<Texture2D>("./Images/egalite");
            _replay = Content.Load<Texture2D>("./Images/Replay");
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState = Keyboard.GetState();
            if (_keyboardState.IsKeyDown(Keys.Enter))
                _reponse = Rejouer.Oui;
        }

        public override void Draw(GameTime gameTime)
        {
            _myGame.GraphicsDevice.Clear(Color.DarkGray);
            _myGame.SpriteBatch.Begin();
            _myGame.SpriteBatch.Draw(_background, new Rectangle(0, 0, 1000, 1000), Color.YellowGreen);
            _myGame.SpriteBatch.Draw(_replay, new Rectangle(-150, 0, 1200, 1200), Color.White);
            if (_fin == FinDePartie.victoire)
            { _myGame.SpriteBatch.Draw(_victoire, new Rectangle(-150, -320, 1200, 1200), Color.White); }
            else if (_fin == FinDePartie.defaite)
            { _myGame.SpriteBatch.Draw(_defaite, new Rectangle(-150, -320, 1200, 1200), Color.White); }
            else if (_fin == FinDePartie.egalite)
            { _myGame.SpriteBatch.Draw(_egalite, new Rectangle(-150, -320, 1200, 1200), Color.White); }
            _myGame.SpriteBatch.End();
        }
    }

}
