using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdom
{
    /// <summary>
    /// A real time fluid simulation of lava.
    /// The result of the simulation, at each fream of the game, is saved into a one dimensional texture height map, to be used in drawing the lava.
    /// </summary>
    public class LavaDraw
    {
        const int WaveN = 256;
        Texture2D WaveTexture;
        const int Int = 3;
        Color[] WaveHeightColor = new Color[Int * (WaveN - 1)];
        float[] WaveHeight = new float[WaveN];
        float[] WaveHeightv = new float[WaveN];
        float[] OldWaveHeight = new float[WaveN];
        float[] OldWaveHeightv = new float[WaveN];

        GraphicsDevice Device;
        public LavaDraw(GraphicsDevice Device)
        {
            this.Device = Device;

            WaveTexture = new Texture2D(Device, Int * (WaveN - 1), 1, false, SurfaceFormat.Color);
        }

        public float Height(float u)
        {
            int i = (int)(WaveN * u);
            return OldWaveHeight[i];
        }

        public bool DoLavaUpdate = false;
        public void Update()
        {
            if (!DoLavaUpdate && Initialized)
                return;
            else
                DoLavaUpdate = false;

            int step = 0;
            if (Tools.CurLevel != null)
                step = Tools.CurLevel.CurPhsxStep;

            if (Initialized)
                PhsxStep();
            else
            {
                for (int i = 0; i < 15; i++)
                    PhsxStep();

                Initialized = true;
            }

            for (int i = 0; i < WaveN - 1; i++)
                for (int j = 0; j < Int; j++)
                {
                    float val = (.85f * WaveHeight[i] + .056f) * (1f - j / (float)Int) +
                                (.85f * WaveHeight[i + 1] + .056f) * j / (float)Int;

                    WaveHeightColor[Int * i + j] = new Color(Tools.EncodeFloatRGBA(val));
                }

            var e = Tools.EffectWad.FindByName("Lava");

            e.effect.Parameters["xHeight"].SetValue((Texture)null);
            Device.Textures[0] = null;
            WaveTexture.SetData(WaveHeightColor);
            e.t.SetValue((float)step);
            e.effect.Parameters["xHeight"].SetValue(WaveTexture);

            Device.Textures[1] = WaveTexture;
        }

        bool Initialized;
        void PhsxStep()
        {
            int step = 0;
            if (Tools.CurLevel != null)
                step = Tools.CurLevel.CurPhsxStep;

            for (int q = 0; q < 1; q++)
            {
                for (int i = 0; i < WaveN; i++)
                {
                    OldWaveHeight[i] = WaveHeight[i];
                    OldWaveHeightv[i] = WaveHeightv[i];
                }

                float h = .5f * 1.05f * 1f / WaveN;
                float dt = .05f;

                for (int i = 0; i < WaveN; i++)
                {
                    float z = (OldWaveHeightv[(i + 1) % WaveN] + OldWaveHeightv[(i - 1 + WaveN) % WaveN] - 2 * OldWaveHeightv[i]) / (h);
                    float w = (OldWaveHeight[(i + 1) % WaveN] + OldWaveHeight[(i - 1 + WaveN) % WaveN] - 2 * OldWaveHeight[i]) / (h);
                    WaveHeightv[i] += dt * (
                        .12f * ((float)Math.Abs(OldWaveHeight[i] * 15 + .033f) + .006f) * w
                        + .00075f * (float)Math.Sin((2 * i * h + (float)step * .3) * 3.14159f * 6)
                        - .0046f * (.04f - Math.Abs(z)) * Math.Sign(z)
                        - .0046f * (.02f - Math.Abs(w)) * Math.Sign(w)
                        - .06f * OldWaveHeightv[i]
                        - 38 * OldWaveHeight[i] * OldWaveHeight[i] * OldWaveHeight[i]
                        - 2 * Math.Abs(OldWaveHeight[i] * OldWaveHeightv[i]) * Math.Sign(OldWaveHeightv[(i + WaveN / 2) % WaveN])
                        - 4 * Math.Abs(OldWaveHeight[i] * OldWaveHeightv[i]) * Math.Sign(OldWaveHeightv[(i + (int)(WaveN * 1.3f)) % WaveN])
                        + .0043f * (.35f - Math.Abs(OldWaveHeight[i])) * Math.Sign(OldWaveHeight[i])
                                     );

                    if (Math.Abs(WaveHeightv[i]) > .047f)
                        WaveHeightv[i] = .047f * Math.Sign(WaveHeightv[i]);
                }

                for (int i = 0; i < WaveN; i++)
                {
                    WaveHeight[i] += dt * 3.6f * WaveHeightv[i];

                    if (Math.Abs(WaveHeight[i]) > .1125f)
                    {
                        WaveHeight[i] = .1125f * Math.Sign(WaveHeight[i]);
                    }
                }
            }
        }
    }
}
