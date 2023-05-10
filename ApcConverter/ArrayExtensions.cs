namespace ApcConverter
{
    public static class ArrayExtensions
    {
        public static int Find(this byte[] array, byte[] pattern, int index = 0)
        {
            if (index + pattern.Length > array.Length)
            {
                return -1;
            }

            for (var i = index; i < array.Length - pattern.Length + 1; i++)
            {
                var found = true;
                for (var j = 0; j < pattern.Length; j++)
                {
                    if (array[i + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int Find(this byte[] array, byte element, int index = 0)
        {
            if (index >= array.Length)
            {
                return -1;
            }
            for (var i = index; i < array.Length; i++)
            {
                if (array[i] == element)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindBack(this byte[] array, byte element, int index)
        {
            if (index >= array.Length || index < 0)
            {
                return -1;
            }
            for (var i = index; i > 0; i--)
            {
                if (array[i] == element)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}