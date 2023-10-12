using System;

namespace Venus {
	public sealed class ClientVnim<TMessage> : Vnim<TMessage> where TMessage : Enum {
		public ClientVnim() : base(){
			
		}
		
		public void Send(IMessage<TMessage> message) {
			
		}
	}
}