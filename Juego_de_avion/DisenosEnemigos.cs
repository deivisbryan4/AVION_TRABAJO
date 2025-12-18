using System.Drawing;

namespace JuegoDeAvion
{
    /// <summary>
    /// Clase estática que actúa como base de datos de diseños para los enemigos.
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
                case 1: // Tipo 1: Caza Ligero (Triángulo invertido clásico)
                    return new Point[] {
                        new Point(25, 50), // Punta abajo
                        new Point(0, 0),   // Esq sup izq
                        new Point(50, 0)   // Esq sup der
                    };

                case 2: // Tipo 2: Platillo Volador (OVNI)
                    return new Point[] {
                        new Point(15, 15), // Inicio cúpula izq
                        new Point(25, 0),  // Tope cúpula
                        new Point(35, 15), // Fin cúpula der
                        new Point(50, 25), // Borde disco der
                        new Point(35, 40), // Base disco der
                        new Point(15, 40), // Base disco izq
                        new Point(0, 25),  // Borde disco izq
                        new Point(15, 15)  // Cierre
                    };

                case 3: // Tipo 3: Rectángulo (Diseño personalizado)
                    return new Point[] {
                        new Point(0, 0), 
                        new Point(0, 33), 
                        new Point(59, 33), 
                        new Point(59, 0)
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
                case 1: return Color.MediumPurple;
                case 2: return Color.LimeGreen;
                case 3: return Color.DarkOrange; // Color para el rectángulo
                default: return Color.Gray;
            }
        }

        /// <summary>Obtiene la vida máxima base para un tipo de enemigo.</summary>
        public static int ObtenerVida(int tipo)
        {
            switch (tipo)
            {
                case 1: return 100;
                case 2: return 200;
                case 3: return 120;
                default: return 100;
            }
        }
    }
}