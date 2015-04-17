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
            private readonly IInterfaceSignal<Quaternion> orientationSignal;

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
                    // orientation matrix
                    var orientationRotation = Matrix.CreateFromQuaternion(this.orientationSignal.Value);
                    
                    // eye device rotation
                    var pitch = EyeRotationY;
                    var roll = EyeRoll;
                    var yaw = RotatePi ? MathHelper.Pi : 0f;
                    var eyeRotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);

                    // translate (Should this be eyeRotation * orientationRotation)
                    var translation = Matrix.CreateTranslation(Translation);
                    var ret = orientationRotation * eyeRotation * translation;
                    return ret;
                }
            }

            public Matrix Projection
            {
                get
                {
                    return Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.PiOver4, Viewport.AspectRatio, 0.01f, 5000f);
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

            public VREye(GraphicsDevice graphicsDevice, IInterfaceSignal<Quaternion> orientationSignal, Eye eye)
            {
                this.graphicsDevice = graphicsDevice;
                this.orientationSignal = orientationSignal;
                this.Eye = eye;
            }
        }
    }
}