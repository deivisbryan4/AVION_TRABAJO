using System.Drawing;

namespace JuegoDeAvion
{
    public class Bala
    {
        public Rectangle Bounds { get; private set; }
        public bool Activa { get; private set; }
        private int velocidad = 15;

        public Bala(int startX, int startY)
        {
            Bounds = new Rectangle(startX - 2, startY, 4, 10);
            Activa = true;
        }

        public void Mover()
        {
            // Mover hacia arriba
            Rectangle r = Bounds;
            r.Y -= velocidad;
            Bounds = r;

            // Desactivar si sale de la pantalla
            if (Bounds.Y + Bounds.Height < 0)
            {
                Activa = false;
            }
        }

        public void Dibujar(Graphics g)
        {
            if (Activa)
            {
                using (SolidBrush b = new SolidBrush(Color.Yellow))
                {
                    g.FillRectangle(b, Bounds);
                }
            }
        }
    }
}