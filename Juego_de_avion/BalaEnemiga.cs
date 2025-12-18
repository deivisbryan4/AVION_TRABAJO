using System.Drawing;

namespace JuegoDeAvion
{
    public class BalaEnemiga
    {
        public Rectangle Bounds { get; private set; }
        public bool Activa { get; private set; }
        private int velocidad = 10;

        public BalaEnemiga(int startX, int startY)
        {
            Bounds = new Rectangle(startX - 3, startY, 6, 12);
            Activa = true;
        }

        public void Mover(int altoPantalla)
        {
            Rectangle r = Bounds;
            r.Y += velocidad;
            Bounds = r;

            if (Bounds.Y > altoPantalla)
            {
                Activa = false;
            }
        }

        public void Dibujar(Graphics g)
        {
            if (Activa)
            {
                using (SolidBrush b = new SolidBrush(Color.Red))
                {
                    g.FillRectangle(b, Bounds);
                }
            }
        }
    }
}