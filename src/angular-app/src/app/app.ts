import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/grpc-services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnInit {
  auth = inject(AuthService);

  async ngOnInit(): Promise<void> {
    await this.auth.activateUser();
  }
}
