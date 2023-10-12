using System;

namespace Venus {
	public interface IMessage<TMessage> where TMessage : Enum {
		TMessage messageType { get; }
	}
}