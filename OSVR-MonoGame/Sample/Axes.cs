using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Axes
    {
        VertexPositionColor[] vertices = new VertexPositionColor[3];
        VertexBuffer vertexBuffer;
        BasicEffect basicEffect;

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            basicEffect = new BasicEffect(graphicsDevice);
            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
        }

        public void Draw(Matrix view, Matrix world, Matrix projection, GraphicsDevice graphicsDevice)
        {
            basicEffect.World = world;
            basicEffect.View = view;
            basicEffect.Projection = projection;
            basicEffect.VertexColorEnabled = true;

            vertices[0] = new VertexPositionColor(new Vector3(0f, 0f, 5f), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(5f, 0f, 0f), Color.Green);
            vertices[2] = new VertexPositionColor(new Vector3(-5f, 0f, 0f), Color.Blue);
            
            vertexBuffer.SetData<VertexPositionColor>(vertices);
            graphicsDevice.SetVertexBuffer(vertexBuffer);

            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rasterizerState;

            foreach(var pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
            }
        }
    }
}
