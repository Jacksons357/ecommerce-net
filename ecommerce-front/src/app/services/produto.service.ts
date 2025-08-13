import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Produto, CreateProdutoRequest, UpdateProdutoRequest } from '../models/produto.model';

@Injectable({
  providedIn: 'root'
})
export class ProdutoService {
  private readonly apiUrl = `${environment.apiBaseUrl}/produtos`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Produto[]> {
    return this.http.get<Produto[]>(this.apiUrl);
  }

  getProdutos(): Observable<Produto[]> {
    return this.getAll();
  }

  getById(id: string): Observable<Produto> {
    return this.http.get<Produto>(`${this.apiUrl}/${id}`);
  }

  create(produto: CreateProdutoRequest): Observable<Produto> {
    return this.http.post<Produto>(this.apiUrl, produto);
  }

  createProduto(produto: CreateProdutoRequest): Observable<Produto> {
    return this.create(produto);
  }

  update(id: string, produto: UpdateProdutoRequest): Observable<Produto> {
    return this.http.put<Produto>(`${this.apiUrl}/${id}`, produto);
  }

  updateProduto(id: string, produto: UpdateProdutoRequest): Observable<Produto> {
    return this.update(id, produto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  deleteProduto(id: string): Observable<void> {
    return this.delete(id);
  }
}
