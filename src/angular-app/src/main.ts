import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { Component } from '@angular/core';
import { TuiRoot } from '@taiga-ui/core';

@Component({
  standalone: true,
  selector: 'root',
  imports: [App, TuiRoot],
  template: '<tui-root> <app-root/> </tui-root>',
})
class Root {}
bootstrapApplication(Root, appConfig).catch((err) => console.error(err));
