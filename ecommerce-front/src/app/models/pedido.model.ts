import { Cliente } from './cliente.model';

export enum StatusPedido {
  Criado = 0,
  Pago = 1,
  Cancelado = 2
}

export interface PedidoResumo {
  id: string;
  clienteNome: string;
  dataPedido: string;
  status: StatusPedido;
  total: number;
}

export interface PedidoItemDetalhe {
  id: string;
  produtoId: string;
  produtoNome: string;
  quantidade: number;
  precoUnitario: number;
  subtotal: number;
}

export interface PedidoDetalhe {
  id: string;
  cliente: Cliente;
  itens: PedidoItemDetalhe[];
  dataPedido: string;
  status: StatusPedido;
  total: number;
}

export interface CreatePedidoItemRequest {
  produtoId: string;
  quantidade: number;
}

export interface CreatePedidoRequest {
  clienteId: string;
  itens: CreatePedidoItemRequest[];
}

export const StatusPedidoLabels = {
  [StatusPedido.Criado]: 'Criado',
  [StatusPedido.Pago]: 'Pago',
  [StatusPedido.Cancelado]: 'Cancelado'
};
