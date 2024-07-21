using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using WLEDController.UI.Mvvm;

namespace WLEDController.UI.Model
{
    internal sealed class MainWindowView : INotifyPropertyChanged
    {
        private CancellationTokenSource? cancellationTokenSource;
        private WLEDClient? client = null;
        private int delay = 200;
        private int numberOfLights = 149;
        private bool startEnabled = true;
        private string text = string.Empty;
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

            Task.Run(async () =>
            {
                LED[] leds = new LED[NumberOfLights];

                for (int i = 0; i < leds.Length; i++)
                {
                    leds[i] = new(i, Color.Black);
                }

                string[] words = Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                List<LetterMap> letterMaps = [new LetterMap('\0', 255, Color.Teal)];
                Random random = new();

                foreach (string word in words)
                {
                    Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                    byte[] bytes = Encoding.UTF8.GetBytes(word);
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        letterMaps.Add(new(word[i], bytes[i], color));
                    }

                    letterMaps.Add(new(' ', 0, Color.Black));
                }

                BinaryColorMap[] binaryColorMaps = letterMaps.SelectMany(x => x.GetBinaryColorMaps()).ToArray();

                long loopCount = (binaryColorMaps.Length) + NumberOfLights;

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    for (int offset = 0; offset < loopCount; offset++)
                    {
                        if(cancellationTokenSource.IsCancellationRequested)
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

                                leds[i].Red = binaryColorMaps[binaryColorIndex].BinaryValue ? binaryColorMaps[binaryColorIndex].Color.R : (byte)0;
                                leds[i].Green = binaryColorMaps[binaryColorIndex].BinaryValue ? binaryColorMaps[binaryColorIndex].Color.G : (byte)0;
                                leds[i].Blue = binaryColorMaps[binaryColorIndex].BinaryValue ? binaryColorMaps[binaryColorIndex].Color.B : (byte)0;

                                binaryColorIndex++;

                            }
                            catch (Exception)
                            {
                                throw;
                            }                        }

                        client.Send(leds);
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

        private class LetterMap
        {
            public LetterMap(char letter, byte byteValue, Color color)
            {
                Letter = letter;
                ByteValue = byteValue;
                Color = color;

                string binaryString = Convert.ToString(byteValue, 2).PadLeft(8, '0');
                BinaryValue = new bool[8];

                for (int i = 0; i < 8; i++)
                {
                    BinaryValue[i] = binaryString[i] == '1';
                }
            }

            public Color Color { get; }

            public char Letter { get; }

            public bool[] BinaryValue { get; }
            public byte ByteValue { get; }

            public IEnumerable<BinaryColorMap> GetBinaryColorMaps()
            {
                foreach (bool b in BinaryValue)
                {
                    yield return new(Color, b);
                }
            }
        }

        private struct BinaryColorMap
        {
            public BinaryColorMap(Color color, bool binaryValue)
            {
                Color = color;
                BinaryValue = binaryValue;
            }

            public Color Color { get; }
            public bool BinaryValue { get; }
        }
    }
}