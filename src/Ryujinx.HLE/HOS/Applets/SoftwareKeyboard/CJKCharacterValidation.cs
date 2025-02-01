using Ryujinx.Common.Helper;

namespace Ryujinx.HLE.HOS.Applets.SoftwareKeyboard
{
    public static class CJKCharacterValidation
    {
        public static bool IsCJK(char value) => Patterns.CJK.IsMatch(value.ToString());
    }
}
