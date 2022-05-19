using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Threading;

namespace perceptron
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        GraphicsManager gm;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gm = new GraphicsManager(GraphicsDevice);
            gm.network = Content.Load<Effect>("network");
            gm.drawEffect = Content.Load<Effect>("texture");
            gm.circle = Content.Load<Effect>("circle");
            gm.square = Content.Load<Effect>("square");
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        Random rand = new Random(69);
        protected override void Draw(GameTime gameTime)
        {
            gm.Begin(0, 0, 200, 200);

            if (rand.Next(0, 2) == 1)
                gm.fillCircle(new Vector2((float)rand.NextDouble() - .5f, (float)rand.NextDouble() - .5f), (float)rand.NextDouble() / 2 + .2f);
            else
                gm.fillRect(new Vector2((float)rand.NextDouble() - .5f, (float)rand.NextDouble() - .5f), new Vector2((float)rand.NextDouble() / 2, (float)rand.NextDouble() / 2));

            gm.feed(input: gm.inputs, weigths: gm.weigths, output: gm.output, bias: 0);
            Color[] output = new Color[200 * 200];
            gm.output.GetData(output);

            int soma = 0;
            for (int i = 0; i < output.Length; i++)
                soma += output[i].R;

            GraphicsDevice.Clear(new Color(20, 20, 20));
            gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            //gm.setInput(gm.inputs).setWeigths(gm.weigths).setBias(0f);
            int y = _graphics.PreferredBackBufferHeight / 2 - 100;
            gm.Draw(10, y, 200, 200).flush(gm.inputs);
            gm.Draw(10 + 200 + 10, y, 200, 200).flush(gm.weigths);
            gm.Draw(10 + 200 + 10 + 200 + 10 + 10, y, 200, 200, cor: new Color(soma / (200 * 200), 255, 255)).flush(gm.output);
            //   gm.feed(input: gm.inputs, weigths: gm.weigths, gm.output, 0).Draw(10 + 200 + 10, 10, 200,200).flush(gm.output);
            //gm.Draw(10, 10, 400, 400).flush(gm.inputs, gm.output, Color.Transparent, gm.network);
            Thread.Sleep(1000);

            base.Draw(gameTime);
        }
    }
}
