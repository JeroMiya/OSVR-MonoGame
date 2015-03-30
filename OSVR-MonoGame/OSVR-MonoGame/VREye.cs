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

            public Eye Eye { get; private set; }

            /// <summary>
            /// Eye rotation around the Y axis, in radians
            /// </summary>
            public float EyeRotationY { get; set; }

            /// <summary>
            /// Eye roll around the Z axis, in radians
            /// </summary>
            public float EyeRoll { get; set; }

            public Vector3 Translation { get; set; }
            
            public bool RotatePi { get; set; }

            // TODO: Should we cache this transform and recalculate it
            // on an Update method?
            public Matrix Transform
            {
                get
                {
                    var ret = Matrix.CreateRotationY(EyeRotationY)
                        * Matrix.CreateRotationZ(EyeRoll);

                    if(RotatePi)
                    {
                        ret = ret * Matrix.CreateRotationZ(MathHelper.Pi);
                    }
                    ret = ret * Matrix.CreateTranslation(Translation);
                    return ret;
                }
            }

            // JEB: Do we need this?
            public bool CameraEnabled { get; set; }

            // TODO: Should we cache this Viewport and recalculate it
            // on an Update method?
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

            public VREye(GraphicsDevice graphicsDevice, Eye eye)
            {
                this.graphicsDevice = graphicsDevice;
                this.Eye = eye;
            }
        }
    }
}