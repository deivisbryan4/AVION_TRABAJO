using System.Windows.Forms;

namespace JuegoDeAvion
{
    public class InputHandler
    {
        public bool Arriba { get; private set; }
        public bool Abajo { get; private set; }
        public bool Izquierda { get; private set; }
        public bool Derecha { get; private set; }
        public bool Disparar { get; private set; }
        public bool Salir { get; private set; }

        public void ProcesarTecla(KeyEventArgs e, bool presionado)
        {
            switch (e.KeyCode)
            {
                // Movimiento Arriba
                case Keys.W:
                case Keys.Up:
                    Arriba = presionado;
                    break;

                // Movimiento Abajo
                case Keys.S:
                case Keys.Down:
                    Abajo = presionado;
                    break;

                // Movimiento Izquierda
                case Keys.A:
                case Keys.Left:
                    Izquierda = presionado;
                    break;

                // Movimiento Derecha
                case Keys.D:
                case Keys.Right:
                    Derecha = presionado;
                    break;

                // Disparo
                case Keys.Space:
                    Disparar = presionado;
                    break;
                
                // Salir
                case Keys.Escape:
                    Salir = presionado;
                    break;
            }
        }
    }
}