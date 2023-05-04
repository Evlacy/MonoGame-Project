using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace SAE
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Screens 
        private ScreenAccueil _screenAccueil;
        private ScreenJeu _screenJeu;
        private ScreenEnd _screenEnd;
        private Screen _ecranAffiche;
        public enum Screen { Accueil, Jeu, End };

        // Tool
        private readonly ScreenManager _screenManager;
        public SpriteBatch SpriteBatch { get => _spriteBatch; set => _spriteBatch = value; }
        public Screen EcranAffiche { get => _ecranAffiche; set => _ecranAffiche = value; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _screenManager = new ScreenManager();
            Components.Add(_screenManager);
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _screenAccueil = new ScreenAccueil(this);
            _screenJeu = new ScreenJeu(this);
            _screenEnd = new ScreenEnd(this);
            _screenManager.LoadScreen(_screenAccueil, new FadeTransition(GraphicsDevice, Color.Black));
            EcranAffiche = Screen.Accueil;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Debut

            if (EcranAffiche == Screen.Accueil && _screenAccueil.Clique == BoutonPlay.Oui)
            {
                EcranAffiche = Screen.Jeu;
                _screenManager.LoadScreen(_screenJeu, new FadeTransition(GraphicsDevice, Color.Black));
            }

            // Rejouer

            if (EcranAffiche == Screen.End && _screenEnd.Reponse == Rejouer.Oui)
            {
                EcranAffiche = Screen.Jeu;
                _screenManager.LoadScreen(_screenJeu, new FadeTransition(GraphicsDevice, Color.Black));
            }

            // Parties

            if (EcranAffiche == Screen.Jeu && _screenJeu.ScorePrincipale == -1)
            {
                EcranAffiche = Screen.End;
                _screenManager.LoadScreen(_screenEnd, new FadeTransition(GraphicsDevice, Color.Black));
                _screenEnd.Fin = FinDePartie.defaite;
            }

            if (EcranAffiche == Screen.Jeu && _screenJeu.ScoreSecondaire == -1)
            {

                EcranAffiche = Screen.End;
                _screenManager.LoadScreen(_screenEnd, new FadeTransition(GraphicsDevice, Color.Black));
                _screenEnd.Fin = FinDePartie.victoire;
            }

            if (EcranAffiche == Screen.Jeu && _screenJeu.ScoreSecondaire < _screenJeu.ScorePrincipale && _screenJeu.Chrono == -1)
            {
                EcranAffiche = Screen.End;
                _screenManager.LoadScreen(_screenEnd, new FadeTransition(GraphicsDevice, Color.Black));
                _screenEnd.Fin = FinDePartie.victoire;
            }
            else if (EcranAffiche == Screen.Jeu && _screenJeu.ScoreSecondaire > _screenJeu.ScorePrincipale && _screenJeu.Chrono == -1)
            {
                EcranAffiche = Screen.End;
                _screenManager.LoadScreen(_screenEnd, new FadeTransition(GraphicsDevice, Color.Black));
                _screenEnd.Fin = FinDePartie.defaite;
            }

            // Chrono

            if (EcranAffiche == Screen.Jeu && _screenJeu.Chrono == -1 && _screenJeu.ScorePrincipale == _screenJeu.ScoreSecondaire)
            {
                EcranAffiche = Screen.End;
                _screenManager.LoadScreen(_screenEnd, new FadeTransition(GraphicsDevice, Color.Black));
                _screenEnd.Fin = FinDePartie.egalite;
            }

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin();
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
