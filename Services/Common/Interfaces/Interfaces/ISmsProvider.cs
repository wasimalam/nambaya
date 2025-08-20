using System;
using System.Collections.Generic;

namespace Common.Interfaces.Interfaces
{
    public interface ISmsProvider
    {
        string Send(string cellNo, string sender, string messageBody);
        string Send(List<String> cellNo, string sender, string messageBody);
    }
}
