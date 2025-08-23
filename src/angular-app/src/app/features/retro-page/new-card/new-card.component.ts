import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TuiTextfieldComponent, TuiLabel, TuiAppearance, TuiButton } from "@taiga-ui/core";
import { TuiTextarea } from "@taiga-ui/kit";
import { TuiCardLarge } from "@taiga-ui/layout";
import {FormsModule} from '@angular/forms';
import { NewCard } from './new-card.model';
import { ColumnModel } from '../../../services/grpc-services/models/room.model';
import { TuiAutoFocus } from "@taiga-ui/cdk";
@Component({
  selector: 'app-new-card',
  templateUrl: './new-card.component.html',
  styleUrls: ['./new-card.component.less'],
  imports: [FormsModule, TuiTextfieldComponent, TuiTextarea, TuiLabel,  TuiAppearance, TuiButton, TuiAutoFocus]
})
export class NewCardComponent implements OnInit {

  @Input() card?: NewCard;
  @Input() column?: ColumnModel;
  @Output() onDelete = new EventEmitter();
  @Output() onSave = new EventEmitter();
  constructor() { }

  ngOnInit() {
  }

}
