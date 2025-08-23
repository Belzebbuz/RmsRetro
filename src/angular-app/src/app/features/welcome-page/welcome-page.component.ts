import { Component, inject, OnInit } from '@angular/core';
import { TuiAppearance, TuiButton } from '@taiga-ui/core';
import { TuiBlockStatusComponent } from '@taiga-ui/layout';
import { ApiService } from '../../services/grpc-services/api.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-welcome-page',
  templateUrl: './welcome-page.component.html',
  styleUrl: './welcome-page.component.less',
  imports: [TuiAppearance, TuiButton, TuiBlockStatusComponent],
})
export class WelcomePageComponent {
  api = inject(ApiService);
  router = inject(Router);

  async createRoom() {
    const roomId = await this.api.initRoom();
    await this.router.navigate([roomId]);
  }
}
