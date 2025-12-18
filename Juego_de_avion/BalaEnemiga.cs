using System.Drawing;

namespace JuegoDeAvion
{
    /// <summary>
    /// Representa un proyectil disparado por un enemigo.
    /// </summary>
    public class BalaEnemiga
    {
        /// <summary>Área de colisión de la bala.</summary>
        public Rectangle Bounds { get; private set; }
        
        /// <summary>Indica si la bala sigue en juego.</summary>
        public bool Activa { get; private set; }
        
        private int velocidad = 10;

        /// <summary>Crea una nueva bala enemiga en la posición especificada.</summary>
        public BalaEnemiga(int startX, int startY)
        {
            Bounds = new Rectangle(startX - 3, startY, 6, 12);
            Activa = true;
        }

        /// <summary>Mueve la bala hacia abajo y la desactiva si sale de la pantalla.</summary>
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

        /// <summary>Dibuja la bala en pantalla si está activa.</summary>
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