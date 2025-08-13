import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  PedidoResumo, 
  PedidoDetalhe, 
  CreatePedidoRequest, 
  StatusPedido 
} from '../models/pedido.model';

@Injectable({
  providedIn: 'root'
})
export class PedidoService {
  private readonly apiUrl = `${environment.apiBaseUrl}/pedidos`;

  constructor(private http: HttpClient) {}

  getAll(status?: StatusPedido): Observable<PedidoResumo[]> {
    let params = new HttpParams();
    if (status !== undefined) {
      params = params.set('status', status.toString());
    }
    
    return this.http.get<PedidoResumo[]>(this.apiUrl, { params });
  }

  getPedidos(status?: StatusPedido): Observable<PedidoResumo[]> {
    return this.getAll(status);
  }

  getById(id: string): Observable<PedidoDetalhe> {
    return this.http.get<PedidoDetalhe>(`${this.apiUrl}/${id}`);
  }

  getPedidoById(id: string): Observable<PedidoDetalhe> {
    return this.getById(id);
  }

  create(pedido: CreatePedidoRequest): Observable<PedidoDetalhe> {
    return this.http.post<PedidoDetalhe>(this.apiUrl, pedido);
  }

  createPedido(pedido: CreatePedidoRequest): Observable<PedidoDetalhe> {
    return this.create(pedido);
  }

  pagar(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/pagar`, {});
  }

  pagarPedido(id: string): Observable<void> {
    return this.pagar(id);
  }

  cancelar(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/cancelar`, {});
  }

  cancelarPedido(id: string): Observable<void> {
    return this.cancelar(id);
  }
}
