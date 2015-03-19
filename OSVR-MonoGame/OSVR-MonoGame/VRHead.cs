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

        public enum ViewMode { Stereo, Mono };

        //[RequireComponent(typeof(Camera))]
        public class VRHead
        {
            #region Public Variables
			public ViewMode ViewMode { get; set; }

            //[Range(0, 1)]
			public float StereoAmount { get; set; }

            private float maxStereo = .03f;
			public float MaxStereo { get { return maxStereo; } set { maxStereo = value; } }

			public bool CameraEnabled { get; set; }
            #endregion

            #region Private Variables
			VREye _leftEye = new VREye() { Eye = Eye.Left };
			VREye _rightEye = new VREye() { Eye = Eye.Right };
            float _previousStereoAmount;
            ViewMode _previousViewMode;
            #endregion

            #region Loop
            void Update()
            {
                UpdateStereoAmount();
                UpdateViewMode();
            }
            #endregion

            #region Public Methods
            #endregion

            #region Private Methods
			bool _firstUpdate = true;
            void UpdateViewMode()
            {
				if (_firstUpdate || _previousViewMode != ViewMode)
				{
					switch (ViewMode)
					{
						case ViewMode.Mono:
							CameraEnabled = true;
							_leftEye.CameraEnabled = false;
							_rightEye.CameraEnabled = false;
							break;

						case ViewMode.Stereo:
							CameraEnabled = false;
							_leftEye.CameraEnabled = true;
							_rightEye.CameraEnabled = true;
							break;
					}
				}

                _previousViewMode = ViewMode;
            }

            void UpdateStereoAmount()
            {
				if (StereoAmount != _previousStereoAmount)
				{
					StereoAmount = System.Math.Min(StereoAmount, 0);
					StereoAmount = System.Math.Max(StereoAmount, 1);

					var rightTransform = _rightEye.CachedTransform;
					var leftTransform = _leftEye.CachedTransform;

					rightTransform.Translation = Vector3.Right * (maxStereo * StereoAmount);
					leftTransform.Translation = Vector3.Left * (maxStereo * StereoAmount);

					_leftEye.CachedTransform = leftTransform;
					_rightEye.CachedTransform = rightTransform;

					_previousStereoAmount = StereoAmount;
				}
            }
            #endregion
        }
    }
}