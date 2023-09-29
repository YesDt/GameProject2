using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameProject2.Collisions;

namespace GameProject2
{
    /// <summary>
    /// States the main character will be in
    /// </summary>
    public enum Action
    {
        Idle = 0,
        Running = 1,
    }

    /// <summary>
    /// Class for the main character sprite
    /// </summary>
    public class mcSprite
    {
        private Texture2D _texture;

        private Vector2 _position = new Vector2(200, 300);


        KeyboardState currentKeyboardState;
        KeyboardState priorKeyboardState;

        private BoundingRectangle _bounds = new BoundingRectangle(new Vector2(200 - 32, 300 - 32), 48, 130);

        private Vector2 gravity;

        private double _animationTimer;

        private short _animationFrame;

        private bool _flipped;

        //private double _jumpTimer;

        public Action action;

        private Vector2 direction;

        public int coinsCollected;


        public Vector2 Position => _position;
        
        /// <summary>
        /// Boundaries for the bounding rectangle of the sprite
        /// </summary>
        public BoundingRectangle Bounds => _bounds;

        /// <summary>
        /// Loads the Main character sprite
        /// </summary>
        /// <param name="content">ContentManager</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_MC");


        }


        /// <summary>
        /// Updates the Main character
        /// </summary>
        /// <param name="gameTime">The real time elapsed in the game</param>
        public void Update(GameTime gameTime)
        {
            direction = new Vector2(200 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            gravity = new Vector2(0, 10 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            priorKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            if (_position.Y < 0)
            {
                _position += gravity;
            }


            for(int i = 0; i < coinsCollected; i++)
            {
                direction += new Vector2(0.75f, 0);
                if (direction.X > 300) direction.X = 300;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A) ||
                currentKeyboardState.IsKeyDown(Keys.Left))
            {
                _position += -direction;
                action = Action.Running;
                _flipped = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) ||
                currentKeyboardState.IsKeyDown(Keys.Right))
            {
                _position += direction;
                action = Action.Running;
                _flipped = false;
            }
            if (!(currentKeyboardState.IsKeyDown(Keys.A) ||
                currentKeyboardState.IsKeyDown(Keys.Left)) &&
                !(currentKeyboardState.IsKeyDown(Keys.D) ||
                currentKeyboardState.IsKeyDown(Keys.Right))
                )
            {
                action = Action.Idle;
            }

            //Jump Function. May work on Later
            //if(currentKeyboardState.IsKeyDown(Keys.Space))
            //{
            //    _position += new Vector2(0, -250 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            //    _jumpTimer += gameTime.ElapsedGameTime.TotalSeconds;
            //    if (_jumpTimer > 1)
            //    {
                     
            //        if (_position.Y > 300)
            //        {
            //            _position += gravity;
            //        }
                    
            //        _jumpTimer = 0;

                    
            //    }
            //}
            if (_position.X < 0) _position.X = 0;
            if (_position.X > 1150) _position.X = 1150;

            _bounds.X = _position.X;
            _bounds.Y = _position.Y;
        }

        /// <summary>
        /// Draws the main character
        /// </summary>
        /// <param name="gameTime">The real time elapsed in the game</param>
        /// <param name="spriteBatch">SpriteBatch</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            SpriteEffects spriteEffects = (_flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            //Update animationFrame
            if (_animationTimer > 0.2)
            {
                _animationFrame++;
                if (_animationFrame > 3) _animationFrame = 0;
                _animationTimer -= 0.2;
            }
            var source = new Rectangle(_animationFrame * 250, (int)action * 512, 268, 512);
            spriteBatch.Draw(_texture, _position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);


        }
    }
}
