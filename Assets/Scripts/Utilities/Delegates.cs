namespace Utilities {
    #region Delegates
    public delegate void ValueSetEventHandler<in T>(T value);
    public delegate void DestroyEventHandler<in T>(T value);
    #endregion
}