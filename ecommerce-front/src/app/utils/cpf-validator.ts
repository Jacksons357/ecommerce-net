import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Função que valida se um CPF é válido usando o algoritmo oficial
 * @param cpf - CPF a ser validado (pode conter ou não formatação)
 * @returns true se o CPF é válido, false caso contrário
 */
export function validarCpf(cpf: string): boolean {
  if (!cpf) return false;

  // Remove formatação (pontos e traços)
  const cpfLimpo = cpf.replace(/\D/g, '');

  // Verifica se tem 11 dígitos
  if (cpfLimpo.length !== 11) return false;

  // Verifica se todos os dígitos são iguais
  if (/^(\d)\1{10}$/.test(cpfLimpo)) return false;

  // Validação do primeiro dígito verificador
  let soma = 0;
  for (let i = 0; i < 9; i++) {
    soma += parseInt(cpfLimpo.charAt(i)) * (10 - i);
  }
  let resto = 11 - (soma % 11);
  let digito1 = resto < 2 ? 0 : resto;

  if (parseInt(cpfLimpo.charAt(9)) !== digito1) return false;

  // Validação do segundo dígito verificador
  soma = 0;
  for (let i = 0; i < 10; i++) {
    soma += parseInt(cpfLimpo.charAt(i)) * (11 - i);
  }
  resto = 11 - (soma % 11);
  let digito2 = resto < 2 ? 0 : resto;

  return parseInt(cpfLimpo.charAt(10)) === digito2;
}

/**
 * Validador Angular para CPF
 * @returns ValidatorFn que retorna ValidationErrors ou null
 */
export function cpfValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null; // Deixa a validação de required ser tratada separadamente
    }

    const isValid = validarCpf(control.value);
    return isValid ? null : { cpfInvalido: { value: control.value } };
  };
}
