import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Adicionar token de autorização se disponível
  const token = authService.getToken();
  
  console.log('AuthInterceptor - URL:', req.url);
  console.log('AuthInterceptor - Token exists:', !!token);
  console.log('AuthInterceptor - Token:', token ? token.substring(0, 20) + '...' : 'null');
  console.log('AuthInterceptor - Is login route:', req.url.includes('/auth/login'));
  
  if (token && !req.url.includes('/auth/login')) {
    console.log('AuthInterceptor - Adding Bearer token');
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  } else if (!token && !req.url.includes('/auth/login')) {
    console.warn('AuthInterceptor - No token available for protected route');
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.error('AuthInterceptor - HTTP Error:', error.status, error.message);
      if (error.status === 401) {
        console.log('AuthInterceptor - 401 error, logging out');
        // Token inválido ou expirado
        authService.logout();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};
