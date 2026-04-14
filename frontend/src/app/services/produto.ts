import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProdutoService {
  private apiUrl = 'http://localhost:5083/api/Produtos';
  constructor(private http: HttpClient) {}

  getProdutos(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  cadastrar(produto: any): Observable<any> {
    return this.http.post(this.apiUrl, produto);
  }
}