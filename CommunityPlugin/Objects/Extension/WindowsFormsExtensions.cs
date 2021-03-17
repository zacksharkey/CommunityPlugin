using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CommunityPlugin.Objects.Extension
{
    public static class WindowsFormsExtensions
    {
        public static IEnumerable<T> AllControls<T>(this Control startingPoint) where T : Control
        {
            bool hit = startingPoint is T;
            if (hit)
                yield return startingPoint as T;
            foreach (Control control in startingPoint.Controls.Cast<Control>())
            {
                Control child = control;
                foreach (T allControl in child.AllControls<T>())
                {
                    T item = allControl;
                    yield return item;
                    item = default(T);
                }
                child = (Control)null;
            }
        }
        public static void CheckAll(this CheckedListBox checkedListBox, bool check)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
                checkedListBox.SetItemChecked(i, check);
        }
    }
}
