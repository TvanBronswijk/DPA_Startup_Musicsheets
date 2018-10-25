using DPA_Musicsheets.Converters.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Factory
{
    interface IConverterFactory
    {
        IMusicConverterStrategy CreateConverter(String name);
    }
}
