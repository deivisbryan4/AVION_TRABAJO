using System;
using System.Collections.Generic;
using System.Drawing;

namespace JuegoDeAvion
{
    public class Escenario
    {
        private struct Estrella
        {
            public float X, Y;
            public float Velocidad;
            public float Tamano;
            public Color Color;
        }

        private List<Estrella> estrellas;
        private Random rnd;
        private int anchoPantalla;
        private int altoPantalla;
        private int nivelActual = 1;

        public Escenario(int ancho, int alto, int cantidadEstrellas)
        {
            this.anchoPantalla = ancho;
            this.altoPantalla = alto;
            this.rnd = new Random();
            estrellas = new List<Estrella>();

            for (int i = 0; i < cantidadEstrellas; i++)
            {
                GenerarEstrella(true);
            }
        }

        public void CambiarNivel(int nuevoNivel)
        {
            nivelActual = nuevoNivel;
            estrellas.Clear();
            for (int i = 0; i < 200; i++) GenerarEstrella(true);
        }

        private void GenerarEstrella(bool inicioAleatorio)
        {
            Estrella e = new Estrella();
            e.X = rnd.Next(0, anchoPantalla);
            e.Y = inicioAleatorio ? rnd.Next(0, altoPantalla) : -5;
            e.Tamano = (float)(rnd.NextDouble() * 2.5 + 1);
            
            float velocidadBase = (float)(rnd.NextDouble() * 2 + 1);
            
            switch (nivelActual)
            {
                case 1:
                    e.Color = Color.FromArgb(rnd.Next(150, 255), 255, 255);
                    e.Velocidad = velocidadBase;
                    break;
                case 2:
                    e.Color = Color.FromArgb(rnd.Next(150, 255), rnd.Next(50, 100), rnd.Next(50, 100));
                    e.Velocidad = velocidadBase * 1.5f;
                    break;
                case 3:
                    e.Color = Color.FromArgb(rnd.Next(100, 200), rnd.Next(100, 200), rnd.Next(100, 200));
                    e.Velocidad = velocidadBase * 1.2f;
                    break;
                case 4:
                    e.Color = Color.FromArgb(rnd.Next(50, 100), rnd.Next(150, 255), rnd.Next(50, 100));
                    e.Velocidad = velocidadBase * 2.0f;
                    break;
                case 5:
                    e.Color = Color.FromArgb(rnd.Next(100, 255), rnd.Next(100, 255), rnd.Next(100, 255));
                    e.Velocidad = velocidadBase * 3.0f;
                    break;
                default:
                    e.Color = Color.White;
                    e.Velocidad = velocidadBase;
                    break;
            }
            estrellas.Add(e);
        }

        public void Mover()
        {
            for (int i = estrellas.Count - 1; i >= 0; i--)
            {
                Estrella e = estrellas[i];
                e.Y += e.Velocidad;
                if (e.Y > altoPantalla)
                {
                    estrellas.RemoveAt(i);
                    GenerarEstrella(false);
                }
                else
                {
                    estrellas[i] = e;
                }
            }
        }

        public void Dibujar(Graphics g)
        {
            g.Clear(Color.Black);
            foreach (var e in estrellas)
            {
                using (SolidBrush b = new SolidBrush(e.Color))
                {
                    g.FillEllipse(b, e.X, e.Y, e.Tamano, e.Tamano);
                }
            }
        }
        
        // MÉTODO CLAVE PARA LA RESPONSIVIDAD
        public void Redimensionar(int nuevoAncho, int nuevoAlto)
        {
            this.anchoPantalla = nuevoAncho;
            this.altoPantalla = nuevoAlto;
            // Opcional: Podrías ajustar la posición de las estrellas existentes
            // para que no se queden todas en una esquina si agrandas mucho la ventana.
        }
    }
}