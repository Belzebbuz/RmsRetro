import { Component, inject, OnInit } from '@angular/core';
import { TuiDialogContext, TuiDialogService, TuiAppearance } from '@taiga-ui/core';
import { injectContext } from '@taiga-ui/polymorpheus';
@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.component.html',
  styleUrls: ['./confirm-dialog.component.less'],
  imports: [TuiAppearance],
})
export class ConfirmDialogComponent implements OnInit {
  private readonly dialogs = inject(TuiDialogService);
  public readonly context = injectContext<TuiDialogContext<string, string>>();

  constructor() {}

  ngOnInit() {}
  protected get text(): string {
    return this.context.data;
  }
  protected confirm() {
    this.context.completeWith('true');
  }
  protected cancel() {
    this.context.completeWith('false');
  }
}
