using System.Collections;
using System.Drawing;

namespace Game.Utils {
    public class BitmapCache {
        public static readonly Hashtable Cache = new Hashtable();

        public static void AddToCache(string key, Bitmap obj) {
            if (Cache.ContainsKey(key))
                Debug.LogWarning($"BitmapCache already contains {key}. Overwriting.");
            Cache[key] = obj;
        }

        public static Bitmap GetBitmap(string key) {
            if (Cache.ContainsKey(key))
                return Cache[key] as Bitmap;
            Debug.LogWarning($"BitmapCache does not contain {key}. Returning `null`");
            return null;
        }

        public static void ClearCache() => Cache.Clear();
    }
}