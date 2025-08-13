import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProdutoService } from '../../services/produto.service';
import { Produto, CreateProdutoRequest, UpdateProdutoRequest } from '../../models/produto.model';
import { CurrencyMaskDirective } from '../../directives/currency-mask.directive';

@Component({
  selector: 'app-produtos',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ReactiveFormsModule, CurrencyMaskDirective],
  templateUrl: './produtos.component.html',
  styleUrl: './produtos.component.css'
})
export class ProdutosComponent implements OnInit {
  produtos: Produto[] = [];
  loading = true;
  error: string | null = null;
  searchTerm = '';
  editingProduto: Produto | null = null;
  showForm = false;
  produtoForm: FormGroup;

  constructor(
    private produtoService: ProdutoService,
    private fb: FormBuilder
  ) {
    this.produtoForm = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(2)]],
      preco: [0, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.carregarProdutos();
  }

  carregarProdutos(): void {
    this.loading = true;
    this.produtoService.getProdutos().subscribe({
      next: (produtos) => {
        this.produtos = produtos;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar produtos';
        this.loading = false;
        console.error(err);
      }
    });
  }

  get produtosFiltrados(): Produto[] {
    if (!this.searchTerm) return this.produtos;
    return this.produtos.filter(produto =>
      produto.nome.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  novoProduto(): void {
    this.editingProduto = null;
    this.produtoForm.reset();
    this.showForm = true;
  }

  editarProduto(produto: Produto): void {
    this.editingProduto = produto;
    this.produtoForm.patchValue({
      nome: produto.nome,
      preco: produto.preco
    });
    this.showForm = true;
  }

  salvarProduto(): void {
    if (this.produtoForm.valid) {
      const produtoData = this.produtoForm.value;

      if (this.editingProduto) {
        // Editar
        this.produtoService.updateProduto(this.editingProduto.id, produtoData).subscribe({
          next: () => {
            this.carregarProdutos();
            this.cancelarForm();
          },
          error: (err) => {
            this.error = err.error?.detail || 'Erro ao atualizar produto';
          }
        });
      } else {
        // Criar
        this.produtoService.createProduto(produtoData).subscribe({
          next: () => {
            this.carregarProdutos();
            this.cancelarForm();
          },
          error: (err) => {
            this.error = err.error?.detail || 'Erro ao criar produto';
          }
        });
      }
    }
  }

  excluirProduto(produto: Produto): void {
    if (confirm(`Deseja excluir o produto "${produto.nome}"?`)) {
      this.produtoService.deleteProduto(produto.id).subscribe({
        next: () => {
          this.carregarProdutos();
          this.error = null; // Limpar mensagem de erro em caso de sucesso
        },
        error: (err) => {
          console.error('Erro ao excluir produto:', err);
          
          // Verificar se é erro 400 (relacionamento) ou outro erro
          if (err.status === 400) {
            this.error = err.error?.detail || 'Este produto não pode ser excluído pois está sendo usado em pedidos.';
          } else {
            this.error = err.error?.detail || 'Erro ao excluir produto.';
          }
        }
      });
    }
  }

  cancelarForm(): void {
    this.showForm = false;
    this.editingProduto = null;
    this.produtoForm.reset();
    this.error = null;
  }
}