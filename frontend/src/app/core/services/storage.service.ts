import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class StorageService {
  private readonly tokenKey = 'rentflow_token';

  get token(): string | null { return localStorage.getItem(this.tokenKey); }
  set token(value: string | null) {
    if (value) localStorage.setItem(this.tokenKey, value);
    else localStorage.removeItem(this.tokenKey);
  }
}
