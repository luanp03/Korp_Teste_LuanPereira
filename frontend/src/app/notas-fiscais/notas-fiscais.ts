import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http'; 
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-notas-fiscais',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule,
    MatProgressBarModule,
    MatIconModule
  ],
  templateUrl: './notas-fiscais.html',
  styleUrl: './notas-fiscais.scss'
})
export class NotasFiscaisComponent implements OnInit {

  itensNota: any[] = [];
  notasFiscais: any[] = [];
  produtosDisponiveis: any[] = [];
  loading = false; 
  proximoNumero: number = 0;

  displayedColumns: string[] = ['id', 'numero', 'itens', 'status', 'acoes'];

  constructor(
    private http: HttpClient,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.adicionarItem();
    this.carregarNotas();
    this.carregarProdutosDoEstoque();
    this.obterProximoNumero();
  }

  obterProximoNumero() {
    this.http.get<number>('http://localhost:5236/api/NotasFiscais/proximo-numero')
      .subscribe(n => this.proximoNumero = n);
  }

  excluirNota(id: string) {
    if (confirm('Deseja excluir esta nota e devolver o saldo ao estoque?')) {
      this.http.delete(`http://localhost:5236/api/NotasFiscais/${id}`)
        .subscribe(() => {
          this.carregarNotas();
          this.carregarProdutosDoEstoque();
          this.obterProximoNumero();
        });
    }
  }

  carregarProdutosDoEstoque() {
    this.http.get<any[]>('http://localhost:5083/api/Produtos').subscribe({
      next: (dados) => {
        this.produtosDisponiveis = dados;
        this.cdRef.detectChanges();
      },
      error: (err) => console.error('Erro ao buscar produtos:', err)
    });
  }

  carregarNotas() {
    this.http.get<any[]>('http://localhost:5236/api/NotasFiscais')
      .subscribe(dados => { 
        this.notasFiscais = dados;
        this.cdRef.detectChanges();
      });
  }

  adicionarItem() {
    this.itensNota.push({ produtoCodigo: '', quantidade: 1 });
  }

  removerItem(index: number) {
    if (this.itensNota.length > 1) {
      this.itensNota.splice(index, 1);
    }
  }

  criarNotaFiscal() {
    for (const item of this.itensNota) {
      const produto = this.produtosDisponiveis.find(p => p.codigo === item.produtoCodigo);
      if (produto && item.quantidade > produto.saldo) {
        alert(`Saldo insuficiente para o produto ${produto.descricao}. Disponível: ${produto.saldo}`);
        return;
      }
    }

    const payload = { 
      itens: this.itensNota.map(item => ({
        produtoCodigo: item.produtoCodigo,
        quantidade: item.quantidade,
        preco: 0, 
        descricao: "" 
      }))
    };

    this.loading = true;

    this.http.post('http://localhost:5236/api/NotasFiscais', payload).subscribe({
      next: () => {
        alert('Nota gerada com sucesso!');
        this.carregarNotas();         
        this.carregarProdutosDoEstoque(); 
        this.obterProximoNumero();      
        this.itensNota = [{ produtoCodigo: '', quantidade: 1 }];
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        console.error("Erro ao gerar nota:", err);
        alert('Erro ao gerar nota fiscal.');
      }
    });
  }

  imprimirNota(nota: any) {
    if (nota.status === 'Aberta' || nota.status === 0) {
      this.loading = true;
      this.http.post(`http://localhost:5236/api/NotasFiscais/${nota.id}/imprimir`, {})
        .pipe(finalize(() => this.loading = false))
        .subscribe({
          next: () => {
            alert('Sucesso: Nota fechada e estoque atualizado!');
            this.carregarNotas();
          },
          error: (err) => {
            const msg = typeof err.error === 'string' ? err.error : 'Erro interno no servidor.';
            alert(msg);
          }
        });
    }
  }

  podeGerarNota(): boolean {
    if (this.itensNota.length === 0) return false;
    return !this.itensNota.some(item => {
      const produto = this.produtosDisponiveis.find(p => p.codigo === item.produtoCodigo);
      return !produto || item.quantidade > produto.saldo || produto.saldo <= 0;
    });
  }
}