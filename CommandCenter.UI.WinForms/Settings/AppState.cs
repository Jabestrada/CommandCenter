using CommandCenter.Infrastructure.Helpers;
using System.Collections.Generic;

namespace CommandCenter.UI.WinForms.Settings {
    public class AppState {
        public MRUList<string> MRUConfigList { get; set; }
        public AppState() {
            MRUConfigList = new MRUList<string>(7);
        }
    }
}
