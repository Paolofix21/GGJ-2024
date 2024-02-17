using UnityEngine.EventSystems;

namespace Code.UI {
    public abstract class SettingsComponentUI<T> : UIBehaviour where T : unmanaged {
        #region Public Methods
        public abstract void Register(System.Action<T> onValueChanged);
        #endregion
    }
}