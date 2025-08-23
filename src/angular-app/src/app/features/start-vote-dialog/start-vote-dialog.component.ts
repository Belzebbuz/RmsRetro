import { Component, inject, OnInit } from '@angular/core';
import {
  TuiDialogService,
  TuiDialogContext,
  TuiTextfieldComponent,
  TuiButton,
} from '@taiga-ui/core';
import { injectContext } from '@taiga-ui/polymorpheus';
import { ApiService } from '../../services/grpc-services/api.service';
import { FormsModule } from '@angular/forms';
import { TuiInputNumber } from '@taiga-ui/kit';
@Component({
  selector: 'app-start-vote-dialog',
  templateUrl: './start-vote-dialog.component.html',
  styleUrls: ['./start-vote-dialog.component.less'],
  imports: [TuiTextfieldComponent, FormsModule, TuiInputNumber, TuiButton],
})
export class StartVoteDialogComponent {
  private api = inject(ApiService);
  private readonly dialogs = inject(TuiDialogService);
  public readonly context = injectContext<TuiDialogContext<string, string>>();
  protected timerMinutes = 0;
  protected votesPerUser = 5;
  protected get roomId(): string {
    return this.context.data;
  }
  async startVote() {
    await this.api.startVote(this.roomId, this.votesPerUser, this.timerMinutes);
    this.context.completeWith('true');
  }
}
