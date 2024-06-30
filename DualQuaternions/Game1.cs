using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Windows.Markup;
using DualQuaternions;

namespace DualQuaternions {

    public class Game1 : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _tris;
        private BasicEffect _basicEffect;

        private float azimuth;
        private float altitude;
        private Vector3 target;
        private Vector3 distance;

        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        private DualQuaternion _worldQuar0 = DualQuaternion.Identity;
        private DualQuaternion _worldQuar1 = DualQuaternion.Identity;
        private double timeLeft = 0;
        private int targetQuar;

        private Dictionary<Keys, bool> held = new Dictionary<Keys, bool>();

        private SetState setState = SetState.None;
        private float[] sets = new float[7];
        private string value = "";
        private bool point = false;
        private SpriteFont _font;

        internal enum SetState {
            None, 
            SetAX, SetAY, SetAZ, SetT, SetTX, SetTY, SetTZ,
            ApplyAX, ApplyAY, ApplyAZ, ApplyT, ApplyTX, ApplyTY, ApplyTZ,
        }

        public Game1() {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            VertexPositionColor[] verts = {
                new VertexPositionColor(new Vector3(0.1f, 0f, 0f), new Color(1f, 0, 0)),
                new VertexPositionColor(new Vector3(0f, 0.25f, 0f), new Color(1f, 0, 0)),
                new VertexPositionColor(new Vector3(-0.1f, 0f, 0f), new Color(0.5f, 0, 0)),
                new VertexPositionColor(new Vector3(0f, -0.25f, 0f), new Color(0.5f, 0, 0)),

                new VertexPositionColor(new Vector3(0.1f, 0f, 1f), new Color(1f, 0, 0)),
                new VertexPositionColor(new Vector3(0f, 0.25f, 1f), new Color(1f, 0, 0)),
                new VertexPositionColor(new Vector3(-0.1f, 0f, 1f), new Color(0.5f, 0, 0)),
                new VertexPositionColor(new Vector3(0f, -0.25f, 1f), new Color(0.5f, 0, 0)),

                new VertexPositionColor(new Vector3(0.2f, 0f, 1f), new Color(1f, 0, 0)),
                new VertexPositionColor(new Vector3(0f, 0.5f, 1f), new Color(1f, 0, 0)),
                new VertexPositionColor(new Vector3(-0.2f, 0f, 1f), new Color(0.5f, 0, 0)),
                new VertexPositionColor(new Vector3(0f, -0.5f, 1f), new Color(0.5f, 0, 0)),

                new VertexPositionColor(new Vector3(0f, 0f, 2f), new Color(1f, 1f, 1f)),
            };

            short[] inds = {
                0, 1, 2, 2, 3, 0, 

                0, 1, 4, 1, 4, 5, 
                1, 2, 5, 2, 5, 6,
                2, 3, 6, 3, 6, 7,
                3, 0, 7, 0, 7, 4, 

                4, 5, 8, 5, 8, 9, 
                5, 6, 9, 6, 9,10,
                6, 7,10, 7,10,11,
                7, 4,11, 4,11, 8,

                8, 9, 12,
                9, 10,12,
                10,11,12,
                11, 8,12,
            };

            _tris = inds.Length / 3;

            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), verts.Length, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), inds.Length, BufferUsage.WriteOnly);

            _vertexBuffer.SetData(verts);
            _indexBuffer.SetData(inds);
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            GraphicsDevice.Indices = _indexBuffer;

            _basicEffect = new BasicEffect(GraphicsDevice);

            azimuth = 0;
            altitude = 0;
            target = Vector3.Zero;
            distance = new Vector3(-2, 0, 0);

            _world = _worldQuar0.ConvertToMatrix();
            _view = Matrix.Multiply(Matrix.CreateLookAt(target + distance, target, Vector3.UnitZ), Matrix.CreateFromYawPitchRoll(azimuth, altitude, 0));
            _projection = Matrix.CreatePerspectiveFieldOfView((float)(Math.PI / 2), 1920f / 1080f, 0.1f, 100f);

            held[Keys.W] = false;
            held[Keys.A] = false;
            held[Keys.S] = false;
            held[Keys.Z] = false;
            held[Keys.X] = false;
            held[Keys.D] = false;
            held[Keys.Q] = false;
            held[Keys.E] = false;
            held[Keys.R] = false;
            held[Keys.F] = false;
            held[Keys.C] = false;

            held[Keys.Decimal] = false;
            held[Keys.NumPad0] = false;
            held[Keys.NumPad1] = false;
            held[Keys.NumPad2] = false;
            held[Keys.NumPad3] = false;
            held[Keys.NumPad4] = false;
            held[Keys.NumPad5] = false;
            held[Keys.NumPad6] = false;
            held[Keys.NumPad7] = false;
            held[Keys.NumPad8] = false;
            held[Keys.NumPad9] = false;

            held[Keys.OemMinus] = false;
            held[Keys.Subtract] = false;

            held[Keys.T] = false;
            held[Keys.G] = false;
            held[Keys.Tab] = false;
            held[Keys.Enter] = false;
            held[Keys.Escape] = false;
            held[Keys.Back] = false;

            _font = Content.Load<SpriteFont>("File");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime) {
            double dt = gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                if (!held[Keys.Escape]) {
                    if (setState == SetState.None) Exit();
                    setState = SetState.None;
                }
                held[Keys.Escape] = true;
            } else {
                held[Keys.Escape] = false;
            }

            bool cameraAltered = false;

            if (Keyboard.GetState().IsKeyDown(Keys.W)) {
                if (!held[Keys.W]) {
                }
                target += Vector3.Transform(new Vector3(1f * (float)dt, 0, 0), Matrix.CreateRotationZ(-azimuth));
                cameraAltered = true;
                held[Keys.W] = true;
            } else {
                held[Keys.W] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A)) {
                if (!held[Keys.A]) {
                }
                target += Vector3.Transform(new Vector3(0, 1f * (float)dt, 0), Matrix.CreateRotationZ(-azimuth));
                cameraAltered = true;
                held[Keys.A] = true;
            } else {
                held[Keys.A] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) {
                if (!held[Keys.S]) {
                }
                target += Vector3.Transform(new Vector3(-1f * (float)dt, 0, 0), Matrix.CreateRotationZ(-azimuth));
                cameraAltered = true;
                held[Keys.S] = true;
            } else {
                held[Keys.S] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) {
                if (!held[Keys.D]) {
                }
                target += Vector3.Transform(new Vector3(0, -1f * (float)dt, 0), Matrix.CreateRotationZ(-azimuth));
                cameraAltered = true;
                held[Keys.D] = true;
            } else {
                held[Keys.D] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Z)) {
                if (!held[Keys.Z]) {
                }
                target += new Vector3(0, 0, -1f * (float)dt);
                cameraAltered = true;
                held[Keys.Z] = true;
            } else {
                held[Keys.Z] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.X)) {
                if (!held[Keys.X]) {
                }
                target += new Vector3(0, 0, 1f * (float)dt);
                cameraAltered = true;
                held[Keys.X] = true;
            } else {
                held[Keys.X] = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q)) {
                if (!held[Keys.Q]) {
                }
                azimuth -= (float)Math.PI / 5f * (float)dt;
                cameraAltered = true;
                held[Keys.Q] = true;
            } else {
                held[Keys.Q] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E)) {
                if (!held[Keys.E]) {
                }
                azimuth += (float)Math.PI / 5f * (float)dt;
                cameraAltered = true;
                held[Keys.E] = true;
            } else {
                held[Keys.E] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R)) {
                if (!held[Keys.R]) {
                }
                altitude -= (float)Math.PI / 5f * (float)dt;
                cameraAltered = true;
                held[Keys.R] = true;
            } else {
                held[Keys.R] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F)) {
                if (!held[Keys.F]) {
                }
                altitude += (float)Math.PI / 5f * (float)dt;
                cameraAltered = true;
                held[Keys.F] = true;
            } else {
                held[Keys.F] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.C)) {
                if (!held[Keys.C]) {
                    altitude = 0;
                    azimuth = 0;
                    target = Vector3.Zero;
                    cameraAltered = true;
                }
                held[Keys.C] = true;
            } else {
                held[Keys.C] = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T)) {
                if (!held[Keys.T]) {
                    setState = SetState.SetAX;
                    point = false;
                    value = "";
                }
                held[Keys.T] = true;
            } else {
                held[Keys.T] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G)) {
                if (!held[Keys.G]) {
                    setState = SetState.ApplyAX;
                    point = false;
                    value = "";
                }
                held[Keys.G] = true;
            } else {
                held[Keys.G] = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                if (!held[Keys.Enter]) {
                    if (value == "") value = "0";
                    if (value == "-") value = "0";
                    switch (setState) {
                        case SetState.None:
                            break;
                        case SetState.SetAX:
                            setState = SetState.SetAY;
                            sets[0] = float.Parse(value);
                            break;
                        case SetState.SetAY:
                            setState = SetState.SetAZ;
                            sets[1] = float.Parse(value);
                            break;
                        case SetState.SetAZ:
                            setState = SetState.SetT;
                            sets[2] = float.Parse(value);
                            break;
                        case SetState.SetT:
                            setState = SetState.SetTX;
                            sets[3] = float.Parse(value);
                            break;
                        case SetState.SetTX:
                            setState = SetState.SetTY;
                            sets[4] = float.Parse(value);
                            break;
                        case SetState.SetTY:
                            setState = SetState.SetTZ;
                            sets[5] = float.Parse(value);
                            break;
                        case SetState.SetTZ:
                            setState = SetState.None;
                            sets[6] = float.Parse(value);
                            if (sets[0] == 0 && sets[1] == 0 && sets[2] == 0) {
                                sets[0] = 1; sets[3] = 0;
                            }
                            switch (targetQuar) {
                                case 0:
                                    _worldQuar0 = DualQuaternion.MakeFromAxisAngleTrans(new Vector3(sets[0], sets[1], sets[2]), sets[3], new Vector3(sets[4], sets[5], sets[6]));
                                    break;
                                case 1:
                                    _worldQuar1 = DualQuaternion.MakeFromAxisAngleTrans(new Vector3(sets[0], sets[1], sets[2]), sets[3], new Vector3(sets[4], sets[5], sets[6]));
                                    break;
                            }
                            break;
                        case SetState.ApplyAX:
                            setState = SetState.ApplyAY;
                            sets[0] = float.Parse(value);
                            break;
                        case SetState.ApplyAY:
                            setState = SetState.ApplyAZ;
                            sets[1] = float.Parse(value);
                            break;
                        case SetState.ApplyAZ:
                            setState = SetState.ApplyT;
                            sets[2] = float.Parse(value);
                            break;
                        case SetState.ApplyT:
                            setState = SetState.ApplyTX;
                            sets[3] = float.Parse(value);
                            break;
                        case SetState.ApplyTX:
                            setState = SetState.ApplyTY;
                            sets[4] = float.Parse(value);
                            break;
                        case SetState.ApplyTY:
                            setState = SetState.ApplyTZ;
                            sets[5] = float.Parse(value);
                            break;
                        case SetState.ApplyTZ:
                            setState = SetState.None;
                            sets[6] = float.Parse(value);
                            if (sets[0] == 0 && sets[1] == 0 && sets[2] == 0) {
                                sets[0] = 1; sets[3] = 0;
                            }
                            switch (targetQuar) {
                                case 0:
                                    _worldQuar0 = _worldQuar0 * DualQuaternion.MakeFromAxisAngleTrans(new Vector3(sets[0], sets[1], sets[2]), sets[3], new Vector3(sets[4], sets[5], sets[6]));
                                    break;
                                case 1:
                                    _worldQuar1 = _worldQuar1 * DualQuaternion.MakeFromAxisAngleTrans(new Vector3(sets[0], sets[1], sets[2]), sets[3], new Vector3(sets[4], sets[5], sets[6]));
                                    break;
                            }
                            break;
                    }
                    point = false;
                    value = "";
                }
                held[Keys.Enter] = true;
            } else {
                held[Keys.Enter] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Back)) {
                if (!held[Keys.Back]) {
                    if (value.Length > 0) {
                        if (value[value.Length - 1] == '.') point = false;
                        value = value.Substring(0, value.Length - 1);
                    }
                }
                held[Keys.Back] = true;
            } else {
                held[Keys.Back] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Decimal)) {
                if (!held[Keys.Decimal] && !point) {
                    point = true;
                    value += '.';
                }
                held[Keys.Decimal] = true;
            } else {
                held[Keys.Decimal] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus)) {
                if (!held[Keys.OemMinus] && !point) {
                    point = true;
                    if (value == "") {
                        value += '-';
                    }
                }
                held[Keys.OemMinus] = true;
            } else {
                held[Keys.OemMinus] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Subtract)) {
                if (!held[Keys.Subtract] && !point) {
                    point = true;
                    if (value == "") {
                        value += '-';
                    }
                }
                held[Keys.Subtract] = true;
            } else {
                held[Keys.Subtract] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) {
                if (!held[Keys.NumPad0]) {
                    if (setState == SetState.None) {
                        value = "0";
                        targetQuar = 0;
                        timeLeft = 1;
                    } else {
                        value += '0';
                    }
                }
                held[Keys.NumPad0] = true;
            } else {
                held[Keys.NumPad0] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
                if (!held[Keys.NumPad1]) {
                    if (setState == SetState.None) {
                        value = "1";
                        targetQuar = 1;
                        timeLeft = 1;
                    } else {
                        value += '1';
                    }
                }
                held[Keys.NumPad1] = true;
            } else {
                held[Keys.NumPad1] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad2)) {
                if (!held[Keys.NumPad2]) {
                    value += '2';
                }
                held[Keys.NumPad2] = true;
            } else {
                held[Keys.NumPad2] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
                if (!held[Keys.NumPad3]) {
                    value += '3';
                }
                held[Keys.NumPad3] = true;
            } else {
                held[Keys.NumPad3] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
                if (!held[Keys.NumPad4]) {
                    value += '4';
                }
                held[Keys.NumPad4] = true;
            } else {
                held[Keys.NumPad4] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5)) {
                if (!held[Keys.NumPad5]) {
                    value += '5';
                }
                held[Keys.NumPad5] = true;
            } else {
                held[Keys.NumPad5] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
                if (!held[Keys.NumPad6]) {
                    value += '6';
                }
                held[Keys.NumPad6] = true;
            } else {
                held[Keys.NumPad6] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad7)) {
                if (!held[Keys.NumPad7]) {
                    value += '7';
                }
                held[Keys.NumPad7] = true;
            } else {
                held[Keys.NumPad7] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad8)) {
                if (!held[Keys.NumPad8]) {
                    value += '8';
                }
                held[Keys.NumPad8] = true;
            } else {
                held[Keys.NumPad8] = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad9)) {
                if (!held[Keys.NumPad9]) {
                    value += '9';
                }
                held[Keys.NumPad9] = true;
            } else {
                held[Keys.NumPad9] = false;
            }

            if (timeLeft > 0) {
                timeLeft = Math.Max(0, timeLeft - dt);
            }
            switch (targetQuar) {
                case 0:
                    _world = (timeLeft * _worldQuar1 + (1 - timeLeft) * _worldQuar0).Norm().ConvertToMatrix();
                    break;
                case 1:
                    _world = (timeLeft * _worldQuar0 + (1 - timeLeft) * _worldQuar1).Norm().ConvertToMatrix();
                    break;
            }
            if (cameraAltered) _view = Matrix.Multiply(Matrix.CreateLookAt(target + distance, target, Vector3.UnitZ), Matrix.CreateFromYawPitchRoll(azimuth, altitude, 0));
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            RasterizerState rasterState = new RasterizerState();
            rasterState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterState;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // TODO: Add your drawing code here

            _basicEffect.World = _world;
            _basicEffect.View = _view;
            _basicEffect.Projection = _projection;
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.LightingEnabled = false;
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _tris);

            string resolved = value == "" ? "0" : value;
            string prefix = "";
            switch (setState) {
                case SetState.None:
                    break;
                case SetState.SetAX:
                    prefix = "set ax = ";
                    break;
                case SetState.SetAY:
                    prefix = "set ay = ";
                    break;
                case SetState.SetAZ:
                    prefix = "set az = ";
                    break;
                case SetState.SetT:
                    prefix = "set t = ";
                    break;
                case SetState.SetTX:
                    prefix = "set tx = ";
                    break;
                case SetState.SetTY:
                    prefix = "set ty = ";
                    break;
                case SetState.SetTZ:
                    prefix = "set tz = ";
                    break;
                case SetState.ApplyAX:
                    prefix = "Apply ax = ";
                    break;
                case SetState.ApplyAY:
                    prefix = "Apply ay = ";
                    break;
                case SetState.ApplyAZ:
                    prefix = "Apply az = ";
                    break;
                case SetState.ApplyT:
                    prefix = "Apply t = ";
                    break;
                case SetState.ApplyTX:
                    prefix = "Apply tx = ";
                    break;
                case SetState.ApplyTY:
                    prefix = "Apply ty = ";
                    break;
                case SetState.ApplyTZ:
                    prefix = "Apply tz = ";
                    break;
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, prefix + resolved, Vector2.Zero, Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}