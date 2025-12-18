using System;
using System.Drawing;

namespace JuegoDeAvion
{
    public class EnemigoMejorado
    {
        public Rectangle Bounds { get; private set; }
        public int VidaActual { get; private set; }
        public int VidaMaxima { get; private set; }
        public int TipoEnemigo { get; private set; }

        private int velocidad;
        private int cooldownDisparo = 0;
        private Image? imagenEnemigo = null;

        public EnemigoMejorado(int x, int y, int ancho, int alto, int vel, int tipo)
        {
            this.Bounds = new Rectangle(x, y, ancho, alto);
            this.velocidad = vel;
            this.TipoEnemigo = tipo;
            this.VidaMaxima = DisenosEnemigos.ObtenerVida(tipo);
            this.VidaActual = this.VidaMaxima;
            
            // Los enemigos mejorados no usan imágenes, solo polígonos
            imagenEnemigo = null;
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

            // Los enemigos mejorados siempre usan polígonos, no imágenes
            if (imagenEnemigo != null)
            {
                g.DrawImage(imagenEnemigo, Bounds);
            }
            else
            {
                Point[] puntos = DisenosEnemigos.ObtenerPuntos(TipoEnemigo);
                if (puntos != null && puntos.Length >= 3)
                {
                    int minX = int.MaxValue, minY = int.MaxValue;
                    int maxX = int.MinValue, maxY = int.MinValue;
                    
                    foreach (var p in puntos)
                    {
                        if (p.X < minX) minX = p.X;
                        if (p.X > maxX) maxX = p.X;
                        if (p.Y < minY) minY = p.Y;
                        if (p.Y > maxY) maxY = p.Y;
                    }
                    
                    float anchoOriginal = maxX - minX;
                    float altoOriginal = maxY - minY;
                    float escalaX = Bounds.Width / anchoOriginal;
                    float escalaY = Bounds.Height / altoOriginal;
                    
                    PointF[] puntosEscalados = new PointF[puntos.Length];
                    for (int i = 0; i < puntos.Length; i++)
                    {
                        puntosEscalados[i] = new PointF(
                            Bounds.X + (puntos[i].X - minX) * escalaX,
                            Bounds.Y + (puntos[i].Y - minY) * escalaY
                        );
                    }
                    
                    using (SolidBrush brush = new SolidBrush(DisenosEnemigos.ObtenerColor(TipoEnemigo)))
                    {
                        g.FillPolygon(brush, puntosEscalados);
                    }
                }
            }
        }

        public bool FueraDePantalla(int altoPantalla)
        {
            return Bounds.Y > altoPantalla;
        }
    }
}
