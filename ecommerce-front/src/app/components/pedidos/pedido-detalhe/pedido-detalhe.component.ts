import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PedidoService } from '../../../services/pedido.service';
import { HistoricoService } from '../../../services/historico.service';
import { PedidoDetalhe, StatusPedido } from '../../../models/pedido.model';
import { HistoricoEvento } from '../../../models/historico.model';

@Component({
  selector: 'app-pedido-detalhe',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './pedido-detalhe.component.html',
  styleUrl: './pedido-detalhe.component.css'
})
export class PedidoDetalheComponent implements OnInit {
  pedido: PedidoDetalhe | null = null;
  historico: HistoricoEvento[] = [];
  loading = true;
  error: string | null = null;
  StatusPedido = StatusPedido;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private pedidoService: PedidoService,
    private historicoService: HistoricoService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.carregarPedido(id);
      this.carregarHistorico(id);
    }
  }

  carregarPedido(id: string): void {
    this.pedidoService.getPedidoById(id).subscribe({
      next: (pedido) => {
        this.pedido = pedido;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Erro ao carregar pedido';
        this.loading = false;
        console.error(err);
      }
    });
  }

  carregarHistorico(id: string): void {
    this.historicoService.getHistorico('Pedido', id).subscribe({
      next: (historico) => {
        this.historico = historico;
      },
      error: (err) => {
        console.error('Erro ao carregar histórico:', err);
      }
    });
  }

  pagar(): void {
    if (this.pedido && this.pedido.status === StatusPedido.Criado) {
      this.pedidoService.pagarPedido(this.pedido.id).subscribe({
        next: () => {
          this.carregarPedido(this.pedido!.id);
          this.carregarHistorico(this.pedido!.id);
        },
        error: (err) => {
          this.error = err.error?.detail || 'Erro ao pagar pedido';
          console.error(err);
        }
      });
    }
  }

  cancelar(): void {
    if (this.pedido && this.pedido.status === StatusPedido.Criado) {
      this.pedidoService.cancelarPedido(this.pedido.id).subscribe({
        next: () => {
          this.carregarPedido(this.pedido!.id);
          this.carregarHistorico(this.pedido!.id);
        },
        error: (err) => {
          this.error = err.error?.detail || 'Erro ao cancelar pedido';
          console.error(err);
        }
      });
    }
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