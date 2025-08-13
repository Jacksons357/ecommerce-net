export interface Cliente {
  id: string;
  nome: string;
  cpf: string;
  dataCriacao: string;
  dataAtualizacao?: string;
}

export interface CreateClienteRequest {
  nome: string;
  cpf: string;
}

export interface UpdateClienteRequest {
  nome: string;
  cpf: string;
}
