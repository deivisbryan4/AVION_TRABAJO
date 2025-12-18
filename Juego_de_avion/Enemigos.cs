using System;
using System.Drawing;
using System.Linq;

namespace JuegoDeAvion
{
    /// <summary>
    /// Representa una nave enemiga que se mueve y dispara.
    /// Su diseño y estadísticas dependen del tipo de enemigo.
    /// </summary>
    public class Enemigo
    {
        public Rectangle Bounds { get; private set; }
        public int VidaActual { get; private set; }
        public int VidaMaxima { get; private set; }
        public int TipoEnemigo { get; private set; }

        private int velocidad;
        private int cooldownDisparo = 0;
        private Point[] puntosForma;
        private Color color;

        /// <summary>
        /// Crea una nueva nave enemiga de un tipo específico.
        /// </summary>
        /// <param name="tipo">El tipo de enemigo (1, 2, 3...).</param>
        public Enemigo(int tipo, int x, int y, int ancho, int alto, int vel)
        {
            this.TipoEnemigo = tipo;
            this.Bounds = new Rectangle(x, y, ancho, alto);
            this.velocidad = vel;
            
            // Cargar estadísticas y diseño desde la clase de diseños
            this.VidaMaxima = DisenosEnemigos.ObtenerVida(tipo);
            this.VidaActual = this.VidaMaxima;
            this.color = DisenosEnemigos.ObtenerColor(tipo);
            
            // Escalar el polígono al tamaño del enemigo
            Point[] rawPoints = DisenosEnemigos.ObtenerPuntos(tipo);
            this.puntosForma = EscalarPoligono(rawPoints, ancho, alto);
        }

        /// <summary>
        /// Escala los puntos de un polígono para que se ajusten a un ancho y alto específicos.
        /// </summary>
        private Point[] EscalarPoligono(Point[] raw, int w, int h)
        {
            if (raw == null || raw.Length == 0) return new Point[0];
            int minX = raw.Min(p => p.X); int maxX = raw.Max(p => p.X);
            int minY = raw.Min(p => p.Y); int maxY = raw.Max(p => p.Y);
            float anchoOriginal = Math.Max(1, maxX - minX); float altoOriginal = Math.Max(1, maxY - minY);
            
            Point[] puntosEscalados = new Point[raw.Length];
            float ratioX = w / anchoOriginal; float ratioY = h / altoOriginal;

            for (int i = 0; i < raw.Length; i++)
            {
                puntosEscalados[i] = new Point((int)((raw[i].X - minX) * ratioX), (int)((raw[i].Y - minY) * ratioY));
            }
            return puntosEscalados;
        }

        public void RecibirDano(int cantidad)
        {
            VidaActual -= cantidad;
            if (VidaActual < 0) VidaActual = 0;
        }

        public void Mover(int anchoPantallaActual)
        {
            Rectangle r = Bounds;
            r.Y += velocidad;
            Bounds = r;
            if (cooldownDisparo > 0) cooldownDisparo--;
        }

        public bool QuiereDisparar(Random rnd)
        {
            if (cooldownDisparo == 0 && Bounds.Y > 50)
            {
                if (rnd.Next(100) < 2)
                {
                    cooldownDisparo = 100;
                    return true;
                }
            }
            return false;
        }

        public void Dibujar(Graphics g)
        {
            if (VidaActual < VidaMaxima)
            {
                float porcentajeVida = (float)VidaActual / VidaMaxima;
                g.FillRectangle(Brushes.Red, Bounds.X, Bounds.Y - 10, Bounds.Width, 5);
                g.FillRectangle(Brushes.Green, Bounds.X, Bounds.Y - 10, Bounds.Width * porcentajeVida, 5);
            }

            // Mover los puntos del polígono a la posición actual del enemigo
            Point[] puntosDibujo = new Point[puntosForma.Length];
            for (int i = 0; i < puntosForma.Length; i++)
            {
                puntosDibujo[i] = new Point(puntosForma[i].X + Bounds.X, puntosForma[i].Y + Bounds.Y);
            }

            using (SolidBrush brush = new SolidBrush(this.color))
            {
                g.FillPolygon(brush, puntosDibujo);
            }
            using (Pen p = new Pen(Color.White, 1)) // Borde blanco fino
            {
                g.DrawPolygon(p, puntosDibujo);
            }
        }

        public bool FueraDePantalla(int altoPantalla)
        {
            return Bounds.Y > altoPantalla;
        }
    }
}