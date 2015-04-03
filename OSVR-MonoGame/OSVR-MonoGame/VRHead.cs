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
using System.Diagnostics;

namespace OSVR
{
    namespace MonoGame
    {

        public enum ViewMode { Stereo, Mono };

        public interface IStereoSceneDrawer
        {
            void DrawScene(GameTime gameTime, Viewport viewport, Matrix view, Matrix projection);
        }

        public class VRHead
        {
            public ViewMode ViewMode { get; set; }

            /// <summary>
            /// Value between 0.0f and 1.0f
            /// </summary>
            public float StereoAmount { get; set; }

            private float maxStereo = .03f;
            public float MaxStereo { get { return maxStereo; } set { maxStereo = value; } }
            public float VerticalFieldOfView { get; private set; }
            public bool CameraEnabled { get; set; }

            readonly VREye leftEye;
            public VREye LeftEye { get { return leftEye; } }

            readonly VREye rightEye;
            public VREye RightEye { get { return rightEye; } }

            float previousStereoAmount;
            ViewMode previousViewMode;
            readonly GraphicsDeviceManager graphicsDeviceManager;
            DeviceDescriptor deviceDescriptor;
            readonly ClientKit clientKit;

            public VRHead(GraphicsDeviceManager graphicsDeviceManager, ClientKit clientKit)
            {
                this.graphicsDeviceManager = graphicsDeviceManager;
                this.clientKit = clientKit;
                leftEye = new VREye(graphicsDeviceManager.GraphicsDevice, Eye.Left);
                rightEye = new VREye(graphicsDeviceManager.GraphicsDevice, Eye.Right);
                // TODO: Provide a way to pass in an explicit json value?
                GetDeviceDescription();
            }

            void Update()
            {
                UpdateStereoAmount();
                UpdateViewMode();
            }

            bool firstUpdate = true;
            void UpdateViewMode()
            {
                if (firstUpdate || previousViewMode != ViewMode)
                {
                    switch (ViewMode)
                    {
                        case ViewMode.Mono:
                            CameraEnabled = true;
                            leftEye.CameraEnabled = false;
                            rightEye.CameraEnabled = false;
                            break;

                        case ViewMode.Stereo:
                            CameraEnabled = false;
                            leftEye.CameraEnabled = true;
                            rightEye.CameraEnabled = true;
                            break;
                    }
                }

                previousViewMode = ViewMode;
            }

            void UpdateStereoAmount()
            {
                if (StereoAmount != previousStereoAmount)
                {
                    StereoAmount = System.Math.Min(StereoAmount, 0);
                    StereoAmount = System.Math.Max(StereoAmount, 1);

                    var rightTransform = rightEye.Transform;
                    var leftTransform = leftEye.Transform;

                    LeftEye.Translation = Vector3.Left * (maxStereo * StereoAmount);
                    RightEye.Translation = Vector3.Right * (maxStereo * StereoAmount);

                    previousStereoAmount = StereoAmount;
                }
            }

            private void GetDeviceDescription()
            {
                var displayJson = clientKit.Context.getStringParameter("/display");
                Debug.WriteLine("/display:");
                Debug.WriteLine(displayJson);
                deviceDescriptor = DeviceDescriptor.Parse(displayJson);
                if(deviceDescriptor != null)
                {
                    switch(deviceDescriptor.DisplayMode)
                    {
                        case "full_screen":
                            ViewMode = MonoGame.ViewMode.Mono;
                            break;
                        case "horz_side_by_side":
                        case "vert_side_by_side":
                        default:
                            ViewMode = MonoGame.ViewMode.Stereo;
                            break;
                    }
                    StereoAmount = MathHelper.Clamp(deviceDescriptor.OverlapPercent, 0f, 100f);
                    SetResolution(deviceDescriptor.Width, deviceDescriptor.Height); // set resolution before FOV
                    VerticalFieldOfView = MathHelper.Clamp(deviceDescriptor.MonocularVertical, 0, 180);
                    // TODO: should we provide HorizontalFieldOfView?
                    //SetDistortion(deviceDescriptor.K1Red, deviceDescriptor.K1Green, deviceDescriptor.K1Blue,
                    //    deviceDescriptor.CenterProjX, deviceDescriptor.CenterProjY); //set distortion shader
                    
                    // if the view needs to be rotated 180 degrees, create a parent game object that is flipped
                    // 180 degrees on the z axis
                    if (deviceDescriptor.Rotate180 > 0)
                    {
                        LeftEye.RotatePi = true;
                        RightEye.RotatePi = true;
                    }

                    SetEyeRotation(deviceDescriptor.OverlapPercent, deviceDescriptor.MonocularHorizontal);
                    SetEyeRoll(deviceDescriptor.LeftRoll, deviceDescriptor.RightRoll);
                }
            }

            //private void SetDistortion(float k1Red, float k1Green, float k1Blue, float centerProjX, float centerProjY)
            //{
            //    if (_distortionEffect != null)
            //    {
            //        _distortionEffect.k1Red = k1Red;
            //        _distortionEffect.k1Green = k1Green;
            //        _distortionEffect.k1Blue = k1Blue;
            //        _distortionEffect.fullCenter = new Vector2(centerProjX, centerProjY);
            //    }
            //}

            //Set the Screen Resolution
            private void SetResolution(int width, int height)
            {
                //set the resolution
                graphicsDeviceManager.PreferredBackBufferWidth = width;
                graphicsDeviceManager.PreferredBackBufferHeight = height;
                graphicsDeviceManager.IsFullScreen = true;
                graphicsDeviceManager.ApplyChanges();
            }

            // rotate each eye based on overlap percent and horizontal FOV
            // Formula: ((OverlapPercent/100) * hFOV)/2
            private void SetEyeRotation(float overlapPercent, float horizontalFov)
            {
                float overlap = overlapPercent * 0.01f * horizontalFov * 0.5f;

                // with a 90 degree FOV with 100% overlap, the eyes should not be rotated
                // compare rotationY with half of FOV

                float halfFOV = horizontalFov * 0.5f;
                float rotateYAmount = MathHelper.ToRadians(System.Math.Abs(overlap - halfFOV));

                LeftEye.EyeRotationY = -rotateYAmount;
                RightEye.EyeRotationY = rotateYAmount;
            }

            // rotate each eye on the z axis by the specified amount, in degrees
            private void SetEyeRoll(float leftRoll, float rightRoll)
            {
                LeftEye.EyeRoll = MathHelper.ToRadians(leftRoll);
                RightEye.EyeRoll = MathHelper.ToRadians(rightRoll);
            }

            // TODO: instead of an Action, maybe pass an interface?
            // I think that might be easier to use. Hard to document
            // what each action arguments should be.
            public void DrawScene(GameTime gameTime, IStereoSceneDrawer sceneDrawer)
            {
                DrawSceneForEye(LeftEye, gameTime, sceneDrawer);
                DrawSceneForEye(RightEye, gameTime, sceneDrawer);
                // TODO: implement monoscopic rendering, which will basically just call DrawScene
                // once with the full ViewPort and a non-stereo view matrix/projection.
            }

            private void DrawSceneForEye(VREye eye, GameTime gameTime, IStereoSceneDrawer sceneDrawer)
            {
                Viewport oldViewPort = graphicsDeviceManager.GraphicsDevice.Viewport;
                graphicsDeviceManager.GraphicsDevice.Viewport = eye.Viewport;
                sceneDrawer.DrawScene(gameTime, eye.Viewport, eye.Transform, eye.Projection);
                graphicsDeviceManager.GraphicsDevice.Viewport = oldViewPort;
            }
        }
    }
}