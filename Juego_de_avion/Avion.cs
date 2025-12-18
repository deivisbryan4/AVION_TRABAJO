using System.Drawing;

namespace JuegoDeAvion
{
    /// <summary>
    /// Clase estática que actúa como una base de datos de diseños de naves.
    /// Contiene los puntos (polígonos) que definen la forma visual de cada tipo de avión.
    /// </summary>
    public static class Avion
    {
        /// <summary>
        /// Obtiene los puntos que forman el polígono de un tipo de nave específico.
        /// </summary>
        /// <param name="tipo">El identificador del tipo de nave (1, 2, 3...).</param>
        /// <returns>Un array de puntos (Point[]) que representa la forma de la nave.</returns>
        public static Point[] GetShipPoints(int tipo)
        {
            switch (tipo)
            {
                case 1: // Tipo 1: Nave de Combate (Caza Estándar)
                    return new Point[] {
                        new(29,0), new(30,1), new(30,6), new(31,6), new(31,11), new(32,11),
                        new(32,17), new(35,17), new(35,16), new(37,16), new(37,17), new(38,18),
                        new(38,28), new(39,28), new(42,39), new(44,45), new(50,51), new(51,51),
                        new(51,52), new(58,59), new(58,66), new(44,66), new(39,71), new(35,71),
                        new(35,74), new(32,77), new(26,77), new(23,74), new(23,71), new(19,71),
                        new(14,66), new(0,66), new(0,59), new(7,52), new(7,51), new(8,51),
                        new(14,45), new(16,39), new(19,28), new(20,28), new(20,18), new(21,17),
                        new(21,16), new(23,16), new(23,17), new(26,17), new(26,11), new(27,11),
                        new(27,6), new(28,6), new(28,1), new(29,0)
                    };
                case 2: // Tipo 2: Nave de Carga / Interceptor Veloz
                    return new Point[] {
                        new(24,0), new(29,5), new(29,18), new(32,21), new(34,21), new(38,17),
                        new(41,20), new(41,30), new(47,36), new(47,41), new(41,41), new(38,44),
                        new(36,44), new(33,41), new(30,41), new(25,46), new(22,46), new(17,41),
                        new(14,41), new(11,44), new(9,44), new(6,41), new(0,41), new(0,36),
                        new(6,30), new(6,20), new(9,17), new(13,21), new(15,21), new(18,18),
                        new(18,5), new(23,0)
                    };
                case 3: // Tipo 3: Bombardero Pesado
                    return new Point[] {
                        new(30, 0), new(60, 25), new(53, 20),
                        new(45, 28), new(38, 23), new(30, 30),
                        new(22, 23), new(15, 28), new(7, 20),
                        new(0, 25), new(30, 0)
                    };
                case 4: // Tipo 4: Nave Galaga (diseño fiel al original)
                    return new Point[] {
                        // Cuerpo superior
                        new(30, 0), new(33, 0), new(33, 3), new(36, 3), new(36, 6),
                        new(39, 6), new(39, 9), new(42, 9), new(42, 20), new(39, 20),
                        new(39, 23), new(36, 23), new(36, 26), new(33, 26), new(33, 29),
                        new(30, 29), new(30, 26), new(27, 26), new(27, 23), new(24, 23),
                        new(24, 20), new(21, 20), new(21, 9), new(24, 9), new(24, 6),
                        new(27, 6), new(27, 3), new(30, 3),
                        // Cuerpo inferior
                        new(21, 23), new(21, 26), new(18, 26), new(18, 29), new(15, 29),
                        new(15, 32), new(12, 32), new(12, 35), new(9, 35), new(9, 38),
                        new(6, 38), new(6, 41), new(3, 41), new(3, 44), new(0, 44),
                        new(0, 47), new(3, 47), new(3, 50), new(6, 50), new(6, 53),
                        new(9, 53), new(9, 56), new(12, 56), new(12, 59), new(15, 59),
                        new(15, 62), new(18, 62), new(18, 65), new(21, 65), new(21, 68),
                        new(39, 68), new(39, 65), new(42, 65), new(42, 62), new(45, 62),
                        new(45, 59), new(48, 59), new(48, 56), new(51, 56), new(51, 53),
                        new(54, 53), new(54, 50), new(57, 50), new(57, 47), new(60, 47),
                        new(60, 44), new(57, 44), new(57, 41), new(54, 41), new(54, 38),
                        new(51, 38), new(51, 35), new(48, 35), new(48, 32), new(45, 32),
                        new(45, 29), new(42, 29), new(42, 26), new(39, 26), new(39, 23),
                        // Detalles interiores
                        new(27, 26), new(27, 29), new(33, 29), new(33, 26)
                    };
                default: // Fallback: Un rombo simple si el tipo no existe
                    return new Point[] { new(0, 0), new(10, 0), new(10, 10), new(0, 10) };
            }
        }
    }
}