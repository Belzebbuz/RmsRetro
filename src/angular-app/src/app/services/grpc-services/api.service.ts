import { inject, Injectable } from '@angular/core';
import {
  AddCardOperation,
  AddLikeCardOperation,
  ApiServiceClient,
  ApiServiceDefinition,
  CombineCardsOperation,
  ConnectRequest,
  DeleteCardOperation,
  EditCardOperation,
  GetRoomInfoRequest,
  InitRoomRequest,
  InvokeRoomOperationRequest,
  MoveCardOperation,
  PauseTimerOperation,
  RemoveLikeCardOperation,
  RoomInfo,
  StartTimerOperation,
  StartVotingOperation,
  StopVotingOperation,
} from '../../../generated/api';
import { MetadataService } from '../metadata.service';
import { GrpcService } from './grpc.service';
import { NewCard } from '../../features/retro-page/new-card/new-card.model';
import { CardModel, RoomModel } from './models/room.model';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  client: ApiServiceClient;
  metadata = inject(MetadataService);
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getApiClient(ApiServiceDefinition);
  }

  async initRoom(): Promise<string> {
    const response = await this.client.initRoom(InitRoomRequest.create());
    return response.roomId;
  }

  async connect(roomId: string){
    const request = ConnectRequest.create();
    request.roomId = roomId;
    await this.client.connect(request);
  }

  async getRoom(id: string): Promise<RoomModel> {
    const request = GetRoomInfoRequest.create();
    request.roomId = id;
    const response = await this.client.getRoomInfo(request);
    if (!response.info) throw new Error();
    return RoomModel.fromGrpc(response.info);
  }

  async addCard(roomId: string, columnId: string, card: NewCard) {
    if (!card.text) return;
    const request = InvokeRoomOperationRequest.create();
    request.roomId = roomId;
    request.add = AddCardOperation.create();
    request.add.columnId = columnId;
    request.add.text = card.text;
    await this.client.invokeRoomOperation(request);
  }

  async combineCards(roomId: string, deleteCardId: string, targetCardId: string) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = roomId;
    request.combine = CombineCardsOperation.create();
    request.combine.deleteCardId = deleteCardId;
    request.combine.targetCardId = targetCardId;
    await this.client.invokeRoomOperation(request);
  }

  async saveCard(card: CardModel) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = card.roomId;
    request.edit = EditCardOperation.create();
    request.edit.cardId = card.id;
    request.edit.text = card.text;
    await this.client.invokeRoomOperation(request);
  }
  async deleteCard(card: CardModel) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = card.roomId;
    request.delete = DeleteCardOperation.create();
    request.delete.cardId = card.id;
    await this.client.invokeRoomOperation(request);
  }
  async moveCard(roomId: string, cardId: string, newColumnId: string, newIndex: number) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = roomId;
    request.move = MoveCardOperation.create();
    request.move.cardId = cardId;
    request.move.newColumnId = newColumnId;
    request.move.newOrderId = newIndex;
    await this.client.invokeRoomOperation(request);
  }

  async startTimer(roomId: string, minutes: number) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = roomId;
    request.startTimer = StartTimerOperation.create();
    request.startTimer.minutes = minutes;
    await this.client.invokeRoomOperation(request);
  }
  async pauseTimer(roomId: string) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = roomId;
    request.pauseTimer = PauseTimerOperation.create();
    await this.client.invokeRoomOperation(request);
  }

  async startVote(roomId: string, votesPerUser: number, timerMinutes: number) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = roomId;
    request.startVoting = StartVotingOperation.create();
    request.startVoting.votesPerUser = votesPerUser;
    request.startVoting.timerMinutes = timerMinutes;
    await this.client.invokeRoomOperation(request);
  }

  async stopVote(roomId: string) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = roomId;
    request.stopVoting = StopVotingOperation.create();
    await this.client.invokeRoomOperation(request);
  }

  async AddLIke(card: CardModel) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = card.roomId;
    request.addLike = AddLikeCardOperation.create();
    request.addLike.cardId = card.id;
    await this.client.invokeRoomOperation(request);
  }

  async RemoveLike(card: CardModel) {
    const request = InvokeRoomOperationRequest.create();
    request.roomId = card.roomId;
    request.removeLike = RemoveLikeCardOperation.create();
    request.removeLike.cardId = card.id;
    await this.client.invokeRoomOperation(request);
  }
}
