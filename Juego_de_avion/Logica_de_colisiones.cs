using System;
using System.Drawing;

namespace JuegoDeAvion
{
    public static class Colision
    {
        // Verifica si un rectángulo choca con otro
        public static bool Verificar(Rectangle r1, Rectangle r2)
        {
            return r1.IntersectsWith(r2);
        }

        // Verifica si la región compleja del avión choca con el rectángulo del asteroide
        public static bool Verificar(Region regionAvion, Rectangle rectObstaculo)
        {
            // Clonamos la región para no modificar la original del avión
            using (Region r = regionAvion.Clone())
            {
                // Intersectamos con el obstáculo
                r.Intersect(rectObstaculo);
                // Si la región resultante no está vacía, hubo choque
                return !r.IsEmpty(Graphics.FromImage(new Bitmap(1, 1)));
            }
        }
    }
}