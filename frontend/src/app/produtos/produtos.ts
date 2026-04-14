import { Component, OnInit, ChangeDetectorRef } from "@angular/core"; 
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { CommonModule } from "@angular/common"
import { MatCardModule } from "@angular/material/card"
import { MatFormFieldModule } from "@angular/material/form-field"
import { MatInputModule } from "@angular/material/input"
import { MatButtonModule } from "@angular/material/button"
import { MatTableModule } from "@angular/material/table"
import { FormsModule } from "@angular/forms"

@Component({
  selector: 'app-produtos',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTableModule
  ],
  templateUrl: './produtos.html',
  styleUrl: './produtos.scss'
})
export class ProdutosComponent implements OnInit {
  
  private readonly API_URL = 'http://localhost:5083/api/Produtos'; 

  produtos: any[] = [];
  displayedColumns: string[] = ['codigo', 'descricao', 'saldo', 'acoes'];

  novoCodigo = '';
  novaDescricao = '';
  novoSaldo = 0;

  constructor(private http: HttpClient, private cdRef: ChangeDetectorRef){}

  ngOnInit() {
    this.carregarProdutos();
  }

  carregarProdutos() {
    this.http.get<any[]>(this.API_URL).subscribe({
      next: (dados) => {
        this.produtos = dados;
        setTimeout(() => {
        this.cdRef.detectChanges();
        }, 0);

      },
      error: (err) => console.error('Erro ao buscar do banco:', err)
    });
  }

  cadastrarProduto() {
    if (this.novoCodigo && this.novaDescricao && this.novoSaldo > 0) {
      
      const payload = {
        codigo: this.novoCodigo,
        descricao: this.novaDescricao,
        saldo: this.novoSaldo || 0
      };

      console.log('Enviando para a API:', payload);

      this.http.post(this.API_URL, payload).subscribe({
        next: () => {
          console.log('Salvo no banco com sucesso!');
          this.carregarProdutos();
          setTimeout(() => this.limparCampos(), 0);
        },
        error: (err) => {
          console.error('Erro ao salvar:', err.error);
          alert('Erro ao salvar. Verifique o console para detalhes.');
        }
      });

    } else {
      console.log('Preencha todos os campos!');
    }
  }

  limparCampos() {
    this.novoCodigo = '';
    this.novaDescricao = '';
    this.novoSaldo = 0;
  }
  
  excluirProduto(id: string) {
    if (confirm('Tem certeza que deseja excluir este produto?')) {
      this.http.delete(`${this.API_URL}/${id}`).subscribe({
        next: () => {
          console.log('Produto removido do banco!');
          this.carregarProdutos();
        },
        error: (err) => {
          console.error('Erro ao excluir:', err);
          alert('Não foi possível excluir o produto. Ele pode estar vinculado a uma nota fiscal.');
        }
      });
    }
  }
}