using Ryujinx.Common.Helper;

namespace Ryujinx.HLE.HOS.Applets.SoftwareKeyboard
{
    public static class NumericCharacterValidation
    {
        public static bool IsNumeric(char value) => Patterns.Numeric.IsMatch(value.ToString());
    }
}
