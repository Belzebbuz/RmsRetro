import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../services/grpc-services/api.service';
import { TuiDialogContext, TuiTextfieldComponent, TuiButton } from '@taiga-ui/core';
import { injectContext } from '@taiga-ui/polymorpheus';
import { TuiInputNumber } from '@taiga-ui/kit';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-start-timer-dialog',
  templateUrl: './start-timer-dialog.component.html',
  styleUrls: ['./start-timer-dialog.component.less'],
  imports: [TuiTextfieldComponent, FormsModule, TuiInputNumber, TuiButton],
})
export class StartTimerDialogComponent {
  private readonly api = inject(ApiService);
  public readonly context = injectContext<TuiDialogContext<string, string>>();
  protected timerMinutes = 1;

  protected get roomId(): string {
    return this.context.data;
  }

  async startTimer(){
    await this.api.startTimer(this.roomId, this.timerMinutes);
    this.context.completeWith('true');
  }
}
