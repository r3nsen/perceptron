using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.IO;

namespace perceptron
{
    class GraphicsManager
    {
        GraphicsDevice gd;

        public Effect network, drawEffect, circle, square;

        Matrix view, projection;

        VertexPositionColorTexture[] vertex = new VertexPositionColorTexture[4];
        short[] index = new short[6];
        int vertexCount, indexCount;

        public RenderTarget2D inputs, output;

        public Texture2D weigths;
        float bias;
        Random rand;

        const int size = 200;
        public GraphicsManager(GraphicsDevice _gd)
        {
            gd = _gd;
            inputs = new RenderTarget2D(gd, size, size);
            weigths = new Texture2D(gd, size, size);

            rand = new Random(69);

            Color[] randColors = new Color[size * size];
            for (int i = 0; i < size * size; i++)
            {
                int r = rand.Next(0, 255);
                randColors[i] = new Color(r, r, r);
            }
            inputs.SetData(randColors);
            for (int i = 0; i < size * size; i++)
            {
                int r = rand.Next(0, 255); 
                randColors[i] = new Color(r, r, r);
            }
            weigths.SetData(randColors);
            output = new RenderTarget2D(gd, size, size);       
        }

        public void Begin(int x, int y, int w, int h)
        {
            Vector3 pos = new Vector3(0, 0, 3);
            Vector3 target = Vector3.Zero;
            Vector3 up = Vector3.Up;

            Matrix.CreateLookAt(ref pos, ref target, ref up, out view);
            Matrix.CreateOrthographicOffCenter(x, x + w, y + h, y, -100, 100, out projection);
        }
        public GraphicsManager Draw(int x, int y, int w, int h, Color? cor = null, bool trainer = false)
        {
            //if (texture != tex && texture != null)
            //    flush();

            //texture = tex;

            //EnsureSpace(6, 4);
            if (trainer) cor = new Color(bias, bias, bias);
            else if (cor == null) cor = Color.White;

            index[indexCount++] = (short)(vertexCount + 0);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 2);
            index[indexCount++] = (short)(vertexCount + 1);
            index[indexCount++] = (short)(vertexCount + 3);
            index[indexCount++] = (short)(vertexCount + 2);

            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 0, 0), (Color)cor, new Vector2(0, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 0, 0), (Color)cor, new Vector2(1, 0));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 1, 0), (Color)cor, new Vector2(0, 1));
            vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 1, 0), (Color)cor, new Vector2(1, 1));

            Matrix world = Matrix.CreateTranslation(new Vector3(-.5f * 0, -.5f * 0, 0))
                //* Matrix.CreateRotationZ(MathHelper.PiOver2 * flipX)
                * Matrix.CreateScale(new Vector3(w, h, 1))
                * Matrix.CreateTranslation(new Vector3(x, y, 0));

            for (int i = vertexCount - 4; i < vertexCount; i++)
                Vector3.Transform(ref vertex[i].Position, ref world, out vertex[i].Position);
            return this;
        }

        public void feedForward(Texture2D input)
        {
            // in -> w -> sum -> out // varios inputs com seus pesos somados em um unico output
            // in -> [ w -> sum ] -> out 
            // in -> [ w -> sum ] -> [ w -> sum ] -> out 
        }

        public GraphicsManager fillCircle(Vector2 pos, float radius)
        {
            Draw(0, 0, size, size);

            RasterizerState rast = new RasterizerState();
            rast.FillMode = FillMode.Solid;
            rast.CullMode = CullMode.None;

            gd.RasterizerState = rast;
            gd.SamplerStates[0] = SamplerState.PointClamp;

            circle.Parameters["WorldViewProjection"].SetValue(view * projection);

            circle.Parameters["desloc"].SetValue(pos);
            circle.Parameters["radius"].SetValue(radius);

            gd.SetRenderTarget(inputs);

            circle.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);

            vertexCount = indexCount = 0;
            gd.SetRenderTarget(null);
            return this;
        }
        public GraphicsManager fillRect(Vector2 pos, Vector2 squareSize)
        {
            Draw(0, 0, size, size);

            RasterizerState rast = new RasterizerState();
            rast.FillMode = FillMode.Solid;
            rast.CullMode = CullMode.None;

            gd.RasterizerState = rast;
            gd.SamplerStates[0] = SamplerState.PointClamp;

            square.Parameters["WorldViewProjection"].SetValue(view * projection);

            square.Parameters["desloc"].SetValue(pos);
            square.Parameters["size"].SetValue(squareSize);

            gd.SetRenderTarget(inputs);

            square.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);

            vertexCount = indexCount = 0;
            gd.SetRenderTarget(null);
            return this;
        }
        public GraphicsManager feed(Texture2D input = null, Texture2D weigths = null, RenderTarget2D output = null, float bias = 0)
        {
            //if (vertexCount == 0) return this;
            this.bias = bias;
            Draw(0, 0, size, size, trainer: true);

            RasterizerState rast = new RasterizerState();
            rast.FillMode = FillMode.Solid;
            rast.CullMode = CullMode.None;

            gd.RasterizerState = rast;
            gd.SamplerStates[0] = SamplerState.PointClamp;

            network.Parameters["WorldViewProjection"].SetValue(view * projection);

            network.Parameters["inputs"].SetValue(input);
            network.Parameters["weigths"].SetValue(weigths);

            gd.SetRenderTarget(output);

            network.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);

            vertexCount = indexCount = 0;
            gd.SetRenderTarget(null);
            // alimenta o histórico 
            return this;
        }
        public GraphicsManager flush(Texture2D tex)
        {
            if (vertexCount == 0) return this;

            gd.SetRenderTarget(null);

            RasterizerState rast = new RasterizerState();
            rast.FillMode = FillMode.Solid;
            rast.CullMode = CullMode.None;
            gd.RasterizerState = rast;

            gd.SamplerStates[0] = SamplerState.PointClamp;

            drawEffect.Parameters["WorldViewProjection"].SetValue(view * projection);
            drawEffect.Parameters["tex"].SetValue(tex);

            drawEffect.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);

            vertexCount = indexCount = 0;            
            //Stream s = File.Create("screen.png");
            //renderScreen.SaveAsPng(s, 960, 540); 
            return this;
        }       
    }
}