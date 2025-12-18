using System.Windows.Forms;

namespace JuegoDeAvion
{
    /// <summary>
    /// Clase centralizada para manejar la entrada del teclado.
    /// Traduce las pulsaciones de teclas en acciones de juego (moverse, disparar, etc.).
    /// Soporta tanto WASD como las flechas direccionales.
    /// </summary>
    public class EntradaDeTeclado
    {
        #region Propiedades de Estado
        
        /// <summary>Indica si la acción 'Mover Arriba' está activa.</summary>
        public bool Arriba { get; private set; }
        
        /// <summary>Indica si la acción 'Mover Abajo' está activa.</summary>
        public bool Abajo { get; private set; }
        
        /// <summary>Indica si la acción 'Mover Izquierda' está activa.</summary>
        public bool Izquierda { get; private set; }
        
        /// <summary>Indica si la acción 'Mover Derecha' está activa.</summary>
        public bool Derecha { get; private set; }
        
        /// <summary>Indica si la acción 'Disparar' está activa.</summary>
        public bool Disparar { get; private set; }
        
        /// <summary>Indica si la acción 'Salir' está activa.</summary>
        public bool Salir { get; private set; }

        #endregion

        /// <summary>
        /// Actualiza el estado de las acciones basado en un evento de teclado.
        /// </summary>
        /// <param name="e">Los argumentos del evento de teclado.</param>
        /// <param name="presionado">True si la tecla fue presionada, false si fue soltada.</param>
        public void ProcesarTecla(KeyEventArgs e, bool presionado)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    Arriba = presionado;
                    break;

                case Keys.S:
                case Keys.Down:
                    Abajo = presionado;
                    break;

                case Keys.A:
                case Keys.Left:
                    Izquierda = presionado;
                    break;

                case Keys.D:
                case Keys.Right:
                    Derecha = presionado;
                    break;

                case Keys.Space:
                    Disparar = presionado;
                    break;
                
                case Keys.Escape:
                    Salir = presionado;
                    break;
            }
        }
    }
}