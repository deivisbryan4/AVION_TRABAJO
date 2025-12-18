using System;
using System.Drawing;

namespace JuegoDeAvion
{
    public class Enemigo
    {
        public Rectangle Bounds { get; private set; }
        public int VidaActual { get; private set; }
        public int VidaMaxima { get; private set; }

        private int velocidad;
        private Color color;
        private bool puedeDisparar;
        private int cooldownDisparo = 0;
        // Eliminamos la variable pantallaAncho interna, se pasará por parámetro

        public Enemigo(int x, int y, int ancho, int alto, int vel, bool dispara, int anchoDePantalla)
        {
            this.Bounds = new Rectangle(x, y, ancho, alto);
            this.velocidad = vel;
            this.puedeDisparar = dispara;
            this.VidaMaxima = dispara ? 150 : 50;
            this.VidaActual = this.VidaMaxima;
            
            Random rnd = new Random();
            this.color = dispara ? Color.MediumPurple : Color.Gray;
        }

        public void RecibirDano(int cantidad)
        {
            VidaActual -= cantidad;
            if (VidaActual < 0) VidaActual = 0;
        }

        // AHORA RECIBE EL ANCHO ACTUAL DE LA PANTALLA
        public void Mover(int anchoPantallaActual)
        {
            Rectangle r = Bounds;
            r.Y += velocidad;
            
            // Movimiento lateral
            if (r.Y % 60 < 30) r.X += 1;
            else r.X -= 1;

            // Límites dinámicos
            if (r.X < 0) r.X = 0;
            if (r.X + r.Width > anchoPantallaActual) r.X = anchoPantallaActual - r.Width;

            Bounds = r;
            if (cooldownDisparo > 0) cooldownDisparo--;
        }

        public bool QuiereDisparar(Random rnd)
        {
            if (puedeDisparar && cooldownDisparo == 0 && Bounds.Y > 50)
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

            using (SolidBrush brush = new SolidBrush(color))
            {
                g.FillEllipse(brush, Bounds);
            }
            if (puedeDisparar)
            {
                using (Pen p = new Pen(Color.Red, 3))
                {
                    g.DrawEllipse(p, Bounds);
                }
            }
        }

        public bool FueraDePantalla(int altoPantalla)
        {
            return Bounds.Y > altoPantalla;
        }
    }
}