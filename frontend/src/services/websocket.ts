import { io, Socket } from 'socket.io-client';
import { WebSocketEvent } from '../types';

class WebSocketService {
  private socket: Socket | null = null;
  private listeners: Map<string, Function[]> = new Map();

  connect() {
    if (this.socket?.connected) return;

    this.socket = io('http://localhost:7001', {
      transports: ['websocket', 'polling'],
      autoConnect: true,
      reconnection: true,
      reconnectionDelay: 1000,
      reconnectionAttempts: 5
    });

    this.socket.on('connect', () => {
      console.log('WebSocket connected');
    });

    this.socket.on('disconnect', () => {
      console.log('WebSocket disconnected');
    });

    this.socket.on('error', (error) => {
      console.error('WebSocket error:', error);
    });

    // Escutar eventos do sistema
    this.socket.on('proposal_created', (data) => {
      this.emit('proposal_created', data);
    });

    this.socket.on('proposal_updated', (data) => {
      this.emit('proposal_updated', data);
    });

    this.socket.on('contract_created', (data) => {
      this.emit('contract_created', data);
    });

    this.socket.on('status_changed', (data) => {
      this.emit('status_changed', data);
    });
  }

  disconnect() {
    if (this.socket) {
      this.socket.disconnect();
      this.socket = null;
    }
  }

  on(event: string, callback: Function) {
    if (!this.listeners.has(event)) {
      this.listeners.set(event, []);
    }
    this.listeners.get(event)!.push(callback);
  }

  off(event: string, callback?: Function) {
    if (!callback) {
      this.listeners.delete(event);
      return;
    }

    const callbacks = this.listeners.get(event);
    if (callbacks) {
      const index = callbacks.indexOf(callback);
      if (index > -1) {
        callbacks.splice(index, 1);
      }
    }
  }

  private emit(event: string, data: any) {
    const callbacks = this.listeners.get(event);
    if (callbacks) {
      callbacks.forEach(callback => {
        try {
          callback(data);
        } catch (error) {
          console.error(`Error in WebSocket callback for ${event}:`, error);
        }
      });
    }
  }

  // Métodos específicos para enviar eventos
  subscribeToProposals() {
    if (this.socket) {
      this.socket.emit('subscribe_proposals');
    }
  }

  subscribeToContracts() {
    if (this.socket) {
      this.socket.emit('subscribe_contracts');
    }
  }

  // Simular eventos para demonstração (quando WebSocket não estiver disponível)
  simulateEvent(event: WebSocketEvent) {
    this.emit(event.type, event.data);
  }
}

export const webSocketService = new WebSocketService();
export default webSocketService; 