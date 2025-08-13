import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ClienteService } from '../../services/cliente.service';
import { Cliente, CreateClienteRequest, UpdateClienteRequest } from '../../models/cliente.model';
import { cpfValidator } from '../../utils/cpf-validator';

@Component({
  selector: 'app-clientes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './clientes.component.html',
  styleUrl: './clientes.component.css'
})
export class ClientesComponent implements OnInit {
  clientes: Cliente[] = [];
  clienteForm!: FormGroup;
  showForm = false;
  editingId: string | null = null;
  loading = false;
  searchTerm = '';
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private clienteService: ClienteService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadClientes();
  }

  initForm(): void {
    this.clienteForm = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(2)]],
      cpf: ['', [Validators.required, Validators.pattern(/^\d{3}\.\d{3}\.\d{3}-\d{2}$/), cpfValidator()]]
    });
  }

  loadClientes(): void {
    this.loading = true;
    this.clienteService.getAll().subscribe({
      next: (clientes) => {
        this.clientes = clientes;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar clientes:', error);
        this.loading = false;
      }
    });
  }

  get filteredClientes(): Cliente[] {
    if (!this.searchTerm) return this.clientes;
    
    return this.clientes.filter(cliente =>
      cliente.nome.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
      cliente.cpf.includes(this.searchTerm)
    );
  }

  showCreateForm(): void {
    this.editingId = null;
    this.clienteForm.reset();
    this.showForm = true;
  }

  editCliente(cliente: Cliente): void {
    this.editingId = cliente.id;
    this.clienteForm.patchValue({
      nome: cliente.nome,
      cpf: cliente.cpf
    });
    this.showForm = true;
  }

  onSubmit(): void {
    if (this.clienteForm.valid) {
      const formData = this.clienteForm.value;
      
      if (this.editingId) {
        this.updateCliente(this.editingId, formData);
      } else {
        this.createCliente(formData);
      }
    }
  }

  createCliente(data: CreateClienteRequest): void {
    this.clienteService.create(data).subscribe({
      next: () => {
        this.loadClientes();
        this.cancelForm();
      },
      error: (error) => {
        console.error('Erro ao criar cliente:', error);
      }
    });
  }

  updateCliente(id: string, data: UpdateClienteRequest): void {
    this.clienteService.update(id, data).subscribe({
      next: () => {
        this.loadClientes();
        this.cancelForm();
      },
      error: (error) => {
        console.error('Erro ao atualizar cliente:', error);
      }
    });
  }

  deleteCliente(id: string): void {
    if (confirm('Tem certeza que deseja excluir este cliente?')) {
      this.clienteService.delete(id).subscribe({
        next: () => {
          this.loadClientes();
          this.error = null; // Limpar mensagem de erro em caso de sucesso
        },
        error: (err) => {
          console.error('Erro ao excluir cliente:', err);
          
          // Verificar se é erro 400 (relacionamento) ou outro erro
          if (err.status === 400) {
            this.error = err.error?.detail || 'Este cliente não pode ser excluído pois possui pedidos.';
          } else {
            this.error = err.error?.detail || 'Erro ao excluir cliente.';
          }
        }
      });
    }
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingId = null;
    this.clienteForm.reset();
    this.error = null;
  }

  formatCpf(event: any): void {
    let value = event.target.value.replace(/\D/g, '');
    value = value.replace(/(\d{3})(\d)/, '$1.$2');
    value = value.replace(/(\d{3})(\d)/, '$1.$2');
    value = value.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
    event.target.value = value;
    this.clienteForm.patchValue({ cpf: value });
    
    // Força a validação do campo
    this.clienteForm.get('cpf')?.updateValueAndValidity();
  }

  get nome() { return this.clienteForm.get('nome'); }
  get cpf() { return this.clienteForm.get('cpf'); }
}
