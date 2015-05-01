using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OSVR.MonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSVR_MonoGame
{
    public class WasdOrientationSignal : IInterfaceSignal<Quaternion>
    {
        float yRotation = 0f;
        float xRotation = 0f;
        float rotationSpeed = 0.05f;
        int width;
        int height;

        public WasdOrientationSignal(Viewport viewport)
        {
            this.width = viewport.Width;
            this.height = viewport.Height;
        }

        public void Update(GameTime gameTime)
        {
            var currentKeyboardState = Keyboard.GetState();
            if (currentKeyboardState.IsKeyDown(Keys.Left)) xRotation += rotationSpeed;
            if (currentKeyboardState.IsKeyDown(Keys.Right)) xRotation -= rotationSpeed;
            if (currentKeyboardState.IsKeyDown(Keys.Up)) yRotation -= rotationSpeed;
            if (currentKeyboardState.IsKeyDown(Keys.Down)) yRotation += rotationSpeed;
        }

        public InterfaceCallbacks Interface
        {
            get { return null; }
        }

        public void Start()
        {
            // noop
        }

        public void Stop()
        {
            // noop
        }

        public Quaternion Value
        {
            get 
            {
                return Quaternion.CreateFromYawPitchRoll(xRotation, yRotation, 0);
            }
        }

        public event EventHandler<Quaternion> ValueChanged;
    }
}
