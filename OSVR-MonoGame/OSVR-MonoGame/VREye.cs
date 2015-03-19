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
using System.Collections;

namespace OSVR
{
    namespace MonoGame
    {
        public enum Eye { Left, Right };

        public class VREye
        {
            #region Public Variables
			public Eye Eye { get; set; }

			// JEB: I should probably just rename this to Transform
			public Matrix CachedTransform { get; set; }

			// JEB: Do we need this?
			public bool CameraEnabled { get; set; }
			
			// JEB: This should probably be renamed ViewPort, but we'd have to pass in
			// the graphics device dimensions somehow to calculate them.
			public Vector4 CameraRectangle { get; set; }

            #endregion

            #region Init
            void Awake()
            {
                Init();
            }
            #endregion

            #region Private Methods
            void Init()
            {
				// JEB: TODO - reimplement this to do viewport calculations. MonoGame
				// has no concept of a Camera other than utilities to create matrices.

				//camera setups:
				switch (Eye)
				{
					case Eye.Left:
						CameraRectangle = new Vector4(0, 0, .5f, 1);
						break;
					case Eye.Right:
						CameraRectangle = new Vector4(.5f, 0, .5f, 1);
						break;
				}
            }
            #endregion
        }
    }
}