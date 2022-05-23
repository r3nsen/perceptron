using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.IO;

namespace perceptron
{
    class GraphicsManager
    {
        GraphicsDevice gd;

        public Effect network, drawEffect, circle, square, renderText;

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
            inputs = new RenderTarget2D(gd, size, size, false, SurfaceFormat.Vector4, DepthFormat.Depth16);
            weigths = new Texture2D(gd, size, size, false, SurfaceFormat.Vector4);

            rand = new Random(69);

            Vector4[] randColors = new Vector4[size * size];

            for (int i = 0; i < size * size; i++)
            {
                //int r = 255;//rand.Next(0, 255); 
                randColors[i] = new Vector4(0, 0, 0, 1f);
            }
            weigths.SetData(randColors);
            weigths.GetData(randColors);
            output = new RenderTarget2D(gd, size, size, false, SurfaceFormat.Vector4, DepthFormat.Depth16);
            gdInit();
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

        void gdInit()
        {
            RasterizerState rast = new RasterizerState();
            rast.FillMode = FillMode.Solid;
            rast.CullMode = CullMode.None;

            gd.RasterizerState = rast;
            gd.BlendState = BlendState.NonPremultiplied;
            gd.SamplerStates[0] = SamplerState.PointClamp;
        }

        public GraphicsManager fillCircle(Vector2 pos, float radius)
        {
            Draw(0, 0, size, size);

            //RasterizerState rast = new RasterizerState();
            //rast.FillMode = FillMode.Solid;
            //rast.CullMode = CullMode.None;

            //gd.RasterizerState = rast;
            //gd.SamplerStates[0] = SamplerState.PointClamp;

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

            //RasterizerState rast = new RasterizerState();
            //rast.FillMode = FillMode.Solid;
            //rast.CullMode = CullMode.None;

            //gd.RasterizerState = rast;
            //gd.SamplerStates[0] = SamplerState.PointClamp;

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
            var temp = gd.GetRenderTargets();
            gd.SetRenderTarget(output);

            network.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);

            vertexCount = indexCount = 0;
            gd.SetRenderTargets(temp);
            // alimenta o histórico 
            return this;
        }
        Vector4[] data = new Vector4[size * size];
        public GraphicsManager back(Texture2D input = null, Texture2D weigths = null, RenderTarget2D output = null, float bias = 0, float sign = 1)
        {
            this.bias = bias;
            Draw(0, 0, size, size, trainer: true);

            network.Parameters["WorldViewProjection"].SetValue(view * projection);

            network.Parameters["inputs"].SetValue(input);
            network.Parameters["weigths"].SetValue(weigths);
            network.Parameters["sign"].SetValue(sign);

            gd.SetRenderTarget(output);

            network.CurrentTechnique.Passes[1].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);

            vertexCount = indexCount = 0;
            gd.SetRenderTarget(null);

            output.GetData(data);
            weigths.SetData(data);

            return this;
        }
        (Vector4 min, Vector4 max) GetMinMax(Texture2D t)
        {
            Vector4 min = Vector4.One * float.MaxValue, max = Vector4.One * float.MinValue;
            t.GetData(data);
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                min.X = data[i].X < min.X ? data[i].X : min.X;
                min.Y = Math.Min(min.Y, data[i].Y);
                min.Z = Math.Min(min.Z, data[i].Z);
                min.W = Math.Min(min.W, data[i].W);

                max.X = Math.Max(max.X, data[i].X);
                max.Y = Math.Max(max.Y, data[i].Y);
                max.Z = Math.Max(max.Z, data[i].Z);
                max.W = Math.Max(max.W, data[i].W);
            }
            return (min, max);
        }
        public GraphicsManager flush(Texture2D tex, int pass = 0)
        {
            if (vertexCount == 0) return this;

            gd.SetRenderTarget(null);

            drawEffect.Parameters["WorldViewProjection"].SetValue(view * projection);
            (Vector4 min, Vector4 max) = GetMinMax(tex);
            drawEffect.Parameters["tex"].SetValue(tex);
            drawEffect.Parameters["vmin"].SetValue(min);
            drawEffect.Parameters["vmax"].SetValue(max);

            drawEffect.CurrentTechnique.Passes[pass].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);

            vertexCount = indexCount = 0;
            return this;
        }
        public GraphicsManager flushText()
        {
            if (vertexCount == 0) return this;

            renderText.Parameters["WorldViewProjection"].SetValue(view * projection);

            renderText.CurrentTechnique.Passes[0].Apply();
            gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertex, 0, vertexCount, index, 0, indexCount / 3);
            vertexCount = indexCount = 0;

            return this;
        }
        public SpriteFont sprfont;
        public GraphicsManager DrawString(string text, Vector2 pos, float _size = 1, bool rtl = false)
        {
            Vector2 offset = Vector2.Zero;
            bool firstGlyph = true;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\r') continue;
                if (c == '\n')
                {
                    offset.X = 0;
                    offset.Y += sprfont.LineSpacing;
                    firstGlyph = true;
                }
                var glyph = sprfont.GetGlyphs()[c];

                if (firstGlyph)
                {
                    offset.X = Math.Max(glyph.LeftSideBearing, 0);
                    firstGlyph = false;
                }
                else offset.X += sprfont.Spacing + glyph.LeftSideBearing;

                Vector2 currentOff = offset;
                currentOff.X += glyph.Cropping.X;
                currentOff.Y += glyph.Cropping.Y;

                Vector2 size = Vector2.One * _size;

                Matrix transform = Matrix.Identity;

                transform.M11 = size.X;
                transform.M22 = size.Y;
                transform.M41 = pos.X * 0;
                transform.M42 = pos.Y * 0;

                offset.X += glyph.Width + glyph.RightSideBearing;

                EnsureSpace(6, 4);



                float _x = glyph.BoundsInTexture.X,
                      _y = glyph.BoundsInTexture.Y,
                      _w = glyph.BoundsInTexture.Width,
                      _h = glyph.BoundsInTexture.Height;

                _w += _x;
                _h += _y;

                _x /= sprfont.Texture.Width;
                _y /= sprfont.Texture.Height;
                _w /= sprfont.Texture.Width;
                _h /= sprfont.Texture.Height;

                float f = 1111f;

                Color cor = Color.White;

                index[indexCount++] = (short)(vertexCount + 0);
                index[indexCount++] = (short)(vertexCount + 1);
                index[indexCount++] = (short)(vertexCount + 2);
                index[indexCount++] = (short)(vertexCount + 1);
                index[indexCount++] = (short)(vertexCount + 3);
                index[indexCount++] = (short)(vertexCount + 2);

                vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 0, 0), cor, new Vector2(_x, _y));
                vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 0, 0), cor, new Vector2(_w, _y));
                vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(0, 1, 0), cor, new Vector2(_x, _h));
                vertex[vertexCount++] = new VertexPositionColorTexture(new Vector3(1, 1, 0), cor, new Vector2(_w, _h));

                Matrix world = Matrix.CreateTranslation(new Vector3(-.5f, -.5f, 0) * 0)                
                    * Matrix.CreateScale(new Vector3(glyph.BoundsInTexture.Width * size.X, glyph.BoundsInTexture.Height * size.Y, 1))
                    * Matrix.CreateTranslation(new Vector3(pos + currentOff, 0));

                for (int j = vertexCount - 4; j < vertexCount; j++)
                    Vector3.Transform(ref vertex[j].Position, ref world, out vertex[j].Position);
            }
            return this;
        }
        private void EnsureSpace(int indexSpace, int vertexSpace)
        {
            if (indexCount + indexSpace >= index.Length)
                Array.Resize(ref index, Math.Max(indexCount + indexSpace, index.Length * 2));
            if (vertexCount + vertexSpace >= vertex.Length)
                Array.Resize(ref vertex, Math.Max(vertexCount + vertexSpace, vertex.Length * 2));
        }
    }
}