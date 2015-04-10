using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace OSVR
{
    namespace MonoGame
    {
        /// <summary>
        /// Interface signal for Orientation data.
        /// </summary>
        public class OrientationSignal : InterfaceSignalBase<Quaternion>
        {
            OSVR.MonoGame.InterfaceCallback<Quaternion> cb;

            public OrientationSignal(string path, OSVR.MonoGame.IClientKit clientKit)
                : base(path, clientKit)
            {
                Value = Quaternion.Identity;
            }

            public override void Start()
            {
                base.Start();
                cb = new MonoGame.InterfaceCallback<Quaternion>(Callback);
                Interface.RegisterCallback(cb);
            }
        }
    }
}