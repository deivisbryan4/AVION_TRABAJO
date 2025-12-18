using System.Collections.Generic;
using System.Drawing;

namespace JuegoDeAvion
{
    /// <summary>
    /// Define las propiedades y estadísticas de un tipo de avión.
    /// </summary>
    public class TipoAvion
    {
        public string Nombre { get; set; }
        public int TipoPoligono { get; set; }
        public int Velocidad { get; set; } // Velocidad base
        public int VidaMaxima { get; set; } // Vida base
        public Color ColorPrincipal { get; set; }
        public bool Desbloqueado { get; set; }
        public int Precio { get; set; }
        public int NivelEscala { get; set; } // 0 a 3 (4 niveles de evolución)

        public TipoAvion(string nombre, int tipo, int vel, int vida, Color color, bool desbloqueado, int precio)
        {
            Nombre = nombre;
            TipoPoligono = tipo;
            Velocidad = vel;
            VidaMaxima = vida;
            ColorPrincipal = color;
            Desbloqueado = desbloqueado;
            Precio = precio;
            NivelEscala = 0; // Todos empiezan en escala 0
        }
    }

    /// <summary>
    /// Almacena datos persistentes del juego, como los puntos del jugador y los aviones disponibles.
    /// Es una clase estática para que sus datos sean accesibles desde cualquier parte del código.
    /// </summary>
    public static class DatosGlobales
    {
        /// <summary>
        /// Puntos acumulados por el jugador para comprar y mejorar naves.
        /// </summary>
        public static int PuntosTotales = 10000; 
        
        /// <summary>
        /// El índice del avión que el jugador ha seleccionado para usar en la partida.
        /// </summary>
        public static int IndiceAvionSeleccionado = 0;

        /// <summary>
        /// Lista de todos los tipos de aviones disponibles en el juego, con sus estadísticas base.
        /// </summary>
        public static List<TipoAvion> Aviones = new List<TipoAvion>()
        {
            new TipoAvion("Caza Estándar", 1, 8, 1, Color.Cyan, true, 0),
            new TipoAvion("Interceptor Veloz", 2, 12, 1, Color.Yellow, false, 1000),
            new TipoAvion("Bombardero Pesado", 3, 5, 3, Color.Red, false, 2500)
        };
    }
}