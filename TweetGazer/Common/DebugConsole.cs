using System.Collections.ObjectModel;
using System.Windows.Data;

namespace TweetGazer.Common
{
    public static class DebugConsole
    {
        public static void Write(object data)
        {
            Data.Add(data.ToString());
            System.Diagnostics.Debug.Write(data);
        }

        public static void WriteLine(object data)
        {
            Data.Add(data.ToString());
            System.Diagnostics.Debug.WriteLine(data);
        }

        public static ObservableCollection<string> Data
        {
            get
            {
                if (_Data == null)
                {
                    _Data = new ObservableCollection<string>();
                    BindingOperations.EnableCollectionSynchronization(_Data, new object());
                }
                return _Data;
            }
            set
            {
                _Data = value;
            }
        }
        private static ObservableCollection<string> _Data;
    }
}
