import { Routes } from '@angular/router';
import { WelcomePageComponent } from './features/welcome-page/welcome-page.component';
import { RetroPageComponent } from './features/retro-page/retro-page.component';

export const routes: Routes = [
  {
    path: '',
    component: WelcomePageComponent,
  },
  {
    path: ':id',
    component: RetroPageComponent,
  },
];
