using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.App;
using Project.Sprites.Items;
using Project.Sprites.Players;

namespace Project.Sprites
{
    public class Hand : ISprite
    {
        private const int HandSize = 32;
        private const int PlayerRadius = 32;

        private readonly Game1 _game;
        private readonly Player _player;
        private readonly SpriteBatch _spriteBatch;

        private readonly Texture2D _handTexture;
        private bool? _oldIsOpen;
        private double _angleFromPlayer;
        private IItem _heldItem;
        

        public Hand(Player player, Game1 game, SpriteBatch spriteBatch, HandEnums hand)
        {
            _handTexture = game.Content.Load<Texture2D>("Hands");
            HandType = hand;
            _spriteBatch = spriteBatch;
            _player = player;
            _game = game;
            ItemBoundingRectangle = new RotatedRectangle(new Rectangle(0, 0, 0, 0), new Vector2(0, 0), 0);
            Flipped = false;

            UpdateSourceRectangle();
        }

        public HandEnums HandType { get; set; }

        public Vector2 CenterPosition { get; set; }

        public IItem HeldItem
        {
            get => _heldItem;
            set
            {
                // UnEquip off hand components
                if (_heldItem is BowWeapon or MagicWeapon)
                {
                    _player.RightHand.HeldItem = null;
                }

                //Set specific off hand components here
                if (value is BowWeapon && (HandType == HandEnums.LeftPlayer || HandType == HandEnums.LeftEnemy))
                {
                    _player.RightHand.HeldItem = value.ItemType is ItemTypeEnums.WoodenBow ? new Item(_spriteBatch, _game, ItemTypeEnums.WoodenArrow) : new Item(_spriteBatch, _game, ItemTypeEnums.BoneArrow);
                }

                if (value is MagicWeapon staff && (HandType == HandEnums.LeftPlayer || HandType == HandEnums.LeftEnemy))
                {
                    switch (staff.ItemType)
                    {
                        case ItemTypeEnums.WoodenScepter:
                        case ItemTypeEnums.WoodenWand:
                            _player.RightHand.HeldItem = new Item(_spriteBatch, _game, ItemTypeEnums.WoodenBook);
                            break;
                        case ItemTypeEnums.BoneScepter:
                        case ItemTypeEnums.BoneWand:
                            _player.RightHand.HeldItem = new Item(_spriteBatch, _game, ItemTypeEnums.BoneBook);
                            break;
                    }
                }

                _heldItem = value;

                if (value != null)
                {
                    _heldItem.SpriteFlip = Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                }
                
            }
        }

        public bool IsOpen  => HeldItem == null;

        public Rectangle SourceRectangle { get; set; }

        public RotatedRectangle ItemBoundingRectangle { get; set; }

        public bool Flipped { get; set; }

        public void Draw()
        {
            if (!_player.IsAlive) //If player is dead, don't draw hands.
            {
                return;
            }

            var rotation = Flipped ? _angleFromPlayer + Math.PI : _angleFromPlayer;

            //Draw item and then hand over item
            if (HandType == HandEnums.RightPlayer)
            {
                rotation = 0;
            }

            if (!IsOpen)
            {
                if (HeldItem.ItemType is ItemTypeEnums.WoodenArrow or ItemTypeEnums.BoneArrow )
                {
                    HeldItem.SpriteAngle = Flipped ? (float)Math.PI / 2 : 3 * (float)Math.PI / 2;
                }
                else
                {
                    HeldItem.SpriteAngle = (float)rotation;
                }

                ItemBoundingRectangle.AngleInRadians = rotation + HeldItem.SpriteAnimationRotation;
                HeldItem.Draw();
            }

            _spriteBatch.Begin();
            _spriteBatch.Draw(_handTexture, new Rectangle((int)CenterPosition.X, (int)CenterPosition.Y, HandSize, HandSize), SourceRectangle, Color.White, (float)rotation, new Vector2(HandSize / 2f, HandSize / 2f), Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            UpdateSourceRectangle();
            UpdateHandAndItemPosition();
            UpdateHandAndItemAnimationPosition();

            if (!IsOpen)
            { 
                HeldItem.Update(gameTime);
                ItemBoundingRectangle.RotationalPoint = HeldItem.SpriteRotationPivot;

                //Need to move the rectangle up so that it matches where the item is drawn
                ItemBoundingRectangle.Rectangle =
                    new Rectangle((int)HeldItem.Position.X - (int)HeldItem.SpriteRotationPivot.X,
                        (int)HeldItem.Position.Y - (int)HeldItem.SpriteRotationPivot.Y,
                        HeldItem.Texture.Width,
                        HeldItem.Texture.Height);
            }
        }

        private void UpdateSourceRectangle()
        {

            if (_oldIsOpen != null && _oldIsOpen == IsOpen)
            {
                return;
            }

            switch (HandType)
            {
                case HandEnums.LeftPlayer:
                    SourceRectangle = IsOpen ?
                        new Rectangle(HandSize, 0, HandSize, HandSize) :
                        new Rectangle(HandSize, HandSize, HandSize, HandSize);
                    break;
                case HandEnums.RightPlayer:
                    SourceRectangle = IsOpen ?
                        new Rectangle(0, 0, HandSize, HandSize) :
                        new Rectangle(0, HandSize, HandSize, HandSize);
                    break;
                case HandEnums.LeftEnemy:
                    SourceRectangle = IsOpen ?
                        new Rectangle(HandSize, HandSize * 2, HandSize, HandSize) :
                        new Rectangle(HandSize, HandSize * 3, HandSize, HandSize);
                    break;
                case HandEnums.RightEnemy:
                    SourceRectangle = IsOpen ?
                        new Rectangle(0, HandSize * 2, HandSize, HandSize) :
                        new Rectangle(0, HandSize * 3, HandSize, HandSize);
                    break;
            }

            _oldIsOpen = IsOpen;
        }

        private void UpdateHandAndItemPosition()
        {
            if (HandType == HandEnums.RightPlayer)
            {
                var itemDistanceMagnitude = Flipped? -1 : 1;
                itemDistanceMagnitude *= IsOpen ? 1 : 3;

                CenterPosition = _player.Position - new Vector2(itemDistanceMagnitude * (PlayerRadius / 3f), -10);

                if (HeldItem == null)
                {
                    return;
                }

                HeldItem.SpriteFlip = Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                HeldItem.Position = CenterPosition;

                return;
            }

            var centerOfPlayer = new Vector2(
                _player.Position.X,
                _player.Position.Y);

            // TODO: Mouse controller and stop moving character when mouse is off screen
            var mousePosition = Mouse.GetState().Position;

            if (_game.GameState != GameStateEnums.Transition)
            {
                _angleFromPlayer = Math.Atan2(mousePosition.Y - centerOfPlayer.Y, mousePosition.X - centerOfPlayer.X);
            }

            if (_game.GameState != GameStateEnums.Running && _game.GameState != GameStateEnums.Transition)
            {
                _angleFromPlayer = 0;
            }

            CenterPosition = _player.Position
                             + new Vector2((float)(PlayerRadius * Math.Cos(_angleFromPlayer)), (float)(PlayerRadius * Math.Sin(_angleFromPlayer)));

            if (HeldItem != null)
            {
                HeldItem.SpriteFlip = Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                HeldItem.Position = CenterPosition;
            }

            UpdatePositionBasedOnHeldItem();
        }

        private void UpdateHandAndItemAnimationPosition()
        {
            var percent = _player.GetPercentThroughAnimationFrame(_player.CurrentFrame);

            if (percent > .5)
            {
                percent = 1 - percent;
            }

            const int distanceToMoveIdle = 3;
            const int distanceToMoveMoving = 10;

            switch (_player.State)
            {
                case PlayerStateEnums.Death:
                    break;
                case PlayerStateEnums.Idle:
                {
                    CenterPosition += new Vector2(0, distanceToMoveIdle * percent);

                    if (HeldItem != null)
                    {
                        HeldItem.Position += new Vector2(0, distanceToMoveIdle * percent);
                    }

                    break;
                }
                case PlayerStateEnums.Moving:
                    var flip = Flipped ? -1 : 1;
                    var add = HandType is HandEnums.RightPlayer or HandEnums.RightEnemy ? 1 : -1;

                    CenterPosition += add * new Vector2(flip * distanceToMoveMoving * percent, 0);
                    if (HeldItem != null)
                    {
                        HeldItem.Position += add * new Vector2(flip * distanceToMoveMoving * percent, 0);
                    }

                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private void UpdatePositionBasedOnHeldItem()
        {
            switch (HeldItem)
            {
                case null:
                    return;

                case BowWeapon when Flipped:
                    HeldItem.Position = CenterPosition - new Vector2((float)(24 * Math.Cos(_angleFromPlayer)), (float)(24 * Math.Sin(_angleFromPlayer)));
                    break;

                case Key key:
                    key.SpriteRotationPivot = Flipped ? 
                        new Vector2(key.TextureSourceRectangle.Width, key.TextureSourceRectangle.Height / 2f) :
                        new Vector2(0, key.TextureSourceRectangle.Height / 2f);

                    HeldItem.Position = CenterPosition;
                    break;
                
                case MagicWeapon { ItemType: ItemTypeEnums.BoneWand or ItemTypeEnums.WoodenWand } staff:
                    staff.SpriteRotationPivot = Flipped ?
                        new Vector2(3f * staff.TextureSourceRectangle.Width / 4, staff.TextureSourceRectangle.Height / 2f) :
                        new Vector2(staff.TextureSourceRectangle.Width / 4f, staff.TextureSourceRectangle.Height / 2f);
                    break;

                case MeleeWeapon { ItemType: ItemTypeEnums.WoodenScythe or ItemTypeEnums.BoneScythe } scythe:
                    scythe.SpriteRotationPivot = Flipped ?
                        new Vector2(3 * scythe.TextureSourceRectangle.Width / 4f, scythe.TextureSourceRectangle.Height - 18) :
                        new Vector2(scythe.TextureSourceRectangle.Width / 4f, scythe.TextureSourceRectangle.Height - 18);
                    break;

                case Potion potion:

                    potion!.SpriteRotationPivot = Flipped
                        ? new Vector2(potion.TextureSourceRectangle.Width, potion.TextureSourceRectangle.Height / 2f)
                        : new Vector2(0, potion.TextureSourceRectangle.Height / 2f);

                    HeldItem.Position = CenterPosition - new Vector2((float)(8 * Math.Cos(_angleFromPlayer)), (float)(8 * Math.Sin(_angleFromPlayer)));
                    break;
            }
        }
    }
}
