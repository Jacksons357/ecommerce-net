import { HttpInterceptorFn } from '@angular/common/http';

export const AuthSimpleInterceptor: HttpInterceptorFn = (req, next) => {
  // Pegar token do localStorage
  const token = localStorage.getItem('auth_token');
  
  // Se tem token e não é rota de login, adicionar Authorization header
  if (token && !req.url.includes('/auth/login')) {
    const authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    return next(authReq);
  }
  
  return next(req);
};
