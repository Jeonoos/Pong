using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
namespace Pong
{
    public class Bar
    {
        public int lives = 3;
        public Vector2 position;
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
