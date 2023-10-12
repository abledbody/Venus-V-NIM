using System;

namespace Venus {
	public static partial class Extensions {
		public static int Int<T>(this T value) where T : Enum
			=> Convert.ToInt32(value);
	}
}