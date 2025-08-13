export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  token: string;
  nome: string;
}

export interface User {
  email: string;
  nome: string;
}
