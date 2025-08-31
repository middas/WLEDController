using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLEDController.UI.Models;

namespace WLEDController.UI.Converters
{
    internal interface IDisplayMethod
    {
        Task SetLights(WLEDClient client, CancellationToken cancellationToken);

        void ConfigureMappings(IEnumerable<WordMap> wordMaps, BitArray start, int numberOfLights, int delay);
    }
}
