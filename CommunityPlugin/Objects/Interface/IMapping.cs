using CommunityPlugin.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityPlugin.Objects.Interface
{
    public interface IMapping
    {
        string ColumnName { get; set; }
        string Description { get; set; }
        string Translation { get; set; }
        TranslationType TranslationType { get; set; }
        Enums.ValueType ValueType { get; set; }
        IDictionary<string, string> Properties { get; set; }
        string GetProperty(string name);
    }
}
