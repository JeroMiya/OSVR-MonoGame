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
        Texture2D blank;
        Axes axes;

        VRHead vrHead;
        ClientKit clientKit;
        WasdOrientationSignal orientationSignal;

        public SampleGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        const float moveSpeed = 5f;
        float posX = 0f;
        float posY = 0f;
        protected override void Initialize()
        {
            clientKit = new ClientKit("");
            //orientationSignal = new OrientationSignal("/me/head", clientKit);
            orientationSignal = new WasdOrientationSignal(GraphicsDevice.Viewport);
            vrHead = new VRHead(graphics, clientKit, orientationSignal);
            axes = new Axes();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            axes.LoadContent(GraphicsDevice);
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
                Exit();
            var t = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Left)) { posX += moveSpeed * t; }
            if (kbs.IsKeyDown(Keys.Right)) { posX -= moveSpeed * t; }
            if (kbs.IsKeyDown(Keys.Up)) { posY += moveSpeed * t; }
            if (kbs.IsKeyDown(Keys.Down)) { posY -= moveSpeed * t; }

            // increase/decrease stereo amount
            if (kbs.IsKeyDown(Keys.Q)) { vrHead.WorldUnitsPerMeter += 5.0f * t; }
            if (kbs.IsKeyDown(Keys.E)) { vrHead.WorldUnitsPerMeter -= 5.0f * t; }

            vrHead.Update();
            orientationSignal.Update(gameTime);

            // clientKit.Update must be called frequently
            // perhaps more frequently than Update is called?
            clientKit.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            vrHead.DrawScene(gameTime, this);

            base.Draw(gameTime);
        }

        public void DrawScene(GameTime gameTime, Viewport viewport, Matrix view, Matrix projection)
        {
            // TODO: Draw something fancy. Or at the very least visible?
            var translation = Matrix.CreateTranslation(new Vector3(posX, posY, 0f));
            var cameraView = translation * view;
            for (int i = 0; i < 10; i++)
            {
                axes.Draw(cameraView, Matrix.CreateTranslation(10f, 10f, -10 * i), projection, GraphicsDevice);
                axes.Draw(cameraView, Matrix.CreateTranslation(10f, -10 * i, 10f), projection, GraphicsDevice);
                axes.Draw(cameraView, Matrix.CreateTranslation(-10 * i, 10f, 10f), projection, GraphicsDevice);
            }
        }
    }
}
