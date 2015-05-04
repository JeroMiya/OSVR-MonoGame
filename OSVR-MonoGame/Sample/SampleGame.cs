using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OSVR.MonoGame;
using OSVR_MonoGame;
using System;

namespace Sample
{
    public class SampleGame : Game, IStereoSceneDrawer
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont diagnosticFont;
        Texture2D blank;
        Model model;

        VRHead vrHead;
        IInterfaceSignal<PoseReport> leftHandPose;
        ClientKit clientKit;
        //KeyboardOrientationSignal keyboardOrientationSignal;
        MouselookOrientationSignal mouseOrientationSignal;
        IInterfaceSignal<Quaternion> orientationSignal;

        const float moveSpeed = 5f;
        Vector3 position = Vector3.Zero;
        float rotationY = 0f;

        public SampleGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            clientKit = new ClientKit("");
            //orientationSignal = new OrientationSignal("/me/head", clientKit);

            //keyboardOrientationSignal = new KeyboardOrientationSignal();
            mouseOrientationSignal = new MouselookOrientationSignal(GraphicsDevice.Viewport);
            orientationSignal = mouseOrientationSignal;
            
            leftHandPose = new PoseSignal("/me/hands/left", clientKit);
            leftHandPose.Start();

            vrHead = new VRHead(graphics, clientKit, orientationSignal);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            model = Content.Load<Model>("SketchupTest");
            diagnosticFont = Content.Load<SpriteFont>("DiagnosticFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });
            orientationSignal.Start();
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            orientationSignal.Stop();
            clientKit.Dispose();
            clientKit = null;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            else
            {
                var t = (float)gameTime.ElapsedGameTime.TotalSeconds;
                var kbs = Keyboard.GetState();
                Vector3 movement = Vector3.Zero;
                // forward/back
                if (kbs.IsKeyDown(Keys.W))
                {
                    movement = Vector3.Forward * moveSpeed * t;
                }
                else if (kbs.IsKeyDown(Keys.S))
                {
                    movement = Vector3.Backward * moveSpeed * t;
                }

                // left/right
                if (kbs.IsKeyDown(Keys.A))
                {
                    movement = Vector3.Left * moveSpeed * t;
                }
                else if (kbs.IsKeyDown(Keys.D))
                {
                    movement = Vector3.Right * moveSpeed * t;
                }

                // kb left/right rotation
                if(kbs.IsKeyDown(Keys.Left))
                {
                    rotationY += moveSpeed * t;
                }
                else if(kbs.IsKeyDown(Keys.Right))
                {
                    rotationY -= moveSpeed * t;
                }

                var kbRotation = Quaternion.CreateFromYawPitchRoll(rotationY, 0f, 0f);
                var transformedMovement = Vector3.Transform(movement, kbRotation * orientationSignal.Value);
                position = position + transformedMovement;

                // increase/decrease stereo amount
                if (kbs.IsKeyDown(Keys.Q)) { vrHead.IPDInMeters += .01f * t; }
                if (kbs.IsKeyDown(Keys.E)) { vrHead.IPDInMeters -= .01f * t; }

                vrHead.Update();
                //keyboardOrientationSignal.Update(gameTime);
                mouseOrientationSignal.Update(gameTime);

                // clientKit.Update must be called frequently
                // perhaps more frequently than Update is called?
                clientKit.Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            vrHead.DrawScene(gameTime, spriteBatch, this);
            base.Draw(gameTime);
        }

        public void DrawScene(GameTime gameTime, Viewport viewport, Matrix stereoTransform, Matrix view, Matrix projection)
        {
            // TODO: Draw something fancy. Or at the very least visible?
            var translation = Matrix.CreateTranslation(Vector3.Negate(position));
            var kbRotation = Matrix.CreateRotationY(-rotationY);
            var cameraView = stereoTransform * translation * kbRotation * view;

            // Draw the model. A model can have multiple meshes, so loop.
            var modelWorld = Matrix.CreateTranslation(0f, -5f, -5f);
            foreach (ModelMesh mesh in model.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = modelWorld;
                    effect.View = cameraView;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            var kbstate = Keyboard.GetState();
            if (kbstate.IsKeyDown(Keys.Q) || kbstate.IsKeyDown(Keys.E))
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(diagnosticFont, "IPD: " + (vrHead.IPDInMeters * 1000).ToString() + "mm",
                    new Vector2((float)viewport.Width / 2f, (float)viewport.Height / 2f), Color.White);
                spriteBatch.End();
            }
        }
    }
}
