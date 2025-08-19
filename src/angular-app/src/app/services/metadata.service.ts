import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MetadataService {
  playerIdKey = 'x-player-id';
  getPlayerId() {
    return localStorage.getItem(this.playerIdKey);
  }
  storePlayerId(id: string) {
    if (!id) return;
    localStorage.setItem(this.playerIdKey, id);
  }
}
