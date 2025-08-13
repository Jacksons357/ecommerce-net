import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { HistoricoEvento } from '../models/historico.model';

@Injectable({
  providedIn: 'root'
})
export class HistoricoService {
  private readonly apiUrl = `${environment.apiBaseUrl}/historico`;

  constructor(private http: HttpClient) {}

  getHistorico(entidade?: string, entidadeId?: string): Observable<HistoricoEvento[]> {
    let params = new HttpParams();
    
    if (entidade) {
      params = params.set('entidade', entidade);
    }
    
    if (entidadeId) {
      params = params.set('entidadeId', entidadeId);
    }
    
    return this.http.get<HistoricoEvento[]>(this.apiUrl, { params });
  }
}
