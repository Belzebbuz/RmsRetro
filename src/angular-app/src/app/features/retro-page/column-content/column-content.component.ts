import { Component, inject, Input, OnInit } from '@angular/core';
import { TuiIcon, TuiButton, tuiDialog } from '@taiga-ui/core';
import { NewCard } from '../new-card/new-card.model';
import { NewCardComponent } from '../new-card/new-card.component';
import { TuiTileHandle } from '@taiga-ui/kit';
import { ApiService } from '../../../services/grpc-services/api.service';
import { CardComponent } from '../card/card.component';
import {
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem,
  CdkDropList,
  CdkDrag,
  CdkDragPlaceholder,
  CdkDragEnd,
} from '@angular/cdk/drag-drop';
import { CommonModule } from '@angular/common';
import { ConfirmDialogComponent } from '../../confirm-dialog/confirm-dialog.component';
import {
  CardModel,
  ColumnModel,
  RoomModel,
} from '../../../services/grpc-services/models/room.model';
@Component({
  selector: 'app-column-content',
  templateUrl: './column-content.component.html',
  styleUrls: ['./column-content.component.less'],
  imports: [
    TuiIcon,
    NewCardComponent,
    TuiButton,
    TuiTileHandle,
    CardComponent,
    CdkDrag,
    CdkDropList,
    CdkDragPlaceholder,
    CommonModule,
  ],
})
export class ColumnContentComponent implements OnInit {
  @Input() column?: ColumnModel;
  @Input() room?: RoomModel;
  private readonly api = inject(ApiService);
  private readonly dialog = tuiDialog(ConfirmDialogComponent, {
    dismissible: true,
    label: 'Подтвердите действие',
  });
  protected newCards: NewCard[] = [];
  protected draggedCard: CardModel | null = null;
  protected hoveredCard: CardModel | null = null;

  constructor() {}

  ngOnInit() {}
  addCard() {
    this.newCards.push(new NewCard());
  }

  deleteNewCard(index: number) {
    if (index !== -1) {
      this.newCards.splice(index, 1);
    }
  }

  async saveCard(index: number) {
    if (index === -1) return;
    const card = this.newCards[index];
    await this.api.addCard(this.room!.roomId, this.column!.id, card);
    this.deleteNewCard(index);
  }

  isMergeTarget(card: CardModel) {
    return (
      this.hoveredCard &&
      this.draggedCard &&
      this.draggedCard.canCombine &&
      this.hoveredCard.id === card.id &&
      this.draggedCard.id !== card.id
    );
  }

  getConnectedColumns() {
    return this.room!.columns.map((column) => column.id);
  }

  async onCardDrop(event: CdkDragDrop<CardModel[]>) {
    if (this.hoveredCard && this.isMergeTarget(this.hoveredCard)) {
      this.mergeCards(this.draggedCard!, this.hoveredCard);
    } else  if (event.previousContainer === event.container) {
      const cardId = event.previousContainer.data[event.previousIndex].id;
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      await this.api.moveCard(this.room!.roomId, cardId, event.container.id, event.currentIndex);
    } else {
      const cardId = event.previousContainer.data[event.previousIndex].id;
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
      await this.api.moveCard(this.room!.roomId, cardId, event.container.id, event.currentIndex);
    }
    this.draggedCard = null;
  }

  mergeCards(deleteCard: CardModel, targetCard: CardModel) {
    this.dialog('Объединить карточки?').subscribe({
      next: async (data) => {
        if (data !== 'true') return;
        await this.api.combineCards(this.room!.roomId, deleteCard.id, targetCard.id);
      },
    });
  }
}
