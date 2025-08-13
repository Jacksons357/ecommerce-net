import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Cliente, CreateClienteRequest, UpdateClienteRequest } from '../models/cliente.model';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {
  private readonly apiUrl = `${environment.apiBaseUrl}/clientes`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.apiUrl);
  }

  getClientes(): Observable<Cliente[]> {
    return this.getAll();
  }

  getById(id: string): Observable<Cliente> {
    return this.http.get<Cliente>(`${this.apiUrl}/${id}`);
  }

  create(cliente: CreateClienteRequest): Observable<Cliente> {
    return this.http.post<Cliente>(this.apiUrl, cliente);
  }

  update(id: string, cliente: UpdateClienteRequest): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.apiUrl}/${id}`, cliente);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
