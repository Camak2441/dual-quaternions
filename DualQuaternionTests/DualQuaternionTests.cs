using DualQuaternions;
using Microsoft.Xna.Framework;

namespace DualQuaternionTests {
    [TestClass]
    public class DualQuaternionTests {
        public const double ERR = 1e-5;
        public const double SQRT2 = 1.41421356237;
        public const double SQRT3 = 1.73205080757;
        [TestMethod]
        public void Add() {
            Assert.AreEqual(((DualQuaternions.Quaternion)(1, 1, 1, 1)) + (1, 2, 3, 4), (DualQuaternions.Quaternion)(2, 3, 4, 5));
        }

        [TestMethod]
        public void Sub() {
            Assert.AreEqual(((DualQuaternions.Quaternion)(1, 1, 1, 1)) - (1, 2, 3, 4), (DualQuaternions.Quaternion)(0, -1, -2, -3));
        }

        [TestMethod]
        public void MultDiv() {
            DualQuaternions.Quaternion a = (1, 2, 3, 4);
            DualQuaternions.Quaternion b = (5, 6, 7, 8);
            Assert.AreEqual(a * b / b, a);
        }

        [TestMethod]
        public void QuatRot1() {
            DualQuaternions.Quaternion t = DualQuaternions.Quaternion.MakeFromAxisAngle(Vector3.UnitZ, Math.PI / 2);
            Vector3 res = t.Apply(Vector3.UnitX);
            if (!(-ERR < res.X && res.X < ERR && 1 - ERR < res.Y && res.Y < 1 + ERR && -ERR < res.Z && res.Z < ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void QuatRot2() {
            DualQuaternions.Quaternion t = DualQuaternions.Quaternion.MakeFromAxisAngle(Vector3.UnitZ, Math.PI / 4);
            Vector3 res = t.Apply(Vector3.UnitX);
            if (!(1 / SQRT2 - ERR < res.X && res.X < 1 / SQRT2 + ERR && 1 / SQRT2 - ERR < res.Y && res.Y < 1 / SQRT2 + ERR && -ERR < res.Z && res.Z < ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void QuatRot3() {
            DualQuaternions.Quaternion t1 = DualQuaternions.Quaternion.MakeFromAxisAngle(Vector3.UnitY, -0.615479709);
            DualQuaternions.Quaternion t2 = DualQuaternions.Quaternion.MakeFromAxisAngle(Vector3.UnitZ, Math.PI / 4);
            Vector3 res = t2.Apply(t1.Apply(Vector3.UnitX));            
            if (!(1 / SQRT3 - ERR < res.X && res.X < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Y && res.Y < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Z && res.Z < 1 / SQRT3 + ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void QuatRot4() {
            DualQuaternions.Quaternion t1 = DualQuaternions.Quaternion.MakeFromAxisAngle(Vector3.UnitY, -0.615479709);
            DualQuaternions.Quaternion t2 = DualQuaternions.Quaternion.MakeFromAxisAngle(Vector3.UnitZ, Math.PI / 4);
            Vector3 res = (t2*t1).Apply(Vector3.UnitX);           
            if (!(1 / SQRT3 - ERR < res.X && res.X < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Y && res.Y < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Z && res.Z < 1 / SQRT3 + ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DQuatRot1() {
            DualQuaternion t = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitZ, Math.PI / 2, Vector3.Zero);
            Vector3 res = t.Apply(Vector3.UnitX);
            if (!(-ERR < res.X && res.X < ERR && 1 - ERR < res.Y && res.Y < 1 + ERR && -ERR < res.Z && res.Z < ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DQuatRot2() {
            DualQuaternion t = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitZ, Math.PI / 4, Vector3.Zero);
            Vector3 res = t.Apply(Vector3.UnitX);
            if (!(1 / SQRT2 - ERR < res.X && res.X < 1 / SQRT2 + ERR && 1 / SQRT2 - ERR < res.Y && res.Y < 1 / SQRT2 + ERR && -ERR < res.Z && res.Z < ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DQuatRot3() {
            DualQuaternion t1 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitY, -0.615479709, Vector3.Zero);
            DualQuaternion t2 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitZ, Math.PI / 4, Vector3.Zero);
            Vector3 res = t2.Apply(t1.Apply(Vector3.UnitX));
            if (!(1 / SQRT3 - ERR < res.X && res.X < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Y && res.Y < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Z && res.Z < 1 / SQRT3 + ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DQuatRot4() {
            DualQuaternion t1 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitY, -0.615479709, Vector3.Zero);
            DualQuaternion t2 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitZ, Math.PI / 4, Vector3.Zero);
            Vector3 res = (t2 * t1).Apply(Vector3.UnitX);
            if (!(1 / SQRT3 - ERR < res.X && res.X < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Y && res.Y < 1 / SQRT3 + ERR && 1 / SQRT3 - ERR < res.Z && res.Z < 1 / SQRT3 + ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DQuatTrans1() {
            DualQuaternion t1 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitX, 0, new Vector3(1, 2, 3));
            Assert.AreEqual(t1.Apply(new Vector3(1, 1, 1)), new Vector3(2, 3, 4));
            Assert.AreEqual(t1.Apply(new Vector3(-1, -1, -1)), new Vector3(0, 1, 2));
        }

        [TestMethod]
        public void DQuatTrans2() {
            DualQuaternion t1 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitX, 0, new Vector3(1, 2, 3));
            DualQuaternion t2 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitX, 0, new Vector3(-10, -9, -7));
            Assert.AreEqual(t2.Apply(t1.Apply(new Vector3(1, 1, 1))), new Vector3(-8, -6, -3));
            Assert.AreEqual(t2.Apply(t1.Apply(new Vector3(-1, -1, -1))), new Vector3(-10, -8, -5));
        }

        [TestMethod]
        public void DQuatTransform1() {
            DualQuaternion t1 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitX, 0, new Vector3(1, 0, 0));
            DualQuaternion t2 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitZ, Math.PI, Vector3.Zero);
            DualQuaternion t3 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitX, 0, new Vector3(-1, 0, 0));
            DualQuaternion t = t3 * t2 * t1;
            Vector3 res = t.Apply(Vector3.UnitY);
            if (!(-2 - ERR < res.X && res.X < -2 + ERR && -1 - ERR < res.Y && res.Y < -1 + ERR && -ERR < res.Z && res.Z < ERR)) {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void DQuatTransform2() {
            DualQuaternion t1 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitX, Math.PI, new Vector3(1, 0, 0));
            DualQuaternion t2 = DualQuaternion.MakeFromAxisAngleTrans(Vector3.UnitZ, Math.PI, new Vector3(-1, 0, 0));
            DualQuaternion t = t2 * t1;
            Vector3 res = t.Apply(Vector3.UnitY);
            if (!(-2 - ERR < res.X && res.X < -2 + ERR && 1 - ERR < res.Y && res.Y < 1 + ERR && -ERR < res.Z && res.Z < ERR)) {
                Assert.Fail();
            }
        }
    }
}