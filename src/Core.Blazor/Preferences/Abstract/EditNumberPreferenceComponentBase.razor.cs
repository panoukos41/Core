using System.Numerics;

namespace Core.Preferences.Abstract;

public abstract partial class EditNumberPreferenceComponentBase<TNumber> where TNumber : notnull, INumber<TNumber>, new()
{
}
