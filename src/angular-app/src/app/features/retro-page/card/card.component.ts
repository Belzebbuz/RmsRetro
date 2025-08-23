import { Component, inject, Input } from '@angular/core';
import {
  TuiAppearance,
  TuiButton,
  TuiTextfieldComponent,
  TuiDropdownOptionsDirective,
  TuiDropdownDirective,
  TuiDataListComponent,
  TuiDropdownPositionSided,
  TuiDropdown,
  TuiOptGroup,
  TuiTextfieldOptionsDirective,
} from '@taiga-ui/core';
import { TuiCardLarge, TuiCardRow, TuiHeader } from '@taiga-ui/layout';
import { TuiBadgeNotification, TuiBadgedContentComponent, TuiTextarea } from '@taiga-ui/kit';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CardModel, ColumnModel } from '../../../services/grpc-services/models/room.model';
import { ApiService } from '../../../services/grpc-services/api.service';
import { TuiAutoFocus } from "@taiga-ui/cdk";

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.less'],
  imports: [
    ReactiveFormsModule,
    TuiAppearance,
    TuiButton,
    TuiBadgeNotification,
    TuiBadgedContentComponent,
    TuiTextfieldComponent,
    TuiTextarea,
    FormsModule,
    TuiCardRow,
    TuiHeader,
    TuiDropdownOptionsDirective,
    TuiDropdownDirective,
    TuiDataListComponent,
    TuiDropdownPositionSided,
    TuiDropdown,
    TuiOptGroup,
    TuiTextfieldOptionsDirective,
    TuiAutoFocus
],
})
export class CardComponent {
  @Input() card?: CardModel;
  @Input() column?: ColumnModel;

  private readonly api = inject(ApiService);
  protected optionsOpen = false;

  enableEditMode() {
    this.optionsOpen = false;
    this.card!.editMode = true;
  }
  async saveCard() {
    if (this.card) {
      await this.api.saveCard(this.card);
    }
  }
  async deleteCard() {
    if (this.card) {
      await this.api.deleteCard(this.card);
    }
  }

  async addLike() {
    if (this.card?.isUserLiked) {
      await this.api.RemoveLike(this.card!);
    } else {
      await this.api.AddLIke(this.card!);
    }
  }
}
