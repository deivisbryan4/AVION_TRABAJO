using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace JuegoDeAvion
{
    public class TipoAvion
    {
        public string Nombre { get; set; }
        public int TipoPoligono { get; set; } // 0 si usa imagen
        public string? RutaImagen { get; set; } // Ruta a la imagen si es personalizado
        public int Velocidad { get; set; }
        public int VidaMaxima { get; set; }
        public Color ColorPrincipal { get; set; }
        public bool Desbloqueado { get; set; }
        public int Precio { get; set; }
        public int NivelEscala { get; set; }

        // Constructor para aviones predefinidos (polígonos)
        public TipoAvion(string nombre, int tipo, int vel, int vida, Color color, bool desbloqueado, int precio)
        {
            Nombre = nombre;
            TipoPoligono = tipo;
            RutaImagen = null;
            Velocidad = vel;
            VidaMaxima = vida;
            ColorPrincipal = color;
            Desbloqueado = desbloqueado;
            Precio = precio;
            NivelEscala = 0;
        }

        // Constructor para aviones personalizados (imágenes)
        public TipoAvion(string nombre, string rutaImagen, int vel, int vida, Color color, bool desbloqueado, int precio)
        {
            Nombre = nombre;
            TipoPoligono = 0; // 0 indica que se debe usar la imagen
            RutaImagen = rutaImagen;
            Velocidad = vel;
            VidaMaxima = vida;
            ColorPrincipal = color;
            Desbloqueado = desbloqueado;
            Precio = precio;
            NivelEscala = 0;
        }
    }

    public static class DatosGlobales
    {
        public static int PuntosTotales = 10000;
        public static int IndiceAvionSeleccionado = 0;
        public static List<TipoAvion> Aviones = new List<TipoAvion>();
        private static bool avionesCargados = false;

        /// <summary>
        /// Carga todos los aviones, tanto los predefinidos como los personalizados.
        /// Se asegura de que solo se ejecute una vez.
        /// </summary>
        public static void CargarAviones()
        {
            if (avionesCargados) return;

            // 1. Añadir aviones predefinidos
            Aviones.Add(new TipoAvion("Caza Estándar", 1, 8, 1, Color.Cyan, true, 0));
            Aviones.Add(new TipoAvion("Interceptor Veloz", 2, 12, 1, Color.Yellow, false, 1000));
            Aviones.Add(new TipoAvion("Bombardero Pesado", 3, 5, 3, Color.Red, false, 2500));

            // 2. Cargar aviones personalizados desde la carpeta
            CargarAvionesPersonalizados();

            avionesCargados = true;
        }

        /// <summary>
        /// Busca imágenes en la carpeta "CrearAvion" y las añade como nuevos tipos de avión.
        /// </summary>
        private static void CargarAvionesPersonalizados()
        {
            try
            {
                string carpeta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrearAvion");
                if (!Directory.Exists(carpeta)) return;

                string[] extensiones = { "*.png", "*.jpg", "*.jpeg" };
                foreach (var ext in extensiones)
                {
                    foreach (var archivo in Directory.GetFiles(carpeta, ext))
                    {
                        string nombre = Path.GetFileNameWithoutExtension(archivo);
                        // Creamos un nuevo tipo de avión con estadísticas base y lo añadimos a la lista
                        Aviones.Add(new TipoAvion(nombre, archivo, 7, 2, Color.White, false, 1500));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar aviones personalizados: " + ex.Message);
            }
        }
    }
}