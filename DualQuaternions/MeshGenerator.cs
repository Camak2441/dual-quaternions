using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DualQuaternions {
    internal class MeshGenerator {

        public (VertexBuffer, IndexBuffer) BonedCylinder(float rad, int sides, float height, int divs, Func<Vector3, (double w1, double w2)> weights, Color wc1, Color wc2, DualQuaternion q1, DualQuaternion q2, Func<DualQuaternion, double, DualQuaternion, double, Matrix> blend) {
            VertexPositionColor[] verts = new VertexPositionColor[sides * (divs + 1)];
            int i, j;
            for (i = 0; i < sides; ++i) {
                for (j = 1; j <= divs; ++j) {

                }
            }

            throw new NotImplementedException();
        }

    }
}
