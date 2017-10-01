using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong
{
    public class Bar
    {
        public float speed = 1f;
        public int lives = 3;
        Vector2 pos;
        public Vector2 position {
            get { return pos; }
            set {
                if (value.Y > Game1.screenHeight - tex.Height / 2)
                    pos = new Vector2(value.X, Game1.screenHeight - tex.Height / 2);
                else if (value.Y < tex.Height / 2)
                    pos = new Vector2(value.X, tex.Height / 2);
                else
                    pos = value;
            }
        }
        public Vector2 size;
        public Texture2D tex;
        public Vector2 origin;

        public Bar(Vector2 position_, Vector2 size_, Texture2D tex_) {
            tex = tex_;
            position = position_;
            size = size_;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
        }
    }
}
