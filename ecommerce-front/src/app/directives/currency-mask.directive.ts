import { Directive, ElementRef, HostListener, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Directive({
  selector: '[appCurrencyMask]',
  standalone: true,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CurrencyMaskDirective),
      multi: true
    }
  ]
})
export class CurrencyMaskDirective implements ControlValueAccessor {
  private onChange = (value: any) => {};
  private onTouched = () => {};
  private element: HTMLInputElement;
  private isUpdating = false;

  constructor(private el: ElementRef) {
    this.element = this.el.nativeElement;
  }

  @HostListener('input', ['$event'])
  onInput(event: KeyboardEvent): void {
    if (this.isUpdating) {
      return;
    }

    const target = event.target as HTMLInputElement;
    const value = target.value;
    const cursorPosition = target.selectionStart || 0;
    
    // Remove todos os caracteres não numéricos
    const numericOnly = value.replace(/\D/g, '');
    
    if (numericOnly === '') {
      this.updateValue('', 0, 0);
      return;
    }

    // Converte para centavos (número inteiro)
    const numericValue = parseInt(numericOnly, 10);
    
    // Converte de volta para reais (divide por 100)
    const realValue = numericValue / 100;
    
    // Formata como moeda
    const formattedValue = this.formatCurrency(realValue);
    
    // Calcula a nova posição do cursor
    const newCursorPosition = this.calculateCursorPosition(
      value, formattedValue, cursorPosition, numericOnly.length
    );
    
    this.updateValue(formattedValue, realValue, newCursorPosition);
  }

  @HostListener('blur', ['$event'])
  onBlur(event: any): void {
    this.onTouched();
    
    // Se o campo estiver vazio, formatar como R$ 0,00
    if (!this.element.value || this.element.value.trim() === '') {
      this.updateValue('R$ 0,00', 0, this.element.value.length);
    }
  }

  @HostListener('focus', ['$event'])
  onFocus(event: any): void {
    // Se o valor for R$ 0,00, limpar o campo para facilitar a digitação
    if (this.element.value === 'R$ 0,00') {
      this.updateValue('', 0, 0);
    }
  }

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent): void {
    // Permitir teclas de navegação e edição
    const allowedKeys = [
      'Backspace', 'Delete', 'Tab', 'Escape', 'Enter',
      'ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown',
      'Home', 'End'
    ];
    
    if (allowedKeys.includes(event.key)) {
      return;
    }
    
    // Permitir Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
    if (event.ctrlKey && ['a', 'c', 'v', 'x'].includes(event.key.toLowerCase())) {
      return;
    }
    
    // Apenas permitir números
    if (!/^\d$/.test(event.key)) {
      event.preventDefault();
    }
  }

  writeValue(value: any): void {
    if (value !== null && value !== undefined && value !== '') {
      const formattedValue = this.formatCurrency(value);
      this.updateValue(formattedValue, value, formattedValue.length);
    } else {
      this.updateValue('', 0, 0);
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.element.disabled = isDisabled;
  }

  private updateValue(displayValue: string, numericValue: number, cursorPosition: number): void {
    this.isUpdating = true;
    
    this.element.value = displayValue;
    this.element.setSelectionRange(cursorPosition, cursorPosition);
    
    // Notificar o Angular sobre a mudança (valor numérico)
    this.onChange(numericValue);
    
    setTimeout(() => {
      this.isUpdating = false;
    }, 0);
  }

  private calculateCursorPosition(
    oldValue: string, 
    newValue: string, 
    oldCursorPosition: number, 
    numericLength: number
  ): number {
    // Se estamos no final do campo, manter no final
    if (oldCursorPosition >= oldValue.length) {
      return newValue.length;
    }
    
    // Calcular posição baseada no número de dígitos
    const numericPart = newValue.replace(/\D/g, '');
    if (numericLength <= 2) {
      return newValue.length; // No final para valores pequenos
    } else {
      // Posicionar após o símbolo R$ e espaços
      return Math.min(oldCursorPosition + 1, newValue.length);
    }
  }

  private formatCurrency(value: number): string {
    if (isNaN(value) || value === null || value === undefined) {
      return 'R$ 0,00';
    }

    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(value);
  }
}
