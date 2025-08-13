import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { LayoutComponent } from './components/layout/layout.component';
import { ClientesComponent } from './components/clientes/clientes.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'clientes', component: ClientesComponent },
      { path: 'produtos', loadComponent: () => import('./components/produtos/produtos.component').then(m => m.ProdutosComponent) },
      { path: 'pedidos', loadComponent: () => import('./components/pedidos/pedidos.component').then(m => m.PedidosComponent) },
      { path: 'pedidos/novo', loadComponent: () => import('./components/pedidos/novo-pedido/novo-pedido.component').then(m => m.NovoPedidoComponent) },
      { path: 'pedidos/:id', loadComponent: () => import('./components/pedidos/pedido-detalhe/pedido-detalhe.component').then(m => m.PedidoDetalheComponent) }
    ]
  }
];
