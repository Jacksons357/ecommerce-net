import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Se já estiver logado, redirecionar
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/clientes']);
    }

    this.loginForm = this.fb.group({
      email: ['admin@local', [Validators.required, Validators.email]],
      senha: ['Admin@123', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.loading = true;
      this.error = '';

      this.authService.login(this.loginForm.value).subscribe({
        next: () => {
          this.router.navigate(['/clientes']);
        },
        error: (error) => {
          this.loading = false;
          this.error = 'Email ou senha inválidos';
          console.error('Erro no login:', error);
        }
      });
    }
  }

  get email() { return this.loginForm.get('email'); }
  get senha() { return this.loginForm.get('senha'); }
}
