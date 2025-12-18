using System.Drawing;

namespace JuegoDeAvion
{
    /// <summary>
    /// Clase estática que actúa como base de datos de diseños para los enemigos.
    /// Inspirado en los diseños de naves de Galaxian con una paleta "pixelred".
    /// </summary>
    public static class DisenosEnemigos
    {
        /// <summary>
        /// Obtiene los puntos del polígono para un tipo de enemigo específico.
        /// </summary>
        /// <param name="tipo">El tipo de enemigo (1, 2, 3...).</param>
        public static Point[] ObtenerPuntos(int tipo)
        {
            switch (tipo)
            {
                case 1: // Tipo 1: Dron Galaxian (Rojo)
                    return new Point[] {
                        new Point(25, 0),  // Pico superior
                        new Point(50, 20), // Ala derecha
                        new Point(40, 25),
                        new Point(25, 40), // Cola
                        new Point(10, 25),
                        new Point(0, 20),  // Ala izquierda
                        new Point(25, 0)   // Cierre
                    };

                case 2: // Tipo 2: Comandante Galaxian (Magenta)
                    return new Point[] {
                        new Point(25, 0),  // Pico superior
                        new Point(50, 15), // Ala superior derecha
                        new Point(35, 15),
                        new Point(45, 40), // Ala inferior derecha
                        new Point(25, 30), // Cola
                        new Point(5, 40),  // Ala inferior izquierda
                        new Point(15, 15),
                        new Point(0, 15),  // Ala superior izquierda
                        new Point(25, 0)   // Cierre
                    };

                case 3: // Tipo 3: Platillo Volador (OVNI Especial)
                    return new Point[] {
                        new Point(15, 15), new Point(25, 0), new Point(35, 15), // Cúpula
                        new Point(50, 25), new Point(35, 40), new Point(15, 40), // Disco
                        new Point(0, 25), new Point(15, 15)
                    };

                default: // Fallback
                    return new Point[] { new Point(0, 0), new Point(50, 0), new Point(25, 50) };
            }
        }

        /// <summary>Obtiene el color base para un tipo de enemigo.</summary>
        public static Color ObtenerColor(int tipo)
        {
            switch (tipo)
            {
                case 1: return Color.FromArgb(255, 50, 50);   // Rojo Pixel
                case 2: return Color.FromArgb(255, 80, 255);  // Magenta/Púrpura Pixel
                case 3: return Color.FromArgb(0, 255, 255);   // Cian Pixel
                default: return Color.Gray;
            }
        }

        /// <summary>Obtiene la vida máxima base para un tipo de enemigo.</summary>
        public static int ObtenerVida(int tipo)
        {
            switch (tipo)
            {
                case 1: return 100; // Dron estándar
                case 2: return 150; // Comandante más resistente
                case 3: return 80;  // OVNI más rápido y frágil
                default: return 100;
            }
        }
    }
}