export interface Produto {
  id: string;
  nome: string;
  preco: number;
  dataCriacao: string;
  dataAtualizacao?: string;
}

export interface CreateProdutoRequest {
  nome: string;
  preco: number;
}

export interface UpdateProdutoRequest {
  nome: string;
  preco: number;
}
