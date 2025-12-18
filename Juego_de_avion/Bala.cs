using System.Drawing;

namespace JuegoDeAvion
{
    /// <summary>
    /// Representa un proyectil disparado por el jugador.
    /// </summary>
    public class Bala
    {
        /// <summary>Área de colisión de la bala.</summary>
        public Rectangle Bounds { get; private set; }
        
        /// <summary>Indica si la bala sigue en juego.</summary>
        public bool Activa { get; private set; }
        
        private int velocidad = 15;

        /// <summary>Crea una nueva bala en la posición especificada.</summary>
        public Bala(int startX, int startY)
        {
            Bounds = new Rectangle(startX - 2, startY, 4, 10);
            Activa = true;
        }

        /// <summary>Mueve la bala hacia arriba y la desactiva si sale de la pantalla.</summary>
        public void Mover()
        {
            Rectangle r = Bounds;
            r.Y -= velocidad;
            Bounds = r;

            if (Bounds.Y + Bounds.Height < 0)
            {
                Activa = false;
            }
        }

        /// <summary>Dibuja la bala en pantalla si está activa.</summary>
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