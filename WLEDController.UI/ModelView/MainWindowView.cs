using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WLEDController.UI.Converters;
using WLEDController.UI.Extensions;
using WLEDController.UI.Models;
using WLEDController.UI.Mvvm;

namespace WLEDController.UI.ModelView
{
    public enum TextConverter
    {
        Binary,
        MorseCode,
        Tap
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
                ITextConverter converter = TextConverter switch
                {
                    TextConverter.Binary => new BinaryTextConverter(),
                    TextConverter.MorseCode => new MorseCodeTextConverter(),
                    TextConverter.Tap => new TapTextConverter(),
                    _ => throw new NotImplementedException()
                };

                IEnumerable<WordMap> wordMaps = converter.GetWordMaps(Text);

                converter.DisplayMethod.ConfigureMappings(wordMaps, converter.Start(), NumberOfLights, Delay);

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    await converter.DisplayMethod.SetLights(client!, cancellationTokenSource.Token);
                }

                client.Dispose();
            });
        }

        private void Stop()
        {
            cancellationTokenSource?.Cancel();
            StartEnabled = true;
        }
    }
}