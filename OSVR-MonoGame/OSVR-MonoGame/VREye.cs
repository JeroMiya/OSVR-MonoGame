/* OSVR-Unity Connection
 * 
 * <http://sensics.com/osvr>
 * Copyright 2014 Sensics, Inc.
 * All rights reserved.
 * 
 * Final version intended to be licensed under Apache v2.0
 */
/// <summary>
/// Author: Bob Berkebile
/// Email: bob@bullyentertainment.com || bobb@pixelplacement.com
/// </summary>

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;

namespace OSVR
{
    namespace MonoGame
    {
        public enum Eye { Left, Right };

        public class VREye
        {
            private readonly GraphicsDevice graphicsDevice;

            #region Public Variables
            public Eye Eye { get; private set; }

            public Matrix Transform { get; set; }

            // JEB: Do we need this?
            public bool CameraEnabled { get; set; }

            public Viewport Viewport {
                get
                {
                    switch(Eye)
                    {
                        case MonoGame.Eye.Left:
                            return new Viewport
                            {
                                MinDepth = 0,
                                MaxDepth = 1,
                                X = 0,
                                Y = 0,
                                Width = graphicsDevice.Viewport.Width / 2,
                                Height = graphicsDevice.Viewport.Height,
                            };
                        case MonoGame.Eye.Right:
                            return new Viewport
                            {
                                MinDepth = 0,
                                MaxDepth = 1,
                                X = graphicsDevice.Viewport.Width / 2,
                                Y = 0,
                                Width = graphicsDevice.Viewport.Width / 2,
                                Height = graphicsDevice.Viewport.Height,
                            };
                    }
                    throw new InvalidOperationException("Unexpected eye type.");
                }
            }
            #endregion

            public VREye(GraphicsDevice graphicsDevice, Eye eye)
            {
                this.graphicsDevice = graphicsDevice;
                this.Eye = eye;
            }
        }
    }
}