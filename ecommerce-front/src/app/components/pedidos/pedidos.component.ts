import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PedidoService } from '../../services/pedido.service';
import { PedidoResumo, StatusPedido } from '../../models/pedido.model';

@Component({
  selector: 'app-pedidos',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './pedidos.component.html',
  styleUrl: './pedidos.component.css'
})
export class PedidosComponent implements OnInit {
  pedidos: PedidoResumo[] = [];
  loading = true;
  error: string | null = null;
  filtroStatus: StatusPedido | '' = '';
  StatusPedido = StatusPedido;

  constructor(private pedidoService: PedidoService) {}

  ngOnInit(): void {
    this.carregarPedidos();
  }

  carregarPedidos(): void {
    this.loading = true;
    const status = this.filtroStatus === '' ? undefined : this.filtroStatus;
    
    this.pedidoService.getPedidos(status).subscribe({
      next: (pedidos) => {
        this.pedidos = pedidos;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar pedidos';
        this.loading = false;
        console.error(err);
      }
    });
  }

  onFiltroChange(): void {
    this.carregarPedidos();
  }

  getStatusBadgeClass(status: StatusPedido): string {
    switch (status) {
      case StatusPedido.Criado:
        return 'badge-warning';
      case StatusPedido.Pago:
        return 'badge-success';
      case StatusPedido.Cancelado:
        return 'badge-danger';
      default:
        return 'badge-secondary';
    }
  }

  getStatusText(status: StatusPedido): string {
    switch (status) {
      case StatusPedido.Criado:
        return 'Criado';
      case StatusPedido.Pago:
        return 'Pago';
      case StatusPedido.Cancelado:
        return 'Cancelado';
      default:
        return 'Desconhecido';
    }
  }
}