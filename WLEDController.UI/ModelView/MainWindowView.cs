using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WLEDController.UI.Converters;
using WLEDController.UI.Mvvm;

namespace WLEDController.UI.ModelView
{
    public enum TextConverter
    {
        Binary,
        MorseCode
    }

    internal sealed class MainWindowView : INotifyPropertyChanged
    {
        private CancellationTokenSource? cancellationTokenSource;
        private WLEDClient? client = null;
        private int delay = 100;
        private int numberOfLights = 149;
        private bool startEnabled = true;
        private string text = string.Empty;
        private TextConverter textConverter = TextConverter.Binary;
        private string url = "192.168.2.51";

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Delay
        {
            get => delay;

            set
            {
                delay = value;
                OnValueChanged();
            }
        }

        public int NumberOfLights
        {
            get => numberOfLights;

            set
            {
                numberOfLights = value;
                OnValueChanged();
            }
        }

        public ICommand OnStart => new DelegateCommand(Start);

        public ICommand OnStop => new DelegateCommand(Stop);

        public bool StartEnabled
        {
            get => startEnabled;

            set
            {
                startEnabled = value;
                OnValueChanged();
            }
        }

        public string Text
        {
            get => text;

            set
            {
                text = value;
                OnValueChanged();
            }
        }

        public TextConverter TextConverter
        {
            get => textConverter;

            set
            {
                textConverter = value;
                OnValueChanged();
            }
        }

        public string Url
        {
            get => url;

            set
            {
                url = value;
                OnValueChanged();
            }
        }

        private void OnValueChanged([CallerMemberName] string? property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Start()
        {
            client = new();
            cancellationTokenSource = new();
            StartEnabled = false;

            if (IPAddress.TryParse(Url, out IPAddress? ipAddress))
            {
                client.Connect(Format.DRGBW, ipAddress);
            }
            else
            {
                client.Connect(Format.DRGBW, Url);
            }

            _ = Task.Run(async () =>
            {
                LED[] leds = new LED[NumberOfLights];

                for (int i = 0; i < leds.Length; i++)
                {
                    leds[i] = new(i, Color.Black);
                }

                string[] words = Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                List<WordMap> wordMaps = [new WordMap(((char)255).ToString(), Color.Teal, new BinaryTextConverter())];
                Random random = new();

                ITextConverter converter = TextConverter switch
                {
                    TextConverter.Binary => new BinaryTextConverter(),
                    TextConverter.MorseCode => new MorseCodeTextConverter(),
                    _ => throw new NotImplementedException()
                };

                foreach (string word in words)
                {
                    Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                    wordMaps.Add(new(word, color, converter));

                    wordMaps.Add(new(" ", Color.Black, converter));
                }

                LightColorMap[] binaryColorMaps = wordMaps.SelectMany(x => x.GetLightColorMaps()).ToArray();

                long loopCount = binaryColorMaps.Length + NumberOfLights;

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    for (int offset = 0; offset < loopCount; offset++)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        int binaryColorIndex = 0;

                        for (int i = offset; i >= 0; i--)
                        {
                            try
                            {
                                if (binaryColorIndex >= binaryColorMaps.Length || cancellationTokenSource.IsCancellationRequested)
                                {
                                    break;
                                }

                                if (i >= leds.Length)
                                {
                                    binaryColorIndex++;
                                    continue;
                                }

                                leds[i].Red = binaryColorMaps[binaryColorIndex].OnValue ? binaryColorMaps[binaryColorIndex].Color.R : (byte)0;
                                leds[i].Green = binaryColorMaps[binaryColorIndex].OnValue ? binaryColorMaps[binaryColorIndex].Color.G : (byte)0;
                                leds[i].Blue = binaryColorMaps[binaryColorIndex].OnValue ? binaryColorMaps[binaryColorIndex].Color.B : (byte)0;

                                binaryColorIndex++;
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }

                        _ = client.Send(leds);
                        await Task.Delay(Delay);
                    }
                }

                client.Dispose();
            });
        }

        private void Stop()
        {
            cancellationTokenSource?.Cancel();
            StartEnabled = true;
        }

        private readonly struct LightColorMap
        {
            public LightColorMap(Color color, bool onValue)
            {
                Color = color;
                OnValue = onValue;
            }

            public Color Color { get; }

            public bool OnValue { get; }
        }

        private class WordMap
        {
            public WordMap(string word, Color color, ITextConverter textConverter)
            {
                Word = word;
                Color = color;

                LightValue = textConverter.ConvertText(word);
            }

            public Color Color { get; }

            public bool[] LightValue { get; }

            public string Word { get; }

            public IEnumerable<LightColorMap> GetLightColorMaps()
            {
                foreach (bool b in LightValue)
                {
                    yield return new(Color, b);
                }
            }
        }
    }
}