import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { PedidoService } from '../../../services/pedido.service';
import { ClienteService } from '../../../services/cliente.service';
import { ProdutoService } from '../../../services/produto.service';
import { Cliente } from '../../../models/cliente.model';
import { Produto } from '../../../models/produto.model';
import { CreatePedidoRequest } from '../../../models/pedido.model';

@Component({
  selector: 'app-novo-pedido',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, ReactiveFormsModule],
  templateUrl: './novo-pedido.component.html',
  styleUrl: './novo-pedido.component.css'
})
export class NovoPedidoComponent implements OnInit {
  pedidoForm: FormGroup;
  clientes: Cliente[] = [];
  produtos: Produto[] = [];
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private pedidoService: PedidoService,
    private clienteService: ClienteService,
    private produtoService: ProdutoService,
    private cdr: ChangeDetectorRef
  ) {
    this.pedidoForm = this.fb.group({
      clienteId: ['', Validators.required],
      itens: this.fb.array([])
    });
  }

  ngOnInit(): void {
    console.log('NovoPedidoComponent - ngOnInit iniciado');
    
    // Fallback de dados para teste (remover em produção)
    this.setupFallbackData();
    
    // Carregar dados da API primeiro
    this.carregarDados();
  }

  setupFallbackData(): void {
    // Dados de fallback para garantir que o select funcione
    this.clientes = [
      {
        id: '22222222-2222-2222-2222-222222222222',
        nome: 'João Silva (Fallback)',
        cpf: '123.456.789-00',
        dataCriacao: '2024-01-15T10:30:00Z'
      }
    ];

    this.produtos = [
      {
        id: '33333333-3333-3333-3333-333333333333',
        nome: 'Notebook Gamer (Fallback)',
        preco: 2500.00,
        dataCriacao: '2024-01-15T10:30:00Z'
      },
      {
        id: '44444444-4444-4444-4444-444444444444',
        nome: 'Mouse Wireless (Fallback)',
        preco: 89.90,
        dataCriacao: '2024-01-15T10:30:00Z'
      }
    ];

    console.log('Fallback data configurado - Clientes:', this.clientes.length, 'Produtos:', this.produtos.length);
  }

  carregarDados(): void {
    this.loading = true;
    this.error = null;
    let clientesCarregados = false;
    let produtosCarregados = false;

    const checkLoadingComplete = () => {
      if (clientesCarregados && produtosCarregados) {
        this.loading = false;
        console.log('Todos os dados carregados com sucesso!');
        console.log('Clientes disponíveis:', this.clientes.length);
        console.log('Produtos disponíveis:', this.produtos.length);
        
        // Forçar atualização da interface após carregamento completo
        this.atualizarInterface();
      }
    };
    
    this.clienteService.getClientes().subscribe({
      next: (clientes) => {
        console.log('✅ Clientes carregados da API:', clientes);
        if (clientes && clientes.length > 0) {
          this.clientes = clientes;
          console.log('✅ Dados da API sobrescreveram fallback');
        } else {
          console.log('⚠️ API retornou lista vazia, mantendo fallback');
        }
        clientesCarregados = true;
        checkLoadingComplete();
      },
      error: (err) => {
        console.error('❌ Erro ao carregar clientes da API:', err);
        console.error('📌 Status do erro:', err.status);
        console.log('📌 Mantendo dados de fallback para clientes');
        // Não limpar this.clientes - manter fallback
        clientesCarregados = true;
        checkLoadingComplete();
      }
    });

    this.produtoService.getProdutos().subscribe({
      next: (produtos) => {
        console.log('✅ Produtos carregados da API:', produtos);
        console.log('📊 Tipo de dados recebidos:', typeof produtos);
        console.log('📊 É array?', Array.isArray(produtos));
        if (produtos && produtos.length > 0) {
          console.log('📊 Primeiro produto:', produtos[0]);
          this.produtos = produtos;
          console.log('✅ Dados da API sobrescreveram fallback de produtos');
        } else {
          console.log('⚠️ API retornou lista vazia de produtos, mantendo fallback');
        }
        
        // Forçar detecção de mudanças após carregar produtos
        this.cdr.detectChanges();
        
        produtosCarregados = true;
        checkLoadingComplete();
      },
      error: (err) => {
        console.error('❌ Erro ao carregar produtos da API:', err);
        console.error('📌 Status do erro:', err.status);
        console.error('📌 Mensagem do erro:', err.message);
        console.log('📌 Mantendo dados de fallback para produtos');
        // Não limpar this.produtos - manter fallback
        produtosCarregados = true;
        checkLoadingComplete();
      }
    });
  }

  get itens(): FormArray {
    return this.pedidoForm.get('itens') as FormArray;
  }

  get produtosIds(): string {
    return this.produtos.map(p => p.id).join(', ') || 'Vazio';
  }

  adicionarItem(): void {
    console.log('Adicionando novo item ao FormArray');
    console.log('Produtos disponíveis no momento da adição:', this.produtos.length);
    console.log('Estado atual dos produtos:', this.produtos);
    
    const itemForm = this.fb.group({
      produtoId: ['', Validators.required],
      quantidade: [1, [Validators.required, Validators.min(1)]]
    });
    
    // Adicionar listeners para mudanças nos campos após adicionar ao FormArray
    // Isso será feito após o push para evitar problemas de índice
    
    try {
      this.itens.push(itemForm);
      const itemIndex = this.itens.length - 1;
      console.log('Item adicionado com sucesso. Total de itens:', this.itens.length);
      
      // Adicionar listeners após o item estar no FormArray
      setTimeout(() => {
        const addedItem = this.itens.at(itemIndex);
        if (addedItem) {
          addedItem.get('produtoId')?.valueChanges.subscribe(() => {
            console.log(`Produto alterado no item ${itemIndex}`);
            this.cdr.detectChanges();
          });
          
          addedItem.get('quantidade')?.valueChanges.subscribe(() => {
            console.log(`Quantidade alterada no item ${itemIndex}`);
            this.cdr.detectChanges();
          });
        }
      }, 10);
      
      // Forçar detecção de mudanças após adicionar item
      this.cdr.detectChanges();
    } catch (error) {
      console.error('Erro ao adicionar item:', error);
    }
  }

  removerItem(index: number): void {
    this.itens.removeAt(index);
  }

  onClienteChange(event: any): void {
    console.log('Cliente selecionado:', event.target.value);
  }

  onProdutoChange(event: any, itemIndex: number): void {
    const produtoId = event.target.value;
    console.log('Produto selecionado no item', itemIndex, ':', produtoId);
    
    if (produtoId) {
      const produto = this.produtos.find(p => p.id === produtoId);
      if (produto) {
        console.log('Produto encontrado:', produto.nome, 'Preço:', produto.preco);
      }
    }
    
    // Forçar atualização da interface
    this.cdr.detectChanges();
  }

  onQuantidadeChange(itemIndex: number): void {
    console.log('Quantidade alterada no item', itemIndex);
    this.cdr.detectChanges();
  }

  trackByClienteId(index: number, cliente: Cliente): string {
    return cliente.id;
  }

  trackByProdutoId(index: number, produto: Produto): string {
    return produto.id;
  }

  getProdutoPreco(produtoId: string): number {
    if (!produtoId || !this.produtos || this.produtos.length === 0) {
      return 0;
    }
    
    const produto = this.produtos.find(p => p.id === produtoId);
    const preco = produto ? produto.preco : 0;
    
    console.log(`GetProdutoPreco para ID ${produtoId}: ${preco}`);
    return preco;
  }

  getSubtotal(index: number): number {
    try {
      const item = this.itens.at(index);
      if (!item) {
        console.log(`GetSubtotal: Item ${index} não encontrado`);
        return 0;
      }
      
      const produtoId = item.get('produtoId')?.value;
      const quantidade = item.get('quantidade')?.value || 0;
      const preco = this.getProdutoPreco(produtoId);
      
      const subtotal = preco * quantidade;
      
      console.log(`GetSubtotal item ${index}: produto=${produtoId}, qtd=${quantidade}, preço=${preco}, subtotal=${subtotal}`);
      return subtotal;
    } catch (error) {
      console.error(`Erro ao calcular subtotal do item ${index}:`, error);
      return 0;
    }
  }

  getTotal(): number {
    let total = 0;
    for (let i = 0; i < this.itens.length; i++) {
      total += this.getSubtotal(i);
    }
    return total;
  }

  atualizarInterface(): void {
    console.log('🔄 Atualizando interface...');
    console.log('🔄 Produtos após atualização:', this.produtos.length);
    
    // Adicionar item inicial se não houver nenhum
    if (this.itens.length === 0) {
      console.log('📝 Adicionando item inicial após carregamento');
      this.adicionarItem();
    }
    
    // Forçar detecção de mudanças
    this.cdr.detectChanges();
    
    // Forçar update dos FormArrays se necessário
    setTimeout(() => {
      this.cdr.markForCheck();
      this.cdr.detectChanges();
      console.log('✅ Interface atualizada!');
    }, 50);
  }

  salvarPedido(): void {
    if (this.pedidoForm.valid && this.itens.length > 0) {
      this.loading = true;
      this.error = null;

      const pedidoData: CreatePedidoRequest = {
        clienteId: this.pedidoForm.get('clienteId')?.value,
        itens: this.itens.value
      };

      this.pedidoService.createPedido(pedidoData).subscribe({
        next: (pedido) => {
          this.router.navigate(['/pedidos', pedido.id]);
        },
        error: (err) => {
          this.error = err.error?.detail || 'Erro ao criar pedido';
          this.loading = false;
        }
      });
    } else {
      this.error = 'Por favor, preencha todos os campos obrigatórios e adicione pelo menos um item.';
    }
  }
}