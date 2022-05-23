using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
//using System.Runtime.Serialization.Formatters.Binary;
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
            _graphics.SynchronizeWithVerticalRetrace = false;
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
            showfps = shownetwork = showstatus = showerrhistoric = false;
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) started = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Q)) showfps = true;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) shownetwork = true;
            if (Keyboard.GetState().IsKeyDown(Keys.E)) showstatus = true;
            if (Keyboard.GetState().IsKeyDown(Keys.R)) showerrhistoric = true;
            if (Keyboard.GetState().IsKeyDown(Keys.T)) showfps = shownetwork = showstatus = showerrhistoric = true;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                savedata();
                loaddata();
             //   while (Keyboard.GetState().IsKeyDown(Keys.S)) 
                    Thread.Sleep(500);
            }
            base.Update(gameTime);
        }
        void savedata()
        {
            data d = new data();
            d.errors = errosList;
            d.errorsCount = errorsCount;
            d.weigths = new (float x, float y, float z, float w)[200 * 200];
            Vector4[] temp = new Vector4[200 * 200];
            gm.weigths.GetData(temp);
            for (int i = 0; i < 200 * 200; i++)
            {
                d.weigths[i].x = temp[i].X;
                d.weigths[i].y = temp[i].Y;
                d.weigths[i].z = temp[i].Z;
                d.weigths[i].w = temp[i].W;
            }
            BinaryFormatter bin = new BinaryFormatter();
            Stream s = File.Create("data.bin");
            bin.Serialize(s, d);
            s.Close();
        }
        void loaddata()
        {
            if (File.Exists("data.bin"))
            {
                BinaryFormatter bin = new BinaryFormatter();
                Stream s = File.Open("data.bin", FileMode.Open);
                data d = (data)bin.Deserialize(s);
                s.Close();
                Vector4[] temp = new Vector4[200 * 200];                

                for (int i = 0; i < 200 * 200; i++)
                {
                    temp[i].X = d.weigths[i].x;
                    temp[i].Y = d.weigths[i].y;
                    temp[i].Z = d.weigths[i].z;
                    temp[i].W = d.weigths[i].w;
                }
                gm.weigths.SetData(temp);
                errosList = d.errors;
                errorsCount = d.errorsCount;
            }
        }
        [Serializable]
        struct data
        {
            public int[] errors;
            public int errorsCount;
            public (float x, float y, float z, float w)[] weigths;

        }
        void restoreData()
        {
            string pesos = File.ReadAllText(@"C:\Users\Renato\Desktop\perceptron dump\data array dump.txt");
            string errosLog = File.ReadAllText(@"C:\Users\Renato\Desktop\perceptron dump\errosList array dump.txt");
            ;
            char c;
            while (false)
            {
                // 20 - 114 - 148 - 150
                c = pesos[151];
            }
            string pesosCleaned;
            char[] pesosArray = new char[pesos.Length];
            int pesosArrayCount = 0;
            for (int i = 0; i < pesos.Length; ++i)
            {
                // 20 - 115 - 148 - 150
                int mod = i % 151;
                c = pesos[i];
                if (mod == 112)
                    ;
                if (mod >= 20 && mod <= 115)
                    pesosArray[pesosArrayCount++] = c;    //pesosCleaned += c;
            }
            pesosCleaned = new string(pesosArray, 0, pesosArrayCount);
            string errosCleaned = "";
            char[] errosArray = new char[pesos.Length];
            int errosArrayCount = 0;
            for (int i = 0; i < errosLog.Length; ++i)
            {
                // 20 - 31 - x148 - 38
                int mod = i % 39;
                c = errosLog[i];

                if (mod >= 20 && mod <= 31)
                    errosArray[errosArrayCount++] = c;
            }
            errosCleaned = new string(errosArray, 0, errosArrayCount);
            ;
            int parseHex(ReadOnlySpan<char> input)
            {
                return int.Parse(input, System.Globalization.NumberStyles.HexNumber);
            }
            int getDword(ref int index, char[] arr)
            {
                int num = 0;
                num |= parseHex(arr[index..(index += 3)]) << 8 * 0;
                num |= parseHex(arr[index..(index += 3)]) << 8 * 1;
                num |= parseHex(arr[index..(index += 3)]) << 8 * 2;
                num |= parseHex(arr[index..(index += 3)]) << 8 * 3;
                return num;
            }
        reset:
            int _byteLen = 3;
            int j = 4 * (4 - 2) * _byteLen;
            Vector4[] restoredArray = new Vector4[getDword(ref j, pesosArray)];
            int racount = 0;
            for (j = 4 * 4 * _byteLen; j < pesosCleaned.Length; /*j++*/)
            {
                // goto reset;
                // 3 chars, 8x 
                c = pesosArray[j];
                int num = getDword(ref j, pesosArray);
                float x = Unsafe.As<int, float>(ref num);
                num = getDword(ref j, pesosArray);
                float y = Unsafe.As<int, float>(ref num);
                num = getDword(ref j, pesosArray);
                float z = Unsafe.As<int, float>(ref num);
                num = getDword(ref j, pesosArray);
                float w = Unsafe.As<int, float>(ref num);
                restoredArray[racount++] = new Vector4(x, y, z, w);


                //int.Parse(pesosArray[i..(i + 3)], System.Globalization.NumberStyles.HexNumber);
                //goto reset;
            }
            ;
            j = 4 * (4 - 2) * _byteLen;
            int[] restorederrArray = new int[getDword(ref j, errosArray)];
            int eacount = 0;
            for (j = 4 * 4 * _byteLen; j < errosCleaned.Length; /*j++*/)
            {
                // goto reset;
                // 3 chars, 8x 
                c = errosArray[j];
                int num = getDword(ref j, errosArray);
                restorederrArray[eacount++] = num;


                //int.Parse(pesosArray[i..(i + 3)], System.Globalization.NumberStyles.HexNumber);
                //goto reset;
            }
            ;
            gm.weigths.SetData(restoredArray);
            errosList = restorederrArray;
            errorsCount = steps = 500;
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
        Vector4[] output = new Vector4[200 * 200];
        int count = 0;
        int fr = 0;
        bool showfps = false, shownetwork = false, showstatus = false, showerrhistoric = false;
        protected override void Draw(GameTime gameTime)
        {

            //int fr = (int)(1 / (gameTime.ElapsedGameTime.Milliseconds / 1000f));
            int x = _graphics.PreferredBackBufferWidth / 4;
            int y = _graphics.PreferredBackBufferHeight / 2 - 100;

            GraphicsDevice.Clear(new Color(20, 20, 20));
            if (count > 500)
            {
                fr = (int)(1 / (gameTime.ElapsedGameTime.Milliseconds / 1000f));
                count -= 500;
            }
            count += gameTime.ElapsedGameTime.Milliseconds;
            if (showfps)
            {
                gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                gm.DrawString(" fps: " + fr, new Vector2(x - 110, 2 * y + 60)).flushText();
                count += gameTime.ElapsedGameTime.Milliseconds * 2;

            }
            if (!started) return;

            //
            // restoreData();
            if (steps == MaxSteps)
            {
                rand = new Random(69);
                fsteps++;
                ferros = erros;
                errosList[errorsCount++] = erros;
                if (errorsCount == errosList.Length)
                    Array.Resize(ref errosList, errosList.Length + 10);
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
                //  return;

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

            if (shownetwork)
            {
                gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                gm.Draw(x - 110, y, 200, 200).flush(gm.inputs, 1);
                gm.Draw(x * 2 - 100, y, 200, 200).flush(gm.weigths);
                gm.Draw(x * 3 - 100 + 10, y, 200, 200, cor: color).flush(gm.output);
            }

            if (result != id && !trained)
            {
                gm.Begin(0, 0, 200, 200);

                gm.back(input: gm.inputs, weigths: gm.weigths, output: gm.output, bias: 0, sign: (id * 2 - 1));
                ++erros;
            }

            if (!trained) ++steps;

            //int fr = (int)(1 / (gameTime.ElapsedGameTime.Milliseconds/1000f));
            if (showstatus)
            {
                gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                gm.DrawString("Steps: " + fsteps + " - err: " + erros + " - count: " + steps + "/" + MaxSteps, new Vector2(x - 110, 2 * y + 100)).flushText();
            }
            if (showerrhistoric)
            {
                gm.Begin(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
                for (int i = errorsCount - 1; i >= errorsCount - 24 && i >= 0; i--)
                {
                    gm.DrawString("" + (fsteps - (errorsCount - i)) + " - " + errosList[i], new Vector2(10 + ((-i + errorsCount - 1) / 6) * 160, 10 + ((-i + errorsCount - 1) % 6 + 1) * 15), .5f).flushText();
                }
            }


            //for (int i = errorsCount - 1; i >= errorsCount - 24 && i >= 0; i--)
            //{
            //    gm.DrawString("" + (fsteps - (errorsCount - i)) + " - " + errosList[i], new Vector2(10 + ((-i + errorsCount - 1) / 6) * 160, 10 + ((-i + errorsCount - 1) % 6 + 1) * 15), .5f).flushText();
            //}

            if (trained)
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

                    //Vector4[] output = new Vector4[200 * 200];
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
                if (!_checked) ++csteps;
            }
            //Thread.Sleep(50);

            base.Draw(gameTime);
        }
    }
}
