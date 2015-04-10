using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OSVR.MonoGame;
using System;

namespace Sample
{
    public class SampleGame : Game, IStereoSceneDrawer
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D blank;

        VRHead vrHead;
        ClientKit clientKit;
        IInterfaceSignal<Quaternion> orientationSignal;
        public SampleGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            clientKit = new ClientKit("");
            orientationSignal = new OrientationSignal("/me/head", clientKit);
            vrHead = new VRHead(graphics, clientKit, orientationSignal);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData(new[] { Color.White });
            orientationSignal.Start();
        }

        protected override void UnloadContent()
        {
            orientationSignal.Stop();
            clientKit.Dispose();
            clientKit = null;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
        }
    }
}
