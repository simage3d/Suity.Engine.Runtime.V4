using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Suity.Views
{
    [Serializable]
    public sealed class ButtonValue : IViewValue
    {
        public static readonly ButtonValue Empty = new ButtonValue(false);
        public static readonly ButtonValue Clicked = new ButtonValue(true);

        private ButtonValue(bool isClicked)
        {
            IsClicked = isClicked;
        }

        public bool IsClicked { get; }

        public override string ToString()
        {
            return IsClicked.ToString();
        }

        public static bool TryParse(string s, out ButtonValue value)
        {
            if (bool.TryParse(s, out bool b))
            {
                value = b ? Clicked : Empty;
                return true;
            }
            else
            {
                value = ButtonValue.Empty;
                return false;
            }
        }
    }
}
