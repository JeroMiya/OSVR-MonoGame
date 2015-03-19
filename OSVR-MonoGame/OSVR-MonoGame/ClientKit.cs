/*
 * Based loosely on ClientKit.cs from OSVR-Unity by Sensics, Inc.
 */

using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace OSVR
{
    namespace MonoGame
    {
		public interface IClientKit : IDisposable
		{
			string AppID { get; }
			OSVR.ClientKit.ClientContext Context { get; }
		}

        public class ClientKit : IClientKit
        {
            /// <summary>
            /// Static constructor that enhances the DLL search path to ensure dependent native dlls are found.
            /// </summary>
            static ClientKit()
            {
                DLLSearchPathFixer.fix();
            }

            private readonly string appID;
			public string AppID { get { return AppID; } }
            private OSVR.ClientKit.ClientContext context;

			public ClientKit(string appID)
			{
				if (appID == null)
				{
					throw new ArgumentNullException("appID");
				}
				if (0 == appID.Length)
				{
					Debug.WriteLine("OSVR ClientKit instance needs AppID set to a reverse-order DNS name! Using dummy name...");
					this.appID = "org.opengoggles.osvr-unity.dummy";
				}
				else
				{
					this.appID = appID;
				}

				Debug.WriteLine("[OSVR] Starting with app ID: " + AppID);
				context = new OSVR.ClientKit.ClientContext(AppID, 0);

			}

			public void Update(GameTime gameTime)
			{
				if (disposed) { throw new ObjectDisposedException("context"); }
				context.update();
			}

            /// <summary>
            /// Access the underlying Managed-OSVR client context object.
            /// </summary>
            public OSVR.ClientKit.ClientContext Context
            {
                get
                {
					if (disposed) { throw new ObjectDisposedException("context"); }
					return context;
                }
            }

			#region Basic Dispose Pattern

			private bool disposed = false;
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected void Dispose(bool disposing)
			{
				if (disposed) {
					return;
				}

				if (disposing)
				{
					if (context != null) { context.Dispose(); }
				}
				context = null;
				disposed = true;
			}

			#endregion

		}
    }
}