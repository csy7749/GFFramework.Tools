namespace FuguFirecracker.TakeNote
{
  // what's a string way ?
  // dotNet 4 doesn't have this implementation. 
    public static class Stringway
    {
        public static bool IsNullOrWhiteSpace(string s)
        {
            if (s == null) return true;
            return s.Trim() == string.Empty;
        }
    }
}