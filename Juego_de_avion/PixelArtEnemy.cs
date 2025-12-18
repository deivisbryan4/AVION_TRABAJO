using System;
using System.Drawing;

namespace JuegoDeAvion
{
    /// <summary>
    /// Enemigo PixelArtRedBlueCreature que se comporta como una nave enemiga normal
    /// pero con la apariencia del PixelArt generada proceduralmente.
    /// </summary>
    public class PixelArtEnemy
    {
        public Rectangle Bounds { get; private set; }
        public int VidaActual { get; private set; }
        public int VidaMaxima { get; private set; }

        private int velocidad;
        private int cooldownDisparo = 0;
        private Image imagenPixelArt;

        public PixelArtEnemy(int x, int y, int ancho, int alto, int velocidad)
        {
            this.Bounds = new Rectangle(x, y, ancho, alto);
            this.velocidad = velocidad;
            this.VidaMaxima = 100; // Resistencia estándar
            this.VidaActual = this.VidaMaxima;
            
            // Generar la imagen del PixelArtRedBlueCreature
            imagenPixelArt = PixelArtRedBlueCreature.Generate();
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
                if (rnd.Next(100) < 2) // 2% de probabilidad por frame
                {
                    cooldownDisparo = 100; // Tiempo de espera antes de volver a disparar
                    return true;
                }
            }
            return false;
        }

        public void Dibujar(Graphics g)
        {
            // Dibuja la barra de vida si ha recibido daño
            if (VidaActual < VidaMaxima)
            {
                float porcentajeVida = (float)VidaActual / VidaMaxima;
                g.FillRectangle(Brushes.Red, Bounds.X, Bounds.Y - 10, Bounds.Width, 5);
                g.FillRectangle(Brushes.Green, Bounds.X, Bounds.Y - 10, Bounds.Width * porcentajeVida, 5);
            }

            // Dibujar la imagen del PixelArt
            if (imagenPixelArt != null)
            {
                g.DrawImage(imagenPixelArt, Bounds);
            }
        }

        public bool FueraDePantalla(int altoPantalla)
        {
            return Bounds.Y > altoPantalla;
        }
    }
}
