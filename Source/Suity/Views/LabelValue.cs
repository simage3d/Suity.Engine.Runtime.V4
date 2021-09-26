using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Views
{
    [Serializable]
    public class LabelValue : IViewValue
    {
        public static readonly LabelValue Empty = new LabelValue();

        private LabelValue()
        {
        }
    }
}
