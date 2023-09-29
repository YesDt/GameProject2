using GameProject2.StateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.TimeZoneInfo;
using System.Reflection.Metadata;
using SharpDX.Direct2D1;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace GameProject2.Screens
{
    // This screen implements the actual game logic.
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        private SpriteFont _gameFont;

        private mcSprite _mc = new mcSprite();
        private CoinSprite[] _coins;

        private Texture2D _level;

        private int _coinsLeft;

        private Song _backgroundMusic;
        private SoundEffect _coinPickup;

        private bool _noCoinsLeft { get; set; } = false;


        //private Vector2 _enemyPosition = new Vector2(100, 100);

        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
        }

        // Load graphics content for the game
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _gameFont = _content.Load<SpriteFont>("gamefont");
            _level = _content.Load<Texture2D>("level");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            _mc.LoadContent(_content);
            _coins = new CoinSprite[]
            {
                new CoinSprite(new Vector2(300, 300)),
                new CoinSprite(new Vector2(700, 300)),
                new CoinSprite(new Vector2(5, 300)),
                new CoinSprite(new Vector2(100, 300)),
                new CoinSprite(new Vector2(543, 300)),
                new CoinSprite(new Vector2(723, 300)),
                new CoinSprite(new Vector2(400, 300)),
                new CoinSprite(new Vector2(1000, 300)),
                new CoinSprite(new Vector2(1100, 300)),
                new CoinSprite(new Vector2(392, 300))
            };
            _coinsLeft = _coins.Length;
            foreach (var coin in _coins) coin.LoadContent(_content);
            _coinPickup = _content.Load<SoundEffect>("Pickup_Coin15");
            _backgroundMusic = _content.Load<Song>("Project2music");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_backgroundMusic);
        }


        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // Apply some random jitter to make the enemy move around.
                //const float randomization = 10;

                //_enemyPosition.X += (float)(_random.NextDouble() - 0.5) * randomization;
                //_enemyPosition.Y += (float)(_random.NextDouble() - 0.5) * randomization;

                // Apply a stabilizing force to stop the enemy moving off the screen.
                var targetPosition = new Vector2(
                    ScreenManager.GraphicsDevice.Viewport.Width / 2 - _gameFont.MeasureString("Insert Gameplay Here").X / 2,
                    200);
                foreach (var coin in _coins)
                {
                    if (!coin.Collected && coin.Bounds.CollidesWith(_mc.Bounds))
                    {
                        coin.Collected = true;
                        _coinPickup.Play();
                        _coinsLeft--;
                        _mc.coinsCollected++;
                    }

                }
                if (_coinsLeft == 0)
                {
                    _noCoinsLeft = true;
                }
                if (_noCoinsLeft)
                {
                    MediaPlayer.Stop();
                    LoadingScreen.Load(ScreenManager, false, null, new MaintainenceScreen());
                }

                //_enemyPosition = Vector2.Lerp(_enemyPosition, targetPosition, 0.05f);

                // This game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                MediaPlayer.Pause();
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                MediaPlayer.Resume();

                _mc.Update(gameTime);
                // Otherwise move the player position.
                //var movement = Vector2.Zero;

                //if (keyboardState.IsKeyDown(Keys.Left))
                //    movement.X--;

                //if (keyboardState.IsKeyDown(Keys.Right))
                //    movement.X++;

                //if (keyboardState.IsKeyDown(Keys.Up))
                //    movement.Y--;

                //if (keyboardState.IsKeyDown(Keys.Down))
                //    movement.Y++;

                //var thumbstick = gamePadState.ThumbSticks.Left;

                //movement.X += thumbstick.X;
                //movement.Y -= thumbstick.Y;

                //if (movement.Length() > 1)
                //    movement.Normalize();

                //_playerPosition += movement * 8f;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float playerX = MathHelper.Clamp(_mc.Position.X, 300, 700);
            float offset = 300 - playerX;


            Matrix transform;
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            transform = Matrix.CreateTranslation(offset, 0, 0);
            spriteBatch.Begin(transformMatrix: transform);


            spriteBatch.Draw(_level, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0f);
            foreach (var coin in _coins)
            {
                coin.Draw(gameTime, spriteBatch);


            }
            _mc.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
