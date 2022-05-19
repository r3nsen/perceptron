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
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsFixedTimeStep = false;
            //_graphics.PreferredBackBufferFormat = SurfaceFormat.;
            //_graphics.te
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
            gm.renderText = Content.Load<Effect>("textRender");
            gm.sprfont = Content.Load<SpriteFont>("font");
            gm.renderText.Parameters["tex"].SetValue(gm.sprfont.Texture);
        }
        bool started;
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) started = true;
            base.Update(gameTime);
        }
        Random rand = new Random(69);
        int steps, erros, fsteps, ferros;
        int MaxSteps = 5000;
        bool trained = false, _checked = false;
        int result;
        Color color;
        int id;
        float soma = 0;
        int csteps, cerros;
        int[] errosList = new int[500];
        int errorsCount;
        protected override void Draw(GameTime gameTime)
        {
            if (!started) return;
            
            if (steps == MaxSteps)
            {
                rand = new Random(69);
                fsteps++;
                ferros = erros;
                errosList[errorsCount++] = erros;
                if (erros == 0)
                {
                    trained = true;
                    rand = new Random(420);
                }
                steps = erros = 0;
            }
            if (!trained)
            {
                gm.Begin(0, 0, 200, 200);

                // preenche os inputs
                id = rand.Next(0, 2);
                if (id == 1)
                    gm.fillCircle(new Vector2((float)rand.NextDouble() - .5f, (float)rand.NextDouble() - .5f),
                        (float)rand.NextDouble() / 2 + .2f);
                else
                    gm.fillRect(new Vector2((float)rand.NextDouble() - .5f, (float)rand.NextDouble() - .5f),
                        new Vector2((float)rand.NextDouble() / 2, (float)rand.NextDouble() / 2));

                gm.feed(input: gm.inputs, weigths: gm.weigths, output: gm.output, bias: 0);

                Vector4[] output = new Vector4[200 * 200];
                gm.output.GetData(output);

                //                float soma = 0;
                soma = 0;
                for (int i = 0; i < output.Length; i++)
                    soma += output[i].X;

                result = soma > 100 ? 1 : 0;
                //Color color;


            }
            if (result == id) // sucesso
                color = new Color(.5f, 1f, 1f);
            else              // falha
                color = new Color(1f, 1f, .5f);

            GraphicsDevice.Clear(new Color(20, 20, 20));
            gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            int x = _graphics.PreferredBackBufferWidth / 4;
            int y = _graphics.PreferredBackBufferHeight / 2 - 100;
            gm.Draw(x - 110, y, 200, 200).flush(gm.inputs, 1);
            gm.Draw(x * 2 - 100, y, 200, 200).flush(gm.weigths);
            gm.Draw(x * 3 - 100 + 10, y, 200, 200, cor: color).flush(gm.output);

            if (result != id && !trained)
            {
                gm.Begin(0, 0, 200, 200);
                gm.back(input: gm.inputs, weigths: gm.weigths, output: gm.output, bias: 0, sign: (id * 2 - 1));
                ++erros;
            }
            if (!trained) ++steps;

            gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            gm.DrawString("Steps: " + fsteps + " - err: " + erros + " - count: " + steps + "/" + MaxSteps, new Vector2(x - 110, 2 * y + 100)).flushText();
            for(int i = errorsCount - 1; i >= errorsCount - 24 && i >= 0; i--)
            {
                gm.DrawString(""+ (fsteps - (errorsCount - i)) + " - " + errosList[i], new Vector2(10 + ((-i + errorsCount - 1) / 6) * 160, 10  +((-i + errorsCount - 1)%6 +1)*15), .5f).flushText();
            }
              
            if (trained )
            {
                // checking
                if (csteps == MaxSteps)
                {
                    _checked = true;
                }
                if (!_checked)
                {
                    gm.Begin(0, 0, 200, 200);

                    // preenche os inputs
                    id = rand.Next(0, 2);
                    if (id == 1)
                        gm.fillCircle(new Vector2((float)rand.NextDouble() - .5f, (float)rand.NextDouble() - .5f),
                            (float)rand.NextDouble() / 2 + .2f);
                    else
                        gm.fillRect(new Vector2((float)rand.NextDouble() - .5f, (float)rand.NextDouble() - .5f),
                            new Vector2((float)rand.NextDouble() / 2, (float)rand.NextDouble() / 2));

                    gm.feed(input: gm.inputs, weigths: gm.weigths, output: gm.output, bias: 0);

                    Vector4[] output = new Vector4[200 * 200];
                    gm.output.GetData(output);

                    //                float soma = 0;
                    soma = 0;
                    for (int i = 0; i < output.Length; i++)
                        soma += output[i].X;

                    result = soma > 100 ? 1 : 0;
                    if (result != id) cerros++; // falha
                }

                gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                gm.DrawString("err: " + cerros + " - count: " + csteps + "/" + MaxSteps, new Vector2(x - 110, 2 * y + 140)).flushText();
                if(!_checked)++csteps;
            }
            //Thread.Sleep(50);
            base.Draw(gameTime);
        }
    }
}
