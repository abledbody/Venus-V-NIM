using System;
using System.Collections.Generic;
using TextEncoding = System.Text.Encoding;

namespace QuickBin {
	public sealed partial class Serializer {
		readonly List<byte> bytes;
		int boolPlace = 0;
		
		/// <summary>
		/// The number of bytes in the Serializer.
		/// </summary>
		public int Length => bytes.Count;

		/// <summary>
		/// Generates a Serializer, initializing an empty list with a capacity of 0.
		/// </summary>
		public Serializer() => bytes = new List<byte>();
		/// <summary>
		/// Generates a Serializer, initializing an empty list with the specified capacity.
		/// </summary>
		/// <param name="capacity">The capacity of the list. See documentation for System.Collections.Generic.List<T>(int capacity) for details.</param>
		public Serializer(int capacity) => bytes = new List<byte>(capacity);

		public static implicit operator byte[](Serializer serializer) => serializer.bytes.ToArray();
		public static implicit operator List<byte>(Serializer serializer) => serializer.bytes;
		
		/// <summary>
		/// Executes an action. This method is purely for the convenience of chaining.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		public Serializer Then(Action action) {
			action();
			return this;
		}

		/// <summary>
		/// Clears the internal List so that the Serializer can be reused.
		/// </summary>
		/// <returns>This Serializer.</returns>
		public Serializer Clear() {
			bytes.Clear();
			return this;
		}
		
		/// <summary>
		/// Executes an action for each value in the specified IEnumerable. This method is purely for the convenience of chaining.
		/// </summary>
		/// <param name="values">The IEnumerable of values to act on.</param>
		/// <param name="action">The action to execute on each value.</param>
		/// <returns>This serializer.</returns>
		public Serializer ForEach<T>(IEnumerable<T> values, Action<T> action) {
			foreach (var value in values)
				action(value);
			return this;
		}

		private Serializer WriteGeneric<T>(T value, Func<T, byte> f) {
			bytes.Add(f(value));
			boolPlace = 0;
			return this;
		}

		private Serializer WriteGeneric<T>(T value, Func<T, byte[]> f) {
			bytes.AddRange(f(value));
			boolPlace = 0;
			return this;
		}

		public Serializer Write(bool value)   => WriteGeneric(value, x => x ? (byte)1 : (byte)0);
		public Serializer Write(byte value)   => WriteGeneric(value, x => x);
		public Serializer Write(sbyte value)  => WriteGeneric(value, x => (byte)x);
		public Serializer Write(char value)   => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(short value)  => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(ushort value) => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(int value)    => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(uint value)   => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(long value)   => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(ulong value)  => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(float value)  => WriteGeneric(value, BitConverter.GetBytes);
		public Serializer Write(double value) => WriteGeneric(value, BitConverter.GetBytes);
		
		public Serializer Write(byte[] value) => WriteGeneric(value, x => x);
		
		// Anything larger than uint is not supported, since .Length is an int.
		public Serializer WriteIntSized(byte[] value) => Write(value.Length).Write(value);
		public Serializer WriteShortSized(byte[] value) => Write((short)value.Length).Write(value);
		public Serializer WriteSByteSized(byte[] value) => Write((sbyte)value.Length).Write(value);
		public Serializer WriteUIntSized(byte[] value) => Write((uint)value.Length).Write(value);
		public Serializer WriteUShortSized(byte[] value) => Write((ushort)value.Length).Write(value);
		public Serializer WriteByteSized(byte[] value) => Write((byte)value.Length).Write(value);
		
		public Serializer Write(string value, TextEncoding encoding) => Write(encoding.GetBytes(value));
		public Serializer Write(string value) => Write(value, TextEncoding.UTF8);
		
		public Serializer WriteIntSized(string value) => Write(value.Length).Write(value);
		public Serializer WriteShortSized(string value) => Write((short)value.Length).Write(value);
		public Serializer WriteSByteSized(string value) => Write((sbyte)value.Length).Write(value);
		public Serializer WriteUIntSized(string value) => Write((uint)value.Length).Write(value);
		public Serializer WriteUShortSized(string value) => Write((ushort)value.Length).Write(value);
		public Serializer WriteByteSized(string value) => Write((byte)value.Length).Write(value);
		

		public Serializer Write(DateTime value) => Write(value.Ticks);
		public Serializer Write(TimeSpan value) => Write(value.Ticks);

		/// <summary>
		/// Writes booleans into the same byte if possible.
		/// </summary>
		/// <param name="value">The boolean to write.</param>
		/// <param name="forceNewByte">Whether to force writing a new byte, even if there's still space for flags in the current byte.</param>
		/// <returns>This Serializer.</returns>
		public Serializer WriteFlag(bool value, bool forceNewByte = false) {
			if (forceNewByte)
				boolPlace = 0;
			
			if (boolPlace == 0)
				Write(value);
			else
				bytes[^1] |= (byte)(value ? 1 << boolPlace : 0);
			
			boolPlace++;
			boolPlace %= 8;
			return this;
		}

		public Serializer Write(Version value) =>
			Write(value.Major)
			.Write(value.Minor)
			.Write(value.Build)
			.Write(value.Revision);
	}
}