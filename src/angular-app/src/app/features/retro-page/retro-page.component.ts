import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { ApiService } from '../../services/grpc-services/api.service';
import { ActivatedRoute } from '@angular/router';
import { TuiTilesComponent, TuiTile, TuiProgressCircle, TuiProgress } from '@taiga-ui/kit';
import { NewCard } from './new-card/new-card.model';
import { ColumnContentComponent } from './column-content/column-content.component';
import { Subscription } from 'rxjs';
import { MessageHubService } from '../../services/grpc-services/message-hub.service';
import { RoomModel } from '../../services/grpc-services/models/room.model';
import { TimerTickEvent } from '../../../generated/hub_api';
import { TuiButton, tuiDialog, TuiIcon, TuiAppearance } from '@taiga-ui/core';
import { StartVoteDialogComponent } from '../start-vote-dialog/start-vote-dialog.component';
import { StartTimerDialogComponent } from '../start-timer-dialog/start-timer-dialog.component';
import { AuthService } from '../../services/grpc-services/auth.service';
import { TuiMedia } from '@taiga-ui/cdk';
@Component({
  selector: 'app-retro-page',
  templateUrl: './retro-page.component.html',
  styleUrl: './retro-page.component.less',
  imports: [
    TuiTilesComponent,
    TuiTile,
    ColumnContentComponent,
    TuiButton,
    TuiIcon,
    TuiProgress,
    TuiProgressCircle,
    TuiAppearance,
    TuiMedia,
  ],
})
export class RetroPageComponent implements OnInit, OnDestroy {
  api = inject(ApiService);
  auth = inject(AuthService);
  hub = inject(MessageHubService);
  route = inject(ActivatedRoute);
  roomId!: string;
  roomInfo!: RoomModel;
  timer?: string;

  get isTimerPaused(): boolean {
    return this.timer === undefined;
  }

  protected timerSoundOff = false;

  protected timerMaxValue = 0;
  protected timerCurrentValue = 0;
  protected order = new Map();
  protected newCards: NewCard[] = [];
  private subscription!: Subscription;
  private readonly dialogStartVote = tuiDialog(StartVoteDialogComponent, {
    dismissible: true,
    label: 'Начать голосование',
  });
  private readonly dialogStartTimer = tuiDialog(StartTimerDialogComponent, {
    dismissible: true,
    label: 'Поставить таймер',
  });
  constructor() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;
    this.roomId = id;
  }

  async ngOnInit() {
    await this.InitRoomBackend();
    const soundOff = localStorage.getItem('soundOff');
    if (soundOff && soundOff === 'true') {
      this.timerSoundOff = true;
    }
  }

  private async InitRoomBackend() {
    await this.auth.activateUser();
    await this.api.connect(this.roomId);
    this.roomInfo = await this.api.getRoom(this.roomId);
    this.subscription = this.hub.subscribe(this.roomId).subscribe({
      next: async (message) => {
        if (message.stateChanged) {
          this.roomInfo = await this.api.getRoom(this.roomId);
        }
        if (message.timerTick) {
          this.handleTimertick(message.timerTick);
        }
      },
    });
  }

  handleTimertick(event: TimerTickEvent) {
    if (event.currentValue > 0) {
      this.timer = `${event.currentValue} seconds left`;
      this.timerMaxValue = event.totalValue;
      this.timerCurrentValue = event.currentValue;
    } else {
      this.timer = undefined;
    }
  }

  async startTimer() {
    if (!this.timer) {
      this.dialogStartTimer(this.roomId).subscribe();
    } else {
      await this.api.pauseTimer(this.roomId);
    }
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  async startVote() {
    if (this.roomInfo.isVoteStarted) {
      await this.api.stopVote(this.roomId);
    } else {
      this.dialogStartVote(this.roomId).subscribe();
    }
  }

  toggleSound() {
    this.timerSoundOff = !this.timerSoundOff;
    localStorage.setItem('soundOff', this.timerSoundOff ? 'true' : 'false');
  }
}
