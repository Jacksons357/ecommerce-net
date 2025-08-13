export interface HistoricoEvento {
  id: string;
  entidade: string;
  entidadeId: string;
  acao: string;
  dadosAntes?: string;
  dadosDepois?: string;
  dataOcorrencia: string;
  usuario: string;
}
