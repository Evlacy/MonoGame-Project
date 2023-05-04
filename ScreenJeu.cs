using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SAE
{
    public class ScreenJeu : GameScreen
    {
        private Game1 _myGame;

        // Principale
        ushort tx;
        ushort ty;
        ushort fx;
        ushort fy;
        float compteurBouleDeFeu = 3;
        bool lancerBouleDeFeu = false;
        bool boulePlacer = false;
        bool detecter = false;

        // Secondaire
        float walkSpeed;
        float walkSpeed2;
        float speedFire;
        float compteurBouleDeFeuIA = 3;
        bool lancerBouleDeFeuIA = false;
        bool boulePlacerIA = false;

        Rectangle _rectPrincipale;
        Rectangle _rectSecondaire;
        Rectangle _rectFirePrincipale;
        Rectangle _rectFireSecondaire;
        Rectangle _rectFireIA;
        Rectangle _rectFireIRL;

        // Score et chrono
        private float _chrono;
        private int _scorePrincipale;
        private int _scoreSecondaire;
        private SpriteFont _textChrono;
        private SpriteFont _textPrincipale;
        private SpriteFont _textSecondaire;
        private Vector2 _positionChrono;
        private Vector2 _positionScorePrincipale;
        private Vector2 _positionScoreSecondaire;

        // Son
        private SoundEffect _fireballSound;
        private Song _song;
        private bool _soundIAplay = false;

        // Fire
        private string animationFirePrincipale;
        private AnimatedSprite _firePrincipale;
        private Vector2 _firePrincipalePosition;
        private string animationFireSecondaire;
        private AnimatedSprite _fireSecondaire;
        private Vector2 _fireSecondairePosition;
        private int _vitesseFire;

        // Perso
        private string animationPrincipale;
        private string animationSecondaire;
        private Vector2 _persoPrincipalePosition;
        private AnimatedSprite _persoPrincipale;
        private int _vitessePerso;
        private int _vitessePerso2;
        private Vector2 _persoSecondairePosition;
        private AnimatedSprite _persoSecondaire;

        // Map
        private TiledMap _tiledMap;
        private TiledMapRenderer _tiledMapRenderer;
        private TiledMapTileLayer mapLayer;

        // Tool
        public float Chrono { get => _chrono; set => _chrono = value; }
        public int ScorePrincipale { get => _scorePrincipale; set => _scorePrincipale = value; }
        public int ScoreSecondaire { get => _scoreSecondaire; set => _scoreSecondaire = value; }

        public override void Initialize()
        {
            animationPrincipale = "sud";
            animationSecondaire = "sud";

            _persoPrincipalePosition = new Vector2(450, 500);
            _persoSecondairePosition = new Vector2(550, 250);
            _firePrincipalePosition = new Vector2(-100, -100);
            _fireSecondairePosition = new Vector2(-100, -100);

            _vitessePerso = 150;
            _vitessePerso2 = 40;
            _vitesseFire = 200;
            _chrono = 60;

            ScorePrincipale = 3;
            ScoreSecondaire = 3;

            _positionChrono = new Vector2(440, 10);
            _positionScorePrincipale = new Vector2(50, 20);
            _positionScoreSecondaire = new Vector2(820, 20);
            base.Initialize();
        }

        public ScreenJeu(Game1 game) : base(game)
        {
            _myGame = game;
        }

        public override void LoadContent()
        {
            _tiledMap = Content.Load<TiledMap>("map");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
            _textChrono = Content.Load<SpriteFont>("file");
            _textPrincipale = Content.Load<SpriteFont>("file");
            _textSecondaire = Content.Load<SpriteFont>("file");

            _song = Content.Load<Song>("./Sons/song");
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_song);
            _fireballSound = Content.Load<SoundEffect>("./Sons/firesound");

            SpriteSheet spriteSheet1 = Content.Load<SpriteSheet>("./Sprites/principale.sf", new JsonContentLoader());
            _persoPrincipale = new AnimatedSprite(spriteSheet1);
            SpriteSheet spriteSheet2 = Content.Load<SpriteSheet>("./Sprites/secondaire.sf", new JsonContentLoader());
            _persoSecondaire = new AnimatedSprite(spriteSheet2);
            SpriteSheet spriteSheet3 = Content.Load<SpriteSheet>("./Sprites/fireprincipale.sf", new JsonContentLoader());
            _firePrincipale = new AnimatedSprite(spriteSheet3);
            SpriteSheet spriteSheet4 = Content.Load<SpriteSheet>("./Sprites/firesecondaire.sf", new JsonContentLoader());
            _fireSecondaire = new AnimatedSprite(spriteSheet4);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            mapLayer = _tiledMap.GetLayer<TiledMapTileLayer>("obstaclesEtRiviere");
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();

            if (_chrono > 0)
            {
                _rectPrincipale = new Rectangle((int)_persoPrincipalePosition.X - 20, (int)_persoPrincipalePosition.Y, 60, 80);
                _rectSecondaire = new Rectangle((int)_persoSecondairePosition.X - 20, (int)_persoSecondairePosition.Y, 60, 80);
                _rectFirePrincipale = new Rectangle((int)_firePrincipalePosition.X, (int)_firePrincipalePosition.Y, 20, 20);
                _rectFireSecondaire = new Rectangle((int)_fireSecondairePosition.X, (int)_fireSecondairePosition.Y, 20, 20);
                
                Chrono -= deltaSeconds;
                walkSpeed = deltaSeconds * _vitessePerso;
                walkSpeed2 = deltaSeconds * _vitessePerso2;
                speedFire = deltaSeconds * _vitesseFire;

                // Vie

                if (_rectFirePrincipale.Intersects(_rectSecondaire))
                {
                    ScoreSecondaire--;
                    _firePrincipalePosition = new Vector2(-100, -100);
                }

                if (_rectFireSecondaire.Intersects(_rectPrincipale))
                {
                    ScorePrincipale--;
                    _fireSecondairePosition = new Vector2(-100, -100);
                }

                if (_rectFirePrincipale.Intersects(_rectFireSecondaire))
                {
                    _firePrincipalePosition = new Vector2(-100, -100);
                    _fireSecondairePosition = new Vector2(-100, -100);
                }


                // Déplacement

                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    tx = (ushort)(_persoPrincipalePosition.X / _tiledMap.TileWidth - 1);
                    ty = (ushort)(_persoPrincipalePosition.Y / _tiledMap.TileHeight);
                    animationPrincipale = "ouest";

                    if (!IsCollision(tx, ty))
                        _persoPrincipalePosition.X -= walkSpeed;

                    _persoPrincipale.Play(animationPrincipale);
                    _persoPrincipale.Update(deltaSeconds);
                }

                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    tx = (ushort)(_persoPrincipalePosition.X / _tiledMap.TileWidth + 1);
                    ty = (ushort)(_persoPrincipalePosition.Y / _tiledMap.TileHeight);
                    animationPrincipale = "est";

                    if (!IsCollision(tx, ty))
                        _persoPrincipalePosition.X += walkSpeed;

                    _persoPrincipale.Play(animationPrincipale);
                    _persoPrincipale.Update(deltaSeconds);
                }

                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    tx = (ushort)(_persoPrincipalePosition.X / _tiledMap.TileWidth);
                    ty = (ushort)(_persoPrincipalePosition.Y / _tiledMap.TileHeight - 1);
                    animationPrincipale = "nord";

                    if (!IsCollision(tx, ty))
                        _persoPrincipalePosition.Y -= walkSpeed;

                    _persoPrincipale.Play(animationPrincipale);
                    _persoPrincipale.Update(deltaSeconds);
                }

                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    tx = (ushort)(_persoPrincipalePosition.X / _tiledMap.TileWidth);
                    ty = (ushort)(_persoPrincipalePosition.Y / _tiledMap.TileHeight + 1);
                    animationPrincipale = "sud";

                    if (!IsCollision(tx, ty))
                        _persoPrincipalePosition.Y += walkSpeed;

                    _persoPrincipale.Play(animationPrincipale);
                    _persoPrincipale.Update(deltaSeconds);
                }


                // Fire

                if (keyboardState.IsKeyDown(Keys.Enter) && lancerBouleDeFeu == false)
                {
                    lancerBouleDeFeu = true;
                    _fireballSound.Play();
                    lancerBouleDeFeuIA = true;
                }

                if (compteurBouleDeFeu < 0)
                {
                    lancerBouleDeFeu = false;
                    compteurBouleDeFeu = 3;
                    _firePrincipalePosition = new Vector2(-100, -100);
                    boulePlacer = false;
                }


                if (lancerBouleDeFeu)
                {
                    if (boulePlacer == false)
                    {
                        _firePrincipalePosition = _persoPrincipalePosition;
                        animationFirePrincipale = animationPrincipale;
                    }

                    boulePlacer = true;
                    fx = (ushort)(_firePrincipalePosition.X / _tiledMap.TileWidth - 1);
                    fy = (ushort)(_firePrincipalePosition.Y / _tiledMap.TileHeight);
                    mapLayer = _tiledMap.GetLayer<TiledMapTileLayer>("obstacles");

                    if (!IsCollision(fx, fy))
                    {
                        if (animationFirePrincipale == "ouest")
                            _firePrincipalePosition.X -= speedFire;
                        else if (animationFirePrincipale == "est")
                            _firePrincipalePosition.X += speedFire;
                        else if (animationFirePrincipale == "sud")
                            _firePrincipalePosition.Y += speedFire;
                        else
                            _firePrincipalePosition.Y -= speedFire;
                    }
                    else
                    {
                        _firePrincipalePosition = new Vector2(-100, -100);
                    }

                    mapLayer = _tiledMap.GetLayer<TiledMapTileLayer>("obstaclesEtRiviere");
                    compteurBouleDeFeu -= deltaSeconds;
                    _firePrincipale.Play(animationPrincipale);
                    _firePrincipale.Update(deltaSeconds);
                }

                _rectFireIA = new Rectangle((int)_persoSecondairePosition.X, (int)_persoSecondairePosition.Y, 250, 250);
                _rectFireIRL = new Rectangle((int)_persoPrincipalePosition.X, (int)_persoPrincipalePosition.Y, 250, 250);

                // IA Deplacement

                if (_persoPrincipalePosition.X < _persoSecondairePosition.X && _persoPrincipalePosition.Y < _persoSecondairePosition.Y)
                {
                    tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth);
                    ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight - 1);
                    animationSecondaire = "nord";
                    if (!IsCollision(tx, ty))
                        _persoSecondairePosition.Y -= walkSpeed2;
                    else
                    {
                        tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth + 1);
                        ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight);
                        if (!IsCollision(tx, ty))
                        {
                            animationSecondaire = "est";
                            _persoSecondairePosition.X += walkSpeed2;
                        }
                        else
                        {
                            animationSecondaire = "ouest";
                            _persoSecondairePosition.X -= walkSpeed2;
                        }
                    }

                    _persoSecondaire.Play(animationSecondaire);
                    _persoSecondaire.Update(deltaSeconds);
                }

                if (_persoPrincipalePosition.X > _persoSecondairePosition.X && _persoPrincipalePosition.Y < _persoSecondairePosition.Y)
                {
                    tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth + 1);
                    ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight);
                    animationSecondaire = "est";
                    if (!IsCollision(tx, ty))
                        _persoSecondairePosition.X += walkSpeed2;
                    else
                    {
                        tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth);
                        ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight - 1);
                        if (!IsCollision(tx, ty))
                        {
                            animationSecondaire = "nord";
                            _persoSecondairePosition.Y -= walkSpeed2;
                        }
                        else
                        {
                            animationSecondaire = "sud";
                            _persoSecondairePosition.Y += walkSpeed2;
                        }
                    }

                    _persoSecondaire.Play(animationSecondaire);
                    _persoSecondaire.Update(deltaSeconds);
                }

                if (_persoPrincipalePosition.X > _persoSecondairePosition.X && _persoPrincipalePosition.Y > _persoSecondairePosition.Y)
                {
                    tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth);
                    ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight + 1);
                    animationSecondaire = "sud";
                    if (!IsCollision(tx, ty))
                        _persoSecondairePosition.Y += walkSpeed2;
                    else
                    {
                        tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth + 1);
                        ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight);
                        if (!IsCollision(tx, ty))
                        {
                            animationSecondaire = "est";
                            _persoSecondairePosition.X += walkSpeed2;
                        }
                        else
                        {
                            animationSecondaire = "ouest";
                            _persoSecondairePosition.X -= walkSpeed2;
                        }
                    }

                    _persoSecondaire.Play(animationSecondaire);
                    _persoSecondaire.Update(deltaSeconds);
                }

                if (_persoPrincipalePosition.X < _persoSecondairePosition.X && _persoPrincipalePosition.Y > _persoSecondairePosition.Y)
                {
                    tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth - 1);
                    ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight);
                    animationSecondaire = "ouest";
                    if (!IsCollision(tx, ty))
                        _persoSecondairePosition.X -= walkSpeed2;
                    else
                    {
                        tx = (ushort)(_persoSecondairePosition.X / _tiledMap.TileWidth);
                        ty = (ushort)(_persoSecondairePosition.Y / _tiledMap.TileHeight + 1);
                        if (!IsCollision(tx, ty))
                        {
                            animationSecondaire = "sud";
                            _persoSecondairePosition.Y += walkSpeed2;
                        }
                        else
                        {
                            animationSecondaire = "nord";
                            _persoSecondairePosition.Y -= walkSpeed2;
                        }
                    }

                    _persoSecondaire.Play(animationSecondaire);
                    _persoSecondaire.Update(deltaSeconds);
                }

                // IA Fire

                if (_rectFireIA.Intersects(_rectFireIRL))
                {
                    if (_soundIAplay == false)
                    {
                        _fireballSound.Play();
                        _soundIAplay = true;
                        lancerBouleDeFeuIA = true;
                    }
                }


                if (compteurBouleDeFeuIA < 0)
                {
                    lancerBouleDeFeuIA = false;
                    compteurBouleDeFeuIA = 3;
                    _fireSecondairePosition = new Vector2(-100, -100);
                    boulePlacerIA = false;
                    _soundIAplay = false;
                    detecter = false;
                }


                if (lancerBouleDeFeuIA)
                {

                    fx = (ushort)(_fireSecondairePosition.X / _tiledMap.TileWidth);
                    fy = (ushort)(_fireSecondairePosition.Y / _tiledMap.TileHeight);
                    mapLayer = _tiledMap.GetLayer<TiledMapTileLayer>("obstacles");

                    if (animationSecondaire == "ouest" && detecter == false)
                    {
                        if (boulePlacerIA == false)
                        {
                            _fireSecondairePosition = _persoSecondairePosition;
                            boulePlacerIA = true;
                        }
                        detecter = true;
                        animationFireSecondaire = "ouest";
                    }
                    else if (animationSecondaire == "est" && detecter == false)
                    {
                        if (boulePlacerIA == false)
                        {
                            _fireSecondairePosition = _persoSecondairePosition;
                            boulePlacerIA = true;
                        }
                        detecter = true;
                        animationFireSecondaire = "est";
                    }
                    else if (animationSecondaire=="sud" && detecter == false)
                    {
                        if (boulePlacerIA == false)
                        {
                            _fireSecondairePosition = _persoSecondairePosition;
                            boulePlacerIA = true;
                        }
                        detecter = true;
                        animationFireSecondaire = "sud";

                    }
                    else if (animationSecondaire == "nord" && detecter == false)
                    {
                        if (boulePlacerIA == false)
                        {
                            _fireSecondairePosition = _persoSecondairePosition;
                            boulePlacerIA = true;
                        }
                        detecter = true;
                        animationFireSecondaire = "nord";
                    }

                    if (detecter == true)
                    {
                        if (!IsCollision(fx, fy))
                        {
                            if (animationFireSecondaire == "ouest")
                            {
                                _fireSecondairePosition.X -= speedFire;

                            }
                            else if (animationFireSecondaire == "est")
                            {
                                _fireSecondairePosition.X += speedFire;

                            }
                            else if (animationFireSecondaire == "sud")
                            {
                                _fireSecondairePosition.Y += speedFire;

                            }
                            else if (animationFireSecondaire == "nord")
                            {
                                _fireSecondairePosition.Y -= speedFire;

                            }
                        }
                        else
                        {
                            _fireSecondairePosition = new Vector2(-100, -100);
                        }

                        mapLayer = _tiledMap.GetLayer<TiledMapTileLayer>("obstaclesEtRiviere");
                        compteurBouleDeFeuIA -= deltaSeconds;
                        _fireSecondaire.Play(animationFireSecondaire);
                        _fireSecondaire.Update(deltaSeconds);
                    }
                }
            }
            else
            {
                _chrono = -1;
            }

            _tiledMapRenderer.Update(gameTime);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }


        public override void Draw(GameTime gameTime)
        {
            _myGame.SpriteBatch.Begin();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _tiledMapRenderer.Draw();
            if (_chrono > 0)
            {
                _myGame.SpriteBatch.DrawString(_textChrono, $"Chrono {Math.Round(_chrono, 0)}\n Reload {Math.Round(compteurBouleDeFeu, 0)} ", _positionChrono, Color.Blue);
            }
            _myGame.SpriteBatch.DrawString(_textPrincipale, $"Vie : {ScorePrincipale}", _positionScorePrincipale, Color.DarkRed);
            _myGame.SpriteBatch.DrawString(_textSecondaire, $"Vie : {ScoreSecondaire}", _positionScoreSecondaire, Color.DarkRed);
            _myGame.SpriteBatch.Draw(_persoPrincipale, _persoPrincipalePosition);
            _myGame.SpriteBatch.Draw(_firePrincipale, _firePrincipalePosition);
            _myGame.SpriteBatch.Draw(_fireSecondaire, _fireSecondairePosition);
            _myGame.SpriteBatch.Draw(_persoSecondaire, _persoSecondairePosition);
            _myGame.SpriteBatch.End();
        }


        private bool IsCollision(ushort x, ushort y)
        {
            TiledMapTile? tile;
            if (mapLayer.TryGetTile(x, y, out tile) == false)
                return false;
            if (!tile.Value.IsBlank)
                return true;
            return false;
        }

    }

}
