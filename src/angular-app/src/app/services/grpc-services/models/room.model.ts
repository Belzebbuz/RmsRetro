// src/app/services/grpc-services/models/room.model.ts

import {
  CardOperationTypes,
  RoomColumn,
  RoomInfo,
  RoomOperationTypes,
  TextCard,
} from '../../../../generated/api';

export class RoomModel {
  constructor(
    public roomId: string,
    public availableOperations: RoomOperationTypes[],
    public columns: ColumnModel[],
    public version: number,
    public isVoteStarted: boolean,
    public votesCount: number,
    public votesLeft: number
  ) {}

  static fromGrpc(roomInfo: RoomInfo): RoomModel {
    return new RoomModel(
      roomInfo.roomId,
      roomInfo.availableOperations,
      roomInfo.columns.map((col) => ColumnModel.fromGrpc(col, roomInfo.roomId)),
      roomInfo.version,
      roomInfo.isVoteStarted,
      roomInfo.votesCount,
      roomInfo.votesLeft
    );
  }

  can(operation: RoomOperationTypes): boolean {
    return this.availableOperations.includes(operation);
  }

  get canStartVote(): boolean {
    return this.can(RoomOperationTypes.StartVoting);
  }
  get canStopVote(): boolean {
    return this.can(RoomOperationTypes.StopVoting);
  }
  get canStartTimer(): boolean {
    return this.can(RoomOperationTypes.StartTimer);
  }
  get canStopTimer(): boolean {
    return this.can(RoomOperationTypes.PauseTimer);
  }
  get canAddCard(): boolean {
    return this.can(RoomOperationTypes.AddCard);
  }
}

export class ColumnModel {
  constructor(
    public id: string,
    public columnName: string,
    public color: string,
    public order: number,
    public cards: CardModel[]
  ) {}

  static fromGrpc(roomColumn: RoomColumn, roomId: string): ColumnModel {
    return new ColumnModel(
      roomColumn.id,
      roomColumn.columnName,
      roomColumn.color,
      roomColumn.order,
      roomColumn.cards.map((card) => CardModel.fromGrpc(card, roomId, roomColumn.id))
    );
  }
}

export class CardModel {
  public editMode = false;
  constructor(
    public roomId: string,
    public columnId: string,
    public id: string,
    public text: string,
    public order: number,
    public likesCount: number,
    public isUserLiked: boolean,
    public availableOperations: CardOperationTypes[]
  ) {}

  static fromGrpc(textCard: TextCard, roomId: string, columnId: string): CardModel {
    return new CardModel(
      roomId,
      columnId,
      textCard.id,
      textCard.text,
      textCard.order,
      textCard.likesCount,
      textCard.isUserLiked,
      textCard.availableOperations
    );
  }

  get canLike(): boolean {
    return this.can(CardOperationTypes.AddLikeCard);
  }

  get canRemoveLike(): boolean {
    return this.can(CardOperationTypes.RemoveLikeCard);
  }
  
  get canEdit(): boolean {
    return this.can(CardOperationTypes.EditCard);
  }

  get canDelete(): boolean {
    return this.can(CardOperationTypes.DeleteCard);
  }

  get canCombine(): boolean {
    return this.can(CardOperationTypes.CombineCards);
  }

  get canMove(): boolean {
    return this.can(CardOperationTypes.MoveCard);
  }

  can(operation: CardOperationTypes): boolean {
    return this.availableOperations.includes(operation);
  }
}
