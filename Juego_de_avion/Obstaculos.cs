using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace JuegoDeAvion
{
    /// <summary>
    /// Representa un asteroide que se mueve y rota. No dispara.
    /// </summary>
    public class Obstaculo
    {
        /// <summary>El área rectangular que ocupa el asteroide.</summary>
        public Rectangle Bounds { get; private set; }
        
        /// <summary>Puntos de vida actuales del asteroide.</summary>
        public int VidaActual { get; private set; }
        
        /// <summary>Máximos puntos de vida del asteroide.</summary>
        public int VidaMaxima { get; private set; }

        private int velocidad;
        private Color colorRelleno;
        private Color colorBorde;
        private Point[] puntosForma;
        private float anguloRotacion = 0;
        private float velocidadRotacion;

        /// <summary>
        /// Crea un nuevo asteroide con una forma poligonal irregular.
        /// </summary>
        public Obstaculo(int x, int y, int ancho, int alto, int vel)
        {
            this.Bounds = new Rectangle(x, y, ancho, alto);
            this.velocidad = vel;
            this.VidaMaxima = 50;
            this.VidaActual = this.VidaMaxima;
            
            this.colorRelleno = Color.FromArgb(60, 40, 20); // Marrón oscuro
            this.colorBorde = Color.FromArgb(30, 20, 10);   // Casi negro
            
            Random rnd = new Random();
            this.puntosForma = GenerarFormaAsteroide(ancho, alto, rnd);
            this.velocidadRotacion = (float)(rnd.NextDouble() * 4 - 2);
        }

        /// <summary>
        /// Genera un polígono irregular aleatorio para simular la forma de una roca.
        /// </summary>
        private Point[] GenerarFormaAsteroide(int w, int h, Random rnd)
        {
            int numPuntos = rnd.Next(7, 12);
            Point[] puntos = new Point[numPuntos];
            double anguloPaso = (Math.PI * 2) / numPuntos;
            int radioX = w / 2;
            int radioY = h / 2;

            for (int i = 0; i < numPuntos; i++)
            {
                double variacion = rnd.NextDouble() * 0.4 + 0.8;
                double angulo = i * anguloPaso;
                
                int px = (int)(radioX + Math.Cos(angulo) * radioX * variacion);
                int py = (int)(radioY + Math.Sin(angulo) * radioY * variacion);
                
                px = Math.Max(0, Math.Min(w, px));
                py = Math.Max(0, Math.Min(h, py));

                puntos[i] = new Point(px, py);
            }
            return puntos;
        }

        /// <summary>Reduce la vida del asteroide.</summary>
        public void RecibirDano(int cantidad)
        {
            VidaActual -= cantidad;
            if (VidaActual < 0) VidaActual = 0;
        }

        /// <summary>Mueve el asteroide y actualiza su rotación.</summary>
        public void Mover(int anchoPantallaActual)
        {
            Rectangle r = Bounds;
            r.Y += velocidad;
            if (r.Y % 100 < 50) r.X += 1; else r.X -= 1;
            if (r.X < 0) r.X = 0;
            if (r.X + r.Width > anchoPantallaActual) r.X = anchoPantallaActual - r.Width;
            Bounds = r;
            anguloRotacion += velocidadRotacion;
        }

        /// <summary>Dibuja el asteroide rotado en pantalla.</summary>
        public void Dibujar(Graphics g)
        {
            if (VidaActual < VidaMaxima)
            {
                float porcentajeVida = (float)VidaActual / VidaMaxima;
                g.FillRectangle(Brushes.Red, Bounds.X, Bounds.Y - 10, Bounds.Width, 5);
                g.FillRectangle(Brushes.Green, Bounds.X, Bounds.Y - 10, Bounds.Width * porcentajeVida, 5);
            }

            GraphicsState estadoOriginal = g.Save();

            // Aplicar rotación
            g.TranslateTransform(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            g.RotateTransform(anguloRotacion);
            g.TranslateTransform(-Bounds.Width / 2, -Bounds.Height / 2);

            // Dibujar polígono
            using (SolidBrush brush = new SolidBrush(colorRelleno))
            {
                g.FillPolygon(brush, puntosForma);
            }
            using (Pen p = new Pen(colorBorde, 3))
            {
                g.DrawPolygon(p, puntosForma);
            }
            using (SolidBrush crater = new SolidBrush(Color.FromArgb(20, 10, 5)))
            {
                g.FillEllipse(crater, Bounds.Width / 3, Bounds.Height / 3, Bounds.Width / 4, Bounds.Height / 4);
            }

            g.Restore(estadoOriginal);
        }

        /// <summary>Comprueba si el asteroide ha salido de la pantalla.</summary>
        public bool FueraDePantalla(int altoPantalla)
        {
            return Bounds.Y > altoPantalla;
        }
    }
}