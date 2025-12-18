using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace JuegoDeAvion
{
    /// <summary>
    /// Gestiona el fondo del juego, combinando una imagen estática con estrellas en movimiento.
    /// </summary>
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
        
        private Image? imagenFondo;
        private int anchoPantalla;
        private int altoPantalla;
        private int nivelActual = 1;

        private Color[] coloresFondo = new Color[] { 
            Color.Black, Color.FromArgb(20, 0, 0), Color.FromArgb(0, 0, 20),
            Color.FromArgb(0, 20, 0), Color.FromArgb(20, 0, 20)
        };

        /// <summary>
        /// Crea una nueva instancia del escenario.
        /// </summary>
        /// <param name="ancho">Ancho inicial de la pantalla.</param>
        /// <param name="alto">Alto inicial de la pantalla.</param>
        /// <param name="cantidadEstrellas">Número de estrellas a generar.</param>
        public Escenario(int ancho, int alto, int cantidadEstrellas)
        {
            this.anchoPantalla = ancho;
            this.altoPantalla = alto;
            this.rnd = new Random();
            this.estrellas = new List<Estrella>();

            for (int i = 0; i < cantidadEstrellas; i++)
            {
                GenerarEstrella(true);
            }
            CargarFondo();
        }

        /// <summary>
        /// Cambia el nivel actual, cargando un nuevo fondo y regenerando las estrellas.
        /// </summary>
        public void CambiarNivel(int nuevoNivel)
        {
            nivelActual = nuevoNivel;
            CargarFondo();
            estrellas.Clear();
            for (int i = 0; i < 200; i++) GenerarEstrella(true);
        }

        /// <summary>
        /// Carga la imagen de fondo desde la carpeta "EscenariosDeNivel".
        /// Busca .png, .jpg y .jpeg. Si no encuentra, usará un color sólido.
        /// </summary>
        private void CargarFondo()
        {
            if (imagenFondo != null) { imagenFondo.Dispose(); imagenFondo = null; }
            try
            {
                string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EscenariosDeNivel");
                string[] extensiones = { ".png", ".jpg", ".jpeg" };
                string? rutaEncontrada = null;

                foreach (var ext in extensiones)
                {
                    string ruta = Path.Combine(carpeta, $"escenario{nivelActual}{ext}");
                    if (File.Exists(ruta))
                    {
                        rutaEncontrada = ruta;
                        break;
                    }
                }

                if (rutaEncontrada != null) imagenFondo = Image.FromFile(rutaEncontrada);
                else Console.WriteLine($"No se encontró fondo para nivel {nivelActual}");
            }
            catch (Exception ex) { Console.WriteLine($"Error al cargar fondo: {ex.Message}"); imagenFondo = null; }
        }

        /// <summary>Genera una nueva estrella con color y velocidad basados en el nivel actual.</summary>
        private void GenerarEstrella(bool inicioAleatorio)
        {
            Estrella e = new Estrella {
                X = rnd.Next(0, anchoPantalla),
                Y = inicioAleatorio ? rnd.Next(0, altoPantalla) : -5,
                Tamano = (float)(rnd.NextDouble() * 2.5 + 1)
            };
            float velocidadBase = (float)(rnd.NextDouble() * 2 + 1);
            
            switch (nivelActual)
            {
                case 1: e.Color = Color.FromArgb(rnd.Next(200, 255), 255, 255); e.Velocidad = velocidadBase; break;
                case 2: e.Color = Color.FromArgb(255, rnd.Next(100, 200), 100); e.Velocidad = velocidadBase * 1.5f; break;
                case 3: e.Color = Color.FromArgb(200, 200, 255); e.Velocidad = velocidadBase * 1.2f; break;
                case 4: e.Color = Color.FromArgb(100, 255, 100); e.Velocidad = velocidadBase * 2.0f; break;
                case 5: e.Color = Color.FromArgb(rnd.Next(100, 255), rnd.Next(100, 255), rnd.Next(100, 255)); e.Velocidad = velocidadBase * 3.0f; break;
                default: e.Color = Color.White; e.Velocidad = velocidadBase; break;
            }
            estrellas.Add(e);
        }

        /// <summary>Mueve las estrellas en el escenario.</summary>
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

        /// <summary>Dibuja el fondo (imagen o color) y luego las estrellas encima.</summary>
        public void Dibujar(Graphics g)
        {
            if (imagenFondo != null)
            {
                g.DrawImage(imagenFondo, 0, 0, anchoPantalla, altoPantalla);
            }
            else
            {
                int indiceColor = (nivelActual - 1) % coloresFondo.Length;
                g.Clear(coloresFondo[indiceColor]);
            }

            foreach (var e in estrellas)
            {
                using (SolidBrush b = new SolidBrush(e.Color))
                {
                    g.FillEllipse(b, e.X, e.Y, e.Tamano, e.Tamano);
                }
            }
        }
        
        /// <summary>Actualiza las dimensiones del escenario cuando la ventana cambia de tamaño.</summary>
        public void Redimensionar(int w, int h)
        {
            this.anchoPantalla = w;
            this.altoPantalla = h;
        }
    }
}